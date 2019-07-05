using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Windows7;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.IO.Step21;
using Xbim.IO.Step21.Parser;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using Xbim.IO.Xml;
using Xbim.Common.Step21;

namespace Xbim.IO.Esent
{
    public class PersistedEntityInstanceCache : IDisposable
    {
        /// <summary>
        /// Holds a collection of all currently opened instances in this process
        /// </summary>
        static readonly HashSet<PersistedEntityInstanceCache> OpenInstances;

        #region ESE Database

        private Instance _jetInstance;
        private readonly IEntityFactory _factory;
        private Session _session;
        private JET_DBID _databaseId;

        static int cacheSizeInBytes = 128 * 1024 * 1024;
        private const int MaxCachedEntityTables = 32;
        private const int MaxCachedGeometryTables = 32;


        static PersistedEntityInstanceCache()
        {
            SystemParameters.DatabasePageSize = 4096;
            SystemParameters.CacheSizeMin = cacheSizeInBytes / SystemParameters.DatabasePageSize;
            SystemParameters.CacheSizeMax = cacheSizeInBytes / SystemParameters.DatabasePageSize;
            SystemParameters.MaxInstances = 128; //maximum number of models that can be opened at once, the abs max is 1024
            OpenInstances = new HashSet<PersistedEntityInstanceCache>();
        }

        internal static int ModelOpenCount
        {
            get
            {
                return OpenInstances.Count;
            }
        }
        /// <summary>
        /// Holds the session and transaction state
        /// </summary>
        private readonly object _lockObject;
        private readonly EsentEntityCursor[] _entityTables;
        private readonly EsentCursor[] _geometryTables;
        private XbimDBAccess _accessMode;
        private string _systemPath;

        #endregion
        #region Cached data
        private readonly ConcurrentDictionary<int, IPersistEntity> _read = new ConcurrentDictionary<int, IPersistEntity>();

        internal ConcurrentDictionary<int, IPersistEntity> Read
        {
            get { return _read; }

        }
        //Entities are only added to this collection in EsentModel.HandleEntityChange which is a single point of access.
        protected ConcurrentDictionary<int, IPersistEntity> ModifiedEntities = new ConcurrentDictionary<int, IPersistEntity>();
        protected ConcurrentDictionary<int, IPersistEntity> CreatedNew = new ConcurrentDictionary<int, IPersistEntity>();
        private BlockingCollection<StepForwardReference> _forwardReferences = new BlockingCollection<StepForwardReference>();

        internal BlockingCollection<StepForwardReference> ForwardReferences
        {
            get { return _forwardReferences; }
        }
        #endregion

        private string _databaseName;
        private readonly EsentModel _model;
        private bool _disposed;
        private bool _caching;
        private bool _previousCaching;

        public PersistedEntityInstanceCache(EsentModel model, IEntityFactory factory)
        {
            _factory = factory;
            _jetInstance = CreateInstance("XbimInstance");
            _lockObject = new object();
            _model = model;
            _entityTables = new EsentEntityCursor[MaxCachedEntityTables];
            _geometryTables = new EsentCursor[MaxCachedGeometryTables];
        }

        public XbimDBAccess AccessMode
        {
            get { return _accessMode; }
        }

        /// <summary>
        /// Creates an empty xbim file, overwrites any existing file of the same name
        /// throw a create failed exception if unsuccessful
        /// </summary>
        /// <returns></returns>
        internal void CreateDatabase(string fileName)
        {
            using (var session = new Session(_jetInstance))
            {
                JET_DBID dbid;
                Api.JetCreateDatabase(session, fileName, null, out dbid, CreateDatabaseGrbit.OverwriteExisting);
                try
                {
                    EsentEntityCursor.CreateTable(session, dbid);
                    EsentCursor.CreateGlobalsTable(session, dbid); //create the gobals table
                    EnsureGeometryTables(session, dbid);
                }
                catch (Exception)
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                    lock (OpenInstances)
                    {
                        Api.JetDetachDatabase(session, fileName);
                        OpenInstances.Remove(this);
                    }
                    File.Delete(fileName);
                    throw;
                }
            }
        }

        internal bool EnsureGeometryTables()
        {
            return EnsureGeometryTables(_session, _databaseId);
        }

        internal void ClearGeometryTables()
        {
            try
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null == _geometryTables[i])
                        continue;
                    _geometryTables[i].Dispose();
                    _geometryTables[i] = null;
                }

                try
                {
                    Api.JetDeleteTable(_session, _databaseId, EsentShapeGeometryCursor.GeometryTableName);
                }
                catch (Exception)
                {
                    //
                }

                try
                {
                    Api.JetDeleteTable(_session, _databaseId, EsentShapeInstanceCursor.InstanceTableName);
                }
                catch (Exception)
                {
                    //
                }
                EnsureGeometryTables(_session, _databaseId);
            }
            catch (Exception e)
            {
                throw new Exception("Could not clear existing geometry tables", e);
            }

        }

        private static bool EnsureGeometryTables(Session session, JET_DBID dbid)
        {

            if (!HasTable(XbimGeometryCursor.GeometryTableName, session, dbid))
                XbimGeometryCursor.CreateTable(session, dbid);
            if (!HasTable(EsentShapeGeometryCursor.GeometryTableName, session, dbid))
                EsentShapeGeometryCursor.CreateTable(session, dbid);
            if (!HasTable(EsentShapeInstanceCursor.InstanceTableName, session, dbid))
                EsentShapeInstanceCursor.CreateTable(session, dbid);
            return true;
        }

        #region Table functions

        /// <summary>
        /// Returns a cached or new entity table, assumes the database filename has been specified
        /// </summary>
        /// <returns></returns>
        internal EsentEntityCursor GetEntityTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _entityTables.Length; ++i)
                {
                    if (null != _entityTables[i])
                    {
                        var table = _entityTables[i];
                        _entityTables[i] = null;
                        return table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new EsentEntityCursor(_model, _databaseName, openMode);
        }

        private OpenDatabaseGrbit AttachedDatabase()
        {
            var openMode = OpenDatabaseGrbit.None;
            if (_accessMode == XbimDBAccess.Read)
                openMode = OpenDatabaseGrbit.ReadOnly;
            if (_session == null)
            {
                lock (OpenInstances) //if a db is opened twice we use the same instance
                {
                    foreach (var cache in OpenInstances)
                    {
                        if (string.Compare(cache.DatabaseName, _databaseName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _jetInstance.Term();
                            _jetInstance = cache.JetInstance;
                            break;
                        }
                    }
                    _session = new Session(_jetInstance);
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(_databaseName))
                            Api.JetAttachDatabase(_session, _databaseName, AttachDatabaseGrbit.None);
                    }
                    catch (EsentDatabaseDirtyShutdownException)
                    {
                        // try and fix the problem with the badly shutdown database
                        var startInfo = new ProcessStartInfo("EsentUtl.exe")
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = String.Format("/p \"{0}\" /o ", _databaseName)
                        };
                        using (var proc = Process.Start(startInfo))
                        {
                            if (proc != null && proc.WaitForExit(60000) == false) //give in if it takes more than a minute
                            {
                                // timed out.
                                if (!proc.HasExited)
                                {
                                    proc.Kill();
                                    // Give the process time to die, as we'll likely be reading files it has open next.
                                    Thread.Sleep(500);
                                }
                                Model.Logger.LogWarning("Repair failed {0} after dirty shutdown, time out", _databaseName);
                            }
                            else
                            {
                                Model.Logger.LogWarning("Repair success {0} after dirty shutdown", _databaseName);
                                if (proc != null) proc.Close();
                                //try again
                                Api.JetAttachDatabase(_session, _databaseName, openMode == OpenDatabaseGrbit.ReadOnly ? AttachDatabaseGrbit.ReadOnly : AttachDatabaseGrbit.None);
                            }
                        }
                    }
                    OpenInstances.Add(this);
                    Api.JetOpenDatabase(_session, _databaseName, String.Empty, out _databaseId, openMode);
                }
            }
            return openMode;
        }

        /// <summary>
        /// Returns a cached or new Geometry Table, assumes the database filename has been specified
        /// </summary>
        /// <returns></returns>
        internal XbimGeometryCursor GetGeometryTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null != _geometryTables[i] && _geometryTables[i] is XbimGeometryCursor)
                    {
                        var table = _geometryTables[i];
                        _geometryTables[i] = null;
                        return (XbimGeometryCursor)table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new XbimGeometryCursor(_model, _databaseName, openMode);
        }

        /// <summary>
        /// Free a table. This will cache the table if the cache isn't full
        /// and dispose of it otherwise.
        /// </summary>
        /// <param name="table">The cursor to free.</param>
        internal void FreeTable(EsentEntityCursor table)
        {
            Debug.Assert(null != table, "Freeing a null table");

            lock (_lockObject)
            {
                for (var i = 0; i < _entityTables.Length; ++i)
                {
                    if (null == _entityTables[i])
                    {
                        _entityTables[i] = table;
                        return;
                    }
                }
            }

            // Didn't find a slot to cache the cursor in, throw it away
            table.Dispose();
        }

        /// <summary>
        /// Free a table. This will cache the table if the cache isn't full
        /// and dispose of it otherwise.
        /// </summary>
        /// <param name="table">The cursor to free.</param>
        public void FreeTable(XbimGeometryCursor table)
        {
            Debug.Assert(null != table, "Freeing a null table");

            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null == _geometryTables[i])
                    {
                        _geometryTables[i] = table;
                        return;
                    }
                }
            }

            // Didn't find a slot to cache the cursor in, throw it away
            table.Dispose();
        }

        /// <summary>
        /// Free a table. This will cache the table if the cache isn't full
        /// and dispose of it otherwise.
        /// </summary>
        /// <param name="table">The cursor to free.</param>
        public void FreeTable(EsentShapeGeometryCursor table)
        {
            Debug.Assert(null != table, "Freeing a null table");

            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null == _geometryTables[i])
                    {
                        _geometryTables[i] = table;
                        return;
                    }
                }
            }

            // Didn't find a slot to cache the cursor in, throw it away
            table.Dispose();
        }

        /// <summary>
        /// Free a table. This will cache the table if the cache isn't full
        /// and dispose of it otherwise.
        /// </summary>
        /// <param name="table">The cursor to free.</param>
        public void FreeTable(EsentShapeInstanceCursor table)
        {
            Debug.Assert(null != table, "Freeing a null table");

            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null == _geometryTables[i])
                    {
                        _geometryTables[i] = table;
                        return;
                    }
                }
            }

            // Didn't find a slot to cache the cursor in, throw it away
            table.Dispose();
        }
        #endregion

        /// <summary>
        ///  Opens an xbim model server file, exception is thrown if errors are encountered
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="accessMode"></param>
        internal void Open(string filename, XbimDBAccess accessMode = XbimDBAccess.Read)
        {
            Close();
            _databaseName = Path.GetFullPath(filename); //success store the name of the DB file
            _accessMode = accessMode;
            _caching = false;
            var entTable = GetEntityTable();
            try
            {
                using (entTable.BeginReadOnlyTransaction())
                {
                    _model.InitialiseHeader(entTable.ReadHeader());
                }
            }
            catch (Exception e)
            {
                Close();
                throw new XbimException("Failed to open " + filename, e);
            }
            finally
            {
                FreeTable(entTable);
            }
        }

        /// <summary>
        /// Clears all contents from the cache and closes any connections
        /// </summary>
        public void Close()
        {
            // contributed by @Sense545
            int refCount;
            lock (OpenInstances)
            {
                refCount = OpenInstances.Count(c => c.JetInstance == JetInstance);
            }
            var disposeTable = (refCount != 0); //only dispose if we have not terminated the instance
            CleanTableArrays(disposeTable);
            EndCaching();

            if (_session == null)
                return;
            Api.JetCloseDatabase(_session, _databaseId, CloseDatabaseGrbit.None);
            lock (OpenInstances)
            {
                OpenInstances.Remove(this);
                refCount = OpenInstances.Count(c => string.Compare(c.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase) == 0);
                if (refCount == 0) //only detach if we have no more references
                    Api.JetDetachDatabase(_session, _databaseName);
            }
            _databaseName = null;
            _session.Dispose();
            _session = null;
        }

        private void CleanTableArrays(bool disposeTables)
        {
            for (var i = 0; i < _entityTables.Length; ++i)
            {
                if (null == _entityTables[i])
                    continue;
                if (disposeTables)
                    _entityTables[i].Dispose();
                _entityTables[i] = null;
            }
            for (var i = 0; i < _geometryTables.Length; ++i)
            {
                if (null == _geometryTables[i])
                    continue;
                if (disposeTables)
                    _geometryTables[i].Dispose();
                _geometryTables[i] = null;
            }
        }

        /// <summary>
        /// Performs a set of actions on a collection of entities inside a single read only transaction
        /// This improves database  performance for retrieving and accessing complex and deep objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            var table = GetEntityTable();
            try
            {
                using (table.BeginReadOnlyTransaction())
                {
                    foreach (var item in source)
                        body(item);
                }
            }
            finally
            {
                FreeTable(table);
            }

        }


        /// <summary>
        /// Sets up the Esent directories, can only be call before the Init method of the instance
        /// </summary>

        internal static string GetXbimTempDirectory()
        {
            //Directories are setup using the following strategy
            //First look in the config file, then try and use windows temporary directory, then the current working directory
            var tempDirectory = ConfigurationManager.AppSettings["XbimTempDirectory"];
            if (!IsValidDirectory(ref tempDirectory))
            {
                tempDirectory = Path.Combine(Path.GetTempPath(), "Xbim." + Guid.NewGuid().ToString());
                if (!IsValidDirectory(ref tempDirectory))
                {
                    tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Xbim." + Guid.NewGuid().ToString());
                    if (!IsValidDirectory(ref tempDirectory))
                        throw new XbimException("Unable to initialise the Xbim database engine, no write access. Please set a location for the XbimTempDirectory in the config file");
                }
            }
            return tempDirectory;
        }

        /// <summary>
        /// Checks the directory is writeable and modifies to be the full path
        /// </summary>
        /// <param name="tempDirectory"></param>
        /// <returns></returns>
        private static bool IsValidDirectory(ref string tempDirectory)
        {
            var tmpFileName = Guid.NewGuid().ToString();
            var fullTmpFileName = "";
            if (!string.IsNullOrWhiteSpace(tempDirectory))
            {
                tempDirectory = Path.GetFullPath(tempDirectory);
                var deleteDir = false;
                try
                {

                    fullTmpFileName = Path.Combine(tempDirectory, tmpFileName);
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                        deleteDir = true;
                    }
                    using (File.Create(fullTmpFileName))
                    { }
                    return true;
                }
                catch (Exception)
                {
                    tempDirectory = null;
                }
                finally
                {
                    File.Delete(fullTmpFileName);
                    if (deleteDir && tempDirectory != null) Directory.Delete(tempDirectory);
                }
            }
            return false;
        }

        private Instance CreateInstance(string instanceName, bool recovery = false, bool createTemporaryTables = false)
        {
            var guid = Guid.NewGuid().ToString();
            var jetInstance = new Instance(instanceName + guid);

            if (string.IsNullOrWhiteSpace(_systemPath)) //we haven't specified a path so make one               
                _systemPath = GetXbimTempDirectory();

            jetInstance.Parameters.BaseName = "XBM";
            jetInstance.Parameters.SystemDirectory = _systemPath;
            jetInstance.Parameters.LogFileDirectory = _systemPath;
            jetInstance.Parameters.TempDirectory = _systemPath;
            jetInstance.Parameters.AlternateDatabaseRecoveryDirectory = _systemPath;
            jetInstance.Parameters.CreatePathIfNotExist = true;
            jetInstance.Parameters.EnableIndexChecking = false;       // TODO: fix unicode indexes
            jetInstance.Parameters.CircularLog = true;
            jetInstance.Parameters.CheckpointDepthMax = cacheSizeInBytes;
            jetInstance.Parameters.LogFileSize = 1024;    // 1MB logs
            jetInstance.Parameters.LogBuffers = 1024;     // buffers = 1/2 of logfile
            if (!createTemporaryTables) jetInstance.Parameters.MaxTemporaryTables = 0; //ensures no temporary files are created
            jetInstance.Parameters.MaxVerPages = 4096 * 2;
            jetInstance.Parameters.NoInformationEvent = true;
            jetInstance.Parameters.WaypointLatency = 1;
            jetInstance.Parameters.MaxSessions = 512;
            jetInstance.Parameters.MaxOpenTables = 256;

            var grbit = EsentVersion.SupportsWindows7Features
                                  ? Windows7Grbits.ReplayIgnoreLostLogs
                                  : InitGrbit.None;
            jetInstance.Parameters.Recovery = recovery;
            jetInstance.Init(grbit);

            return jetInstance;
        }

        #region Import functions
        public void ImportModel(IModel fromModel, string xbimDbName, ReportProgressDelegate progressHandler = null)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);

            try
            {
                using (var transaction = Model.BeginTransaction())
                {
                    var table = Model.GetTransactingCursor();
                    foreach (var instance in fromModel.Instances)
                    {
                        table.AddEntity(instance);
                        transaction.Pulse();
                    }
                    table.WriteHeader(fromModel.Header);
                    transaction.Commit();
                }
                //copy geometry over

                var readGeomStore = fromModel.GeometryStore;
                using (var writeGeomStore = Model.GeometryStore)
                {
                    using (var writer = writeGeomStore.BeginInit())
                    {
                        using (var reader = readGeomStore.BeginRead())
                        {
                            foreach (var shapeGeom in reader.ShapeGeometries)
                            {
                                writer.AddShapeGeometry(shapeGeom);
                            }
                            foreach (var shapeInstance in reader.ShapeInstances)
                            {
                                writer.AddShapeInstance(shapeInstance, shapeInstance.ShapeGeometryLabel);
                            }
                            foreach (var regions in reader.ContextRegions)
                            {
                                writer.AddRegions(regions);
                            }
                        }
                        writer.Commit();
                    }
                }
                Close();
            }
            catch (Exception)
            {
                Close();
                File.Delete(xbimDbName);
                throw;
            }
        }

        /// <summary>
        /// Imports the contents of the ifc file into the named database, the resulting database is closed after success, use LoadStep21 to access
        /// </summary>
        /// <param name="toImportIfcFilename"></param>
        /// <param name="progressHandler"></param>
        /// <param name="xbimDbName"></param>
        /// <param name="keepOpen"></param>
        /// <param name="cacheEntities"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        public void ImportStep(string xbimDbName, string toImportIfcFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            using (var reader = new FileStream(toImportIfcFilename, FileMode.Open, FileAccess.Read))
            {
                ImportStep(xbimDbName, reader, reader.Length, progressHandler, keepOpen, cacheEntities, codePageOverride);
            }
        }

        internal void ImportStep(string xbimDbName, Stream stream, long streamSize, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {

            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) CacheStart();
            try
            {

                _forwardReferences = new BlockingCollection<StepForwardReference>();
                using (var part21Parser = new P21ToIndexParser(stream, streamSize, table, this, codePageOverride))
                {
                    if (progressHandler != null) part21Parser.ProgressStatus += progressHandler;
                    part21Parser.Parse();
                    _model.Header = part21Parser.Header;
                    if (progressHandler != null) part21Parser.ProgressStatus -= progressHandler;
                }

                using (var transaction = table.BeginLazyTransaction())
                {
                    table.WriteHeader(_model.Header);
                    transaction.Commit();
                }
                FreeTable(table);
                if (!keepOpen) Close();
            }
            catch (Exception)
            {
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
                throw;
            }
        }

        public void ImportZip(string xbimDbName, string toImportFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            using (var fileStream = File.OpenRead(toImportFilename))
            {
                ImportZip(xbimDbName, fileStream, progressHandler, keepOpen, cacheEntities, codePageOverride);
                fileStream.Close();
            }
        }

        /// <summary>
        /// Imports an Ifc Zip file
        /// </summary>
        /// <param name="xbimDbName"></param>
        /// <param name="fileStream"></param>
        /// <param name="progressHandler"></param>
        /// <param name="keepOpen"></param>
        /// <param name="cacheEntities"></param>
        /// <param name="codePageOverride"></param>
        internal void ImportZip(string xbimDbName, Stream fileStream, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) CacheStart();
            try
            {
                // used because - The ZipInputStream has one major advantage over using ZipFile to read a zip: 
                // it can read from an unseekable input stream - such as a WebClient download
                using (var zipStream = new ZipArchive(fileStream))
                {
                    foreach (var entry in zipStream.Entries)
                    {
                        var extension = Path.GetExtension(entry.Name);
                        if (extension == null)
                            continue;

                        var ext = extension.ToLowerInvariant();
                        //look for a valid ifc supported file
                        if (
                                string.Compare(ext, ".ifc", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".step21", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".stp", StringComparison.OrdinalIgnoreCase) == 0
                            )
                        {

                            using (var reader = entry.Open())
                            {
                                _forwardReferences = new BlockingCollection<StepForwardReference>();
                                using (var part21Parser = new P21ToIndexParser(reader, entry.Length, table, this, codePageOverride))
                                {
                                    if (progressHandler != null) part21Parser.ProgressStatus += progressHandler;
                                    part21Parser.Parse();
                                    _model.Header = part21Parser.Header;
                                    if (progressHandler != null) part21Parser.ProgressStatus -= progressHandler;
                                }
                            }
                            using (var transaction = table.BeginLazyTransaction())
                            {
                                table.WriteHeader(_model.Header);
                                transaction.Commit();
                            }
                            FreeTable(table);
                            if (!keepOpen) Close();
                            return; // we only want the first file
                        }
                        if (
                            string.CompareOrdinal(ext, ".ifcxml") == 0 ||
                            string.CompareOrdinal(ext, ".stpxml") == 0 ||
                            string.CompareOrdinal(ext, ".xml") == 0
                            )
                        {
                            using (var transaction = _model.BeginTransaction())
                            {
                                // XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = false };
                                using (var xmlInStream = entry.Open())
                                {

                                    var schema = Model.Factory.SchemasIds.First();
                                    if (schema == "IFC2X3")
                                    {
                                        var reader3 = new XbimXmlReader3(GetOrCreateEntity, e =>
                                        { //add entity to modified list
                                            ModifiedEntities.TryAdd(e.EntityLabel, e);
                                            //pulse will flush the model if necessary (based on the number of entities being processed)
                                            transaction.Pulse();
                                        }, Model.Metadata);
                                        if (progressHandler != null) reader3.ProgressStatus += progressHandler;
                                        _model.Header = reader3.Read(xmlInStream, _model, entry.Length);
                                        if (progressHandler != null) reader3.ProgressStatus -= progressHandler;
                                    }
                                    else
                                    {
                                        var xmlReader = new XbimXmlReader4(GetOrCreateEntity, e =>
                                        { //add entity to modified list
                                            ModifiedEntities.TryAdd(e.EntityLabel, e);
                                            //pulse will flush the model if necessary (based on the number of entities being processed)
                                            transaction.Pulse();
                                        }, Model.Metadata, Model.Logger);
                                        if (progressHandler != null) xmlReader.ProgressStatus += progressHandler;
                                        _model.Header = xmlReader.Read(xmlInStream, _model);
                                        if (progressHandler != null) xmlReader.ProgressStatus -= progressHandler;
                                    }
                                    var cursor = _model.GetTransactingCursor();
                                    cursor.WriteHeader(_model.Header);

                                }
                                transaction.Commit();
                            }
                            FreeTable(table);
                            if (!keepOpen) Close();
                            return;
                        }
                    }
                }
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
            }
            catch (Exception)
            {
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
                throw;
            }
        }

        /// <summary>
        ///   Imports an Xml file memory model into the model server, only call when the database instances table is empty
        /// </summary>
        public void ImportIfcXml(string xbimDbName, string xmlFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false)
        {
            using (var stream = File.OpenRead(xmlFilename))
            {
                ImportIfcXml(xbimDbName, stream, progressHandler, keepOpen, cacheEntities);
            }
        }

        internal void ImportIfcXml(string xbimDbName, Stream inputStream, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            if (cacheEntities) CacheStart();
            try
            {
                using (var transaction = _model.BeginTransaction())
                {

                    var schema = Model.Factory.SchemasIds.First();
                    if (schema == "IFC2X3")
                    {
                        var reader3 = new XbimXmlReader3(GetOrCreateEntity, e =>
                        { //add entity to modified list
                            ModifiedEntities.TryAdd(e.EntityLabel, e);
                            //pulse will flush the model if necessary (based on the number of entities being processed)
                            transaction.Pulse();
                        }, Model.Metadata);
                        if (progressHandler != null) reader3.ProgressStatus += progressHandler;
                        _model.Header = reader3.Read(inputStream, _model, inputStream.Length);
                        if (progressHandler != null) reader3.ProgressStatus -= progressHandler;
                    }
                    else
                    {
                        var xmlReader = new XbimXmlReader4(GetOrCreateEntity, e =>
                        { //add entity to modified list
                            ModifiedEntities.TryAdd(e.EntityLabel, e);
                            //pulse will flush the model if necessary (based on the number of entities being processed)
                            transaction.Pulse();
                        }, Model.Metadata, _model.Logger);
                        if (progressHandler != null) xmlReader.ProgressStatus += progressHandler;
                        _model.Header = xmlReader.Read(inputStream, _model);
                        if (progressHandler != null) xmlReader.ProgressStatus -= progressHandler;
                    }
                    var cursor = _model.GetTransactingCursor();
                    cursor.WriteHeader(_model.Header);
                    transaction.Commit();
                }
                if (!keepOpen) Close();
            }
            catch (Exception e)
            {
                Close();
                File.Delete(xbimDbName);
                throw new Exception("Error importing IfcXml file.", e);
            }
        }
        private IPersistEntity GetOrCreateEntity(int label, Type type)
        {
            //return existing entity
            if (Contains(label))
                return GetInstance(label, false, true);

            //create new entity and add it to the list
            var cursor = _model.GetTransactingCursor();
            var h = cursor.AddEntity(type, label);
            var entity = _factory.New(_model, type, h.EntityLabel, true) as IPersistEntity;
            entity = _read.GetOrAdd(h.EntityLabel, entity);
            CreatedNew.TryAdd(h.EntityLabel, entity);
            return entity;
        }

        #endregion

        public bool Contains(IPersistEntity instance)
        {
            return Contains(instance.EntityLabel);
        }

        public bool Contains(int entityLabel)
        {
            if (_caching && _read.ContainsKey(entityLabel)) //check if it is cached
                return true;
            else //look in the database
            {
                var entityTable = GetEntityTable();
                try
                {
                    return entityTable.TrySeekEntityLabel(entityLabel);
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }

        /// <summary>
        /// returns the number of instances of the specified type and its sub types
        /// </summary>
        /// <typeparam name="TIfcType"></typeparam>
        /// <returns></returns>
        public long CountOf<TIfcType>() where TIfcType : IPersistEntity
        {
            return CountOf(typeof(TIfcType));

        }
        /// <summary>
        /// returns the number of instances of the specified type and its sub types
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        private long CountOf(Type theType)
        {
            var entityLabels = new HashSet<int>();
            var expressType = Model.Metadata.ExpressType(theType);
            var entityTable = GetEntityTable();
            var typeIds = new HashSet<short>();
            //get all the type ids we are going to check for
            foreach (var t in expressType.NonAbstractSubTypes)
                typeIds.Add(t.TypeId);
            try
            {

                XbimInstanceHandle ih;
                if (expressType.IndexedClass)
                {
                    foreach (var typeId in typeIds)
                    {
                        if (entityTable.TrySeekEntityType(typeId, out ih))
                        {
                            do
                            {
                                entityLabels.Add(ih.EntityLabel);
                            } while (entityTable.TryMoveNextEntityType(out ih));
                        }
                    }
                }
                else
                {
                    entityTable.MoveBeforeFirst();
                    while (entityTable.TryMoveNext())
                    {
                        ih = entityTable.GetInstanceHandle();
                        if (typeIds.Contains(ih.EntityTypeId))
                            entityLabels.Add(ih.EntityLabel);
                    }
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
            if (_caching) //look in the createdNew cache and find the new ones only
            {
                foreach (var entity in CreatedNew.Where(m => m.Value.GetType() == theType))
                    entityLabels.Add(entity.Key);

            }
            return entityLabels.Count;
        }

        public bool Any<TIfcType>() where TIfcType : IPersistEntity
        {
            var expressType = Model.Metadata.ExpressType(typeof(TIfcType));
            var entityTable = GetEntityTable();
            try
            {
                foreach (var t in expressType.NonAbstractSubTypes)
                {
                    XbimInstanceHandle ih;
                    if (!entityTable.TrySeekEntityType(t.TypeId, out ih))
                        return true;
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
            return false;
        }
        /// <summary>
        /// returns the number of instances in the model
        /// </summary>
        /// <returns></returns>
        public long Count
        {
            get
            {
                var entityTable = GetEntityTable();
                try
                {
                    long dbCount = entityTable.RetrieveCount();
                    if (_caching) dbCount += CreatedNew.Count;
                    return dbCount;
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }

        /// <summary>
        /// returns the value of the highest current entity label
        /// </summary>
        public int HighestLabel
        {
            get
            {
                var entityTable = GetEntityTable();
                try
                {
                    return entityTable.RetrieveHighestLabel();
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }



        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal IPersistEntity CreateNew(Type t)
        {
            if (!_caching)
                throw new XbimException("XbimModel.BeginTransaction must be called before editing a model");
            var cursor = _model.GetTransactingCursor();
            var h = cursor.AddEntity(t);
            var entity = _factory.New(_model, t, h.EntityLabel, true) as IPersistEntity;
            entity = _read.GetOrAdd(h.EntityLabel, entity);
            ModifiedEntities.TryAdd(h.EntityLabel, entity);
            CreatedNew.TryAdd(h.EntityLabel, entity);

            return entity;
        }

        /// <summary>
        /// Creates a new instance, this is not a reversable action, and the instance is not cached
        /// It is for performance in import and export routines and should not be used in normal code
        /// </summary>
        /// <param name="type"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        internal IPersistEntity CreateNew(Type type, int label)
        {
            return _factory.New(_model, type, label, true);
        }


        internal void AddForwardReference(StepForwardReference forwardReference)
        {
            _forwardReferences.Add(forwardReference);
        }


        /// <summary>
        /// Deprecated. Use CountOf, returns the number of instances of the specified type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public long InstancesOfTypeCount(Type t)
        {
            return CountOf(t);
        }

        /// <summary>
        /// Returns an enumeration of handles to all instances in the database and in the cache
        /// </summary>
        public IEnumerable<XbimInstanceHandle> InstanceHandles
        {
            get
            {
                var entityTable = GetEntityTable();
                try
                {
                    if (entityTable.TryMoveFirst()) // we have something
                    {
                        do
                        {
                            yield return entityTable.GetInstanceHandle();
                        }
                        while (entityTable.TryMoveNext());
                    }
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }
        /// <summary>
        /// Returns an enumeration of handles to all instances in the database or the cache of specified type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XbimInstanceHandle> InstanceHandlesOfType<TIfcType>()
        {
            var reqType = typeof(TIfcType);
            var expressType = Model.Metadata.ExpressType(reqType);
            var entityTable = GetEntityTable();
            try
            {
                foreach (var t in expressType.NonAbstractSubTypes)
                {
                    XbimInstanceHandle ih;
                    if (entityTable.TrySeekEntityType(t.TypeId, out ih))
                    {
                        yield return ih;
                        while (entityTable.TryMoveNextEntityType(out ih))
                        {
                            yield return ih;
                        }
                    }
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
        }

        /// <summary>
        /// Returns an instance of the entity with the specified label,
        /// if the instance has already been loaded it is returned from the cache
        /// if it has not been loaded a blank instance is loaded, i.e. will not have been activated
        /// </summary>
        /// <param name="label"></param>
        /// <param name="loadProperties"></param>
        /// <param name="unCached"></param>
        /// <returns></returns>
        public IPersistEntity GetInstance(int label, bool loadProperties = false, bool unCached = false)
        {

            IPersistEntity entity;
            if (_caching && _read.TryGetValue(label, out entity))
                return entity;
            return GetInstanceFromStore(label, loadProperties, unCached);
        }


        /// <summary>
        /// Looks for this instance in the cache and returns it, if not found it creates a new instance and adds it to the cache
        /// </summary>
        /// <param name="label">Entity label to create</param>
        /// <param name="type">If not null creates an instance of this type, else creates an unknown Ifc Type</param>
        /// <param name="properties">if not null populates all properties of the instance</param>
        /// <returns></returns>
        public IPersistEntity GetOrCreateInstanceFromCache(int label, Type type, byte[] properties)
        {
            Debug.Assert(_caching); //must be caching to call this

            IPersistEntity entity;
            if (_read.TryGetValue(label, out entity)) return entity;

            if (type.IsAbstract)
            {
                Model.Logger.LogError("Illegal Entity in the model #{0}, Type {1} is defined as Abstract and cannot be created", label, type.Name);
                return null;
            }

            return _read.GetOrAdd(label, l =>
            {
                var instance = _factory.New(_model, type, label, true);
                instance.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false, true);
                return instance;
            }); //might have been done by another
        }

        /// <summary>
        /// Loads a blank instance from the database, do not call this before checking that the instance is in the instances cache
        /// If the entity has already been cached it will throw an exception
        /// This is not a undoable/reversable operation
        /// </summary>
        /// <param name="entityLabel">Must be a positive value of the label</param>
        /// <param name="loadProperties">if true the properties of the object are loaded  at the same time</param>
        /// <param name="unCached">if true the object is not cached, this is dangerous and can lead to object duplicates</param>
        /// <returns></returns>
        private IPersistEntity GetInstanceFromStore(int entityLabel, bool loadProperties = false, bool unCached = false)
        {
            var entityTable = GetEntityTable();
            try
            {
                using (entityTable.BeginReadOnlyTransaction())
                {

                    if (entityTable.TrySeekEntityLabel(entityLabel))
                    {
                        var currentIfcTypeId = entityTable.GetIfcType();
                        if (currentIfcTypeId == 0) // this should never happen (there's a test for it, but old xbim files might be incorrectly identified)
                            return null;
                        IPersistEntity entity;
                        if (loadProperties)
                        {
                            var properties = entityTable.GetProperties();
                            entity = _factory.New(_model, currentIfcTypeId, entityLabel, true);
                            if (entity == null)
                            {
                                // this has been seen to happen when files attempt to instantiate abstract classes.
                                return null;
                            }
                            entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), unCached);
                        }
                        else
                            entity = _factory.New(_model, currentIfcTypeId, entityLabel, false);
                        if (_caching && !unCached)
                            entity = _read.GetOrAdd(entityLabel, entity);
                        return entity;
                    }
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
            return null;

        }

        public void Print()
        {
            Debug.WriteLine(InstanceHandles.Count());

            Debug.WriteLine(HighestLabel);
            Debug.WriteLine(Count);
            Debug.WriteLine(GeometriesCount());
        }

        private IEnumerable<TIfcType> InstancesOf<TIfcType>(IEnumerable<ExpressType> expressTypes, bool activate = false, HashSet<int> read = null) where TIfcType : IPersistEntity
        {
            var types = expressTypes as ExpressType[] ?? expressTypes.ToArray();
            if (types.Any())
            {
                var entityLabels = read ?? new HashSet<int>();
                var entityTable = GetEntityTable();

                try
                {
                    //get all the type ids we are going to check for
                    var typeIds = new HashSet<short>();
                    foreach (var t in types)
                        typeIds.Add(t.TypeId);
                    using (entityTable.BeginReadOnlyTransaction())
                    {
                        entityTable.MoveBeforeFirst();
                        while (entityTable.TryMoveNext())
                        {
                            var ih = entityTable.GetInstanceHandle();
                            if (typeIds.Contains(ih.EntityTypeId))
                            {
                                IPersistEntity entity;
                                if (_caching && _read.TryGetValue(ih.EntityLabel, out entity))
                                {
                                    if (activate && !entity.Activated)
                                    //activate if required and not already done
                                    {
                                        var properties = entityTable.GetProperties();
                                        entity.ReadEntityProperties(this,
                                            new BinaryReader(new MemoryStream(properties)));
                                        FlagSetter.SetActivationFlag(entity, true);
                                    }
                                    entityLabels.Add(entity.EntityLabel);
                                    yield return (TIfcType)entity;
                                }
                                else
                                {
                                    if (activate)
                                    {
                                        var properties = entityTable.GetProperties();
                                        entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, true);
                                        entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)));
                                    }
                                    else
                                        //the attributes of this entity have not been loaded yet
                                        entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, false);

                                    if (_caching) entity = _read.GetOrAdd(ih.EntityLabel, entity);
                                    entityLabels.Add(entity.EntityLabel);
                                    yield return (TIfcType)entity;
                                }

                            }
                        }
                    }
                    if (_caching) //look in the modified cache and find the new ones only
                    {
                        foreach (var item in CreatedNew.Where(e => e.Value is TIfcType))
                        //.ToList()) //force the iteration to avoid concurrency clashes
                        {
                            if (entityLabels.Add(item.Key))
                            {
                                yield return (TIfcType)item.Value;
                            }
                        }
                    }
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }


        /// <summary>
        /// Enumerates of all instances of the specified type. The values are cached, if activate is true all the properties of the entity are loaded
        /// </summary>
        /// <typeparam name="TOType"></typeparam>
        /// <param name="activate">if true loads the properties of the entity</param>
        /// <param name="indexKey">if the entity has a key object, optimises to search for this handle</param>
        /// <param name="overrideType">if specified this parameter overrides the expressType used internally (but not TIfcType) for filtering purposes</param>
        /// <returns></returns>
        internal IEnumerable<TOType> OfType<TOType>(bool activate = false, int? indexKey = null, ExpressType overrideType = null) where TOType : IPersistEntity
        {
            //srl this needs to be removed, but preserves compatibility with old databases, the -1 should not be used in future
            int indexKeyAsInt;
            if (indexKey.HasValue) indexKeyAsInt = indexKey.Value; //this is lossy and needs to be fixed if we get large databases
            else indexKeyAsInt = -1;
            var eType = overrideType ?? Model.Metadata.ExpressType(typeof(TOType));

            // when searching for Interface types expressType is null
            //
            var typesToSearch = eType != null ?
                eType.NonAbstractSubTypes :
                Model.Metadata.TypesImplementing(typeof(TOType));

            var unindexedTypes = new HashSet<ExpressType>();

            //Set the IndexedClass Attribute of this class to ensure that seeking by index will work, this is a optimisation
            // Trying to look a class up by index that is not declared as indexable
            var entityLabels = new HashSet<int>();
            var entityTable = GetEntityTable();
            try
            {
                using (entityTable.BeginReadOnlyTransaction())
                {
                    foreach (var expressType in typesToSearch)
                    {
                        if (!expressType.IndexedClass) //if the class is indexed we can seek, otherwise go slow
                        {
                            unindexedTypes.Add(expressType);
                            continue;
                        }

                        var typeId = expressType.TypeId;
                        XbimInstanceHandle ih;
                        if (entityTable.TrySeekEntityType(typeId, out ih, indexKeyAsInt) &&
                            entityTable.TrySeekEntityLabel(ih.EntityLabel)) //we have the first instance
                        {
                            do
                            {
                                IPersistEntity entity;
                                if (_caching && _read.TryGetValue(ih.EntityLabel, out entity))
                                {
                                    if (activate && !entity.Activated)
                                    //activate if required and not already done
                                    {
                                        var properties = entityTable.GetProperties();
                                        entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, true);
                                        entity.ReadEntityProperties(this,
                                            new BinaryReader(new MemoryStream(properties)));
                                    }
                                    entityLabels.Add(entity.EntityLabel);
                                    yield return (TOType)entity;
                                }
                                else
                                {
                                    if (activate)
                                    {
                                        var properties = entityTable.GetProperties();
                                        entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, true);
                                        entity.ReadEntityProperties(this,
                                            new BinaryReader(new MemoryStream(properties)));
                                    }
                                    else
                                        // the attributes of this entity have not been loaded yet
                                        entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, false);

                                    if (_caching) entity = _read.GetOrAdd(ih.EntityLabel, entity);
                                    entityLabels.Add(entity.EntityLabel);
                                    yield return (TOType)entity;
                                }
                            } while (entityTable.TryMoveNextEntityType(out ih) &&
                                     entityTable.TrySeekEntityLabel(ih.EntityLabel));
                        }
                    }

                }

                // we need to see if there are any objects in the cache that have not been written to the database yet.
                // 
                if (_caching) //look in the create new cache and find the new ones only
                {
                    foreach (var item in CreatedNew.Where(e => e.Value is TOType))
                    {
                        if (entityLabels.Add(item.Key))
                            yield return (TOType)item.Value;
                    }
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
            //we need to deal with types that are not indexed in the database in a single pass to save time
            // MC: Commented out this assertion because it just fires when inverse property is empty result.
            // Debug.Assert(indexKeyAsInt == -1, "Trying to look a class up by index key, but the class is not indexed");
            foreach (var item in InstancesOf<TOType>(unindexedTypes, activate, entityLabels))
                yield return item;


        }



        public void Activate(IPersistEntity entity)
        {
            var bytes = GetEntityBinaryData(entity);
            if (bytes != null)
                (entity as IInstantiableEntity).ReadEntityProperties(this, new BinaryReader(new MemoryStream(bytes)));
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        ~PersistedEntityInstanceCache()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    Close();

                }
                try
                {
                    var systemPath = _jetInstance.Parameters.SystemDirectory;
                    lock (OpenInstances)
                    {
                        OpenInstances.Remove(this);
                        var refCount = OpenInstances.Count(c => c.JetInstance == JetInstance);
                        if (refCount == 0) //only terminate if we have no more references
                        {
                            _jetInstance.Term();
                            //TODO: MC: Check this with Steve. System path was obtained from private field before and was deleted even if the instance wasn't terminated. That didn't seem to be right.
                            if (Directory.Exists(systemPath))
                                Directory.Delete(systemPath, true);
                        }
                    }

                }
                catch (Exception) //just in case we cannot delete
                {
                    // ignored
                }
                finally
                {
                    _jetInstance = null;
                }
            }
            _disposed = true;
        }


        /// <summary>
        /// Gets the entities propertyData on binary stream
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal byte[] GetEntityBinaryData(IPersistEntity entity)
        {
            var entityTable = GetEntityTable();
            try
            {
                using (entityTable.BeginReadOnlyTransaction())
                {
                    if (entityTable.TrySeekEntityLabel(entity.EntityLabel))
                        return entityTable.GetProperties();
                }
            }
            finally
            {
                FreeTable(entityTable);
            }
            return null;
        }

        public void SaveAs(StorageType storageType, string storageFileName, ReportProgressDelegate progress = null, IDictionary<int, int> map = null)
        {
            switch (storageType)
            {
                case StorageType.IfcXml:
                    SaveAsIfcXml(storageFileName);
                    break;
                case StorageType.Ifc:
                case StorageType.Stp:
                    SaveAsIfc(storageFileName, map);
                    break;
                case StorageType.IfcZip:
                case StorageType.StpZip:
                case StorageType.Zip:
                    SaveAsIfcZip(storageFileName);
                    break;
                case StorageType.Xbim:
                    Debug.Assert(false, "Incorrect call, see XbimModel.SaveAs");
                    break;
            }

        }

        private void SaveAsIfcZip(string storageFileName)
        {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(storageFileName))) //make sure we have an extension
                storageFileName = Path.ChangeExtension(storageFileName, "IfcZip");

            var ext = Path.GetExtension(storageFileName).ToLowerInvariant();
            var fileBody = ext.Contains("ifc") ?
                Path.ChangeExtension(Path.GetFileName(storageFileName), "ifc") :
                Path.ChangeExtension(Path.GetFileName(storageFileName), "stp");
            var entityTable = GetEntityTable();
            try
            {
                using (var fs = new FileStream(storageFileName, FileMode.Create, FileAccess.Write))
                {
                    using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        var newEntry = archive.CreateEntry(fileBody);
                        using (var stream = newEntry.Open())
                        {
                            using (entityTable.BeginReadOnlyTransaction())
                            {
                                using (TextWriter tw = new StreamWriter(stream))
                                {
                                    Part21Writer.Write(_model, tw, Model.Metadata);
                                    tw.Flush();
                                }
                            }
                            stream.Close();
                        }
                    }
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to write IfcZip file " + storageFileName, e);
            }
            finally
            {
                FreeTable(entityTable);
            }
        }



        private void SaveAsIfc(string storageFileName, IDictionary<int, int> map = null)
        {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(storageFileName))) //make sure we have an extension
                storageFileName = Path.ChangeExtension(storageFileName, "Ifc");
            var entityTable = GetEntityTable();
            try
            {
                using (entityTable.BeginReadOnlyTransaction())
                {
                    using (TextWriter tw = new StreamWriter(storageFileName))
                    {
                        Part21Writer.Write(_model, tw, Model.Metadata, map);
                        tw.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to write Ifc file " + storageFileName, e);
            }
            finally
            {
                FreeTable(entityTable);
            }
        }

        private void SaveAsIfcXml(string storageFileName)
        {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(storageFileName))) //make sure we have an extension
                storageFileName = Path.ChangeExtension(storageFileName, "IfcXml");
            try
            {
                using (var stream = new FileStream(storageFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    var settings = new XmlWriterSettings { Indent = true };
                    var schema = _model.Header.FileSchema.Schemas.FirstOrDefault();
                    using (var xmlWriter = XmlWriter.Create(stream, settings))
                    {
                        switch (_model.SchemaVersion)
                        {
                            case XbimSchemaVersion.Ifc2X3:
                                var writer3 = new IfcXmlWriter3();
                                writer3.Write(_model, xmlWriter, InstanceHandles.Select(i => _model.GetInstanceVolatile(i.EntityLabel)));
                                break;
                            case XbimSchemaVersion.Ifc4:
                            default:
                                var writer4 = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                                writer4.Write(_model, xmlWriter, InstanceHandles.Select(i => _model.GetInstanceVolatile(i.EntityLabel)));
                                break;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to write IfcXml file " + storageFileName, e);
            }
        }



        public void Delete_Reversable(IPersistEntity instance)
        {
            throw new NotImplementedException();
        }

        public bool Saved
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #region Support for Linq based indexed searching


        //private static MemberExpression GetIndexablePropertyOnLeft(Expression leftSide)
        //{
        //    var mex = leftSide as MemberExpression;
        //    if (leftSide.NodeType == ExpressionType.Call)
        //    {
        //        var call = leftSide as MethodCallExpression;
        //        if (call != null && call.Method.Name == "CompareString")
        //        {
        //            mex = call.Arguments[0] as MemberExpression;
        //        }
        //    }
        //    return mex;
        //}


        //private static object GetRight(Expression leftSide, Expression rightSide)
        //{
        //    if (leftSide.NodeType == ExpressionType.Call)
        //    {
        //        var call = leftSide as MethodCallExpression;
        //        if (call != null && call.Method.Name == "CompareString")
        //        {
        //            var evalRight = Expression.Lambda(call.Arguments[1], null);
        //            //Compile it, invoke it, and get the resulting hash
        //            return (evalRight.Compile().DynamicInvoke(null));
        //        }
        //    }
        //    //rightside is where we get our hash...
        //    switch (rightSide.NodeType)
        //    {
        //        //shortcut constants, dont eval, will be faster
        //        case ExpressionType.Constant:
        //            var constExp
        //                = (ConstantExpression)rightSide;
        //            return (constExp.Value);

        //        //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
        //        default:
        //            //Lambdas can be created from expressions... yay
        //            var evalRight = Expression.Lambda(rightSide, null);
        //            //Compile and invoke it, and get the resulting hash
        //            return (evalRight.Compile().DynamicInvoke(null));
        //    }
        //}

        public IEnumerable<T> Where<T>(Func<T, bool> condition) where T : IPersistEntity
        {
            return Where(condition, null, null);
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument) where T : IPersistEntity
        {
            var type = typeof(T);
            var et = Model.Metadata.ExpressType(type);
            List<ExpressType> expressTypes;
            if (et != null)
                expressTypes = new List<ExpressType> { et };
            else
            {
                //get specific interface implementations and make sure it doesn't overlap
                var implementations = Model.Metadata.ExpressTypesImplementing(type).Where(t => !t.Type.IsAbstract).ToList();
                expressTypes = implementations.Where(implementation => !implementations.Any(i => i != implementation && i.NonAbstractSubTypes.Contains(implementation))).ToList();
            }

            var canUseSecondaryIndex = inverseProperty != null && inverseArgument != null &&
                                       expressTypes.All(e => e.HasIndexedAttribute &&
                                                             e.IndexedProperties.Any(
                                                                 p => p.Name == inverseProperty));
            if (!canUseSecondaryIndex)
                return expressTypes.SelectMany(expressType => OfType<T>(true, null, expressType).Where(condition));

            //we can use a secondary index to look up
            var cache = _model._inverseCache;
            IEnumerable<T> result;
            if (cache != null && cache.TryGet(inverseProperty, inverseArgument, out result))
                return result;
            result = expressTypes.SelectMany(t => OfType<T>(true, inverseArgument.EntityLabel, t).Where(condition));
            var entities = result as IList<T> ?? result.ToList();
            if (cache != null)
                cache.Add(inverseProperty, inverseArgument, entities);
            return entities;
        }

        //public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        //{
        //    var indexFound = false;
        //    var type = typeof(T);
        //    var et = Model.Metadata.ExpressType(type);
        //    IEnumerable<ExpressType> expressTypes;
        //    if (et != null)
        //        expressTypes = new[] {et};
        //    else
        //    {
        //        //get interface implementations and make sure it doesn't overlap
        //        var implementations = Model.Metadata.ExpressTypesImplementing(type).ToList();
        //        expressTypes = implementations.Where(implementation => !implementations.Any(i => i != implementation && i.NonAbstractSubTypes.Contains(implementation.Type)));
        //    }
        //    var predicate = expr.Compile();

        //    foreach (var expressType in expressTypes)
        //    {
        //        if (expressType.HasIndexedAttribute) //we can use a secondary index to look up
        //        {
        //            //our indexes work from the hash values of that which is indexed, regardless of type

        //            //indexes only work on equality expressions here
        //            //this  matches "Property" = "Value"
        //            switch (expr.Body.NodeType)
        //            {
        //                case ExpressionType.Equal:
        //                    //Equality is a binary expression
        //                    var binExp = (BinaryExpression)expr.Body;
        //                    //Get some aliases for either side
        //                    var leftSide = binExp.Left;
        //                    var rightSide = binExp.Right;

        //                    var hashRight = GetRight(leftSide, rightSide);

        //                    //if we were able to create a hash from the right side (likely)
        //                    var returnedEx = GetIndexablePropertyOnLeft(leftSide);
        //                    if (returnedEx != null)
        //                    {
        //                        //cast to MemberExpression - it allows us to get the property
        //                        var propExp = returnedEx;

        //                        if (expressType.IndexedProperties.Contains(propExp.Member)) //we have a primary key match
        //                        {
        //                            var entity = hashRight as IPersistEntity;
        //                            if (entity != null)
        //                            {
        //                                indexFound = true;
        //                                foreach (var item in OfType<T>(true, entity.EntityLabel, expressType).Where(item => predicate(item)))
        //                                {
        //                                    yield return item;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case ExpressionType.Call:
        //                    var callExp = (MethodCallExpression)expr.Body;
        //                    if (callExp.Method.Name == "Contains")
        //                    {
        //                        var keyExpr = callExp.Arguments[0];
        //                        if (keyExpr.NodeType == ExpressionType.Constant)
        //                        {
        //                            var constExp = (ConstantExpression)keyExpr;
        //                            var key = constExp.Value;
        //                            if (callExp.Object != null && callExp.Object.NodeType == ExpressionType.MemberAccess)
        //                            {
        //                                var memExp = (MemberExpression)callExp.Object;
        //                                var pInfo = (PropertyInfo)(memExp.Member);
        //                                if (expressType.IndexedProperties.Contains(pInfo, ComparePropInfo)) //we have a primary key match
        //                                {
        //                                    var entity = key as IPersistEntity;
        //                                    if (entity != null)
        //                                    {
        //                                        indexFound = true;
        //                                        foreach (var item in OfType<T>(true, entity.EntityLabel, expressType).Where(item => predicate(item)))
        //                                        {
        //                                            yield return item;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //            }
        //        }

        //        //we cannot optimise so just do it
        //        if (!indexFound)
        //        {
        //            foreach (var item in OfType<T>(true, null, expressType))
        //            {
        //                if (predicate(item))
        //                    yield return item;
        //            }
        //        }
        //    }

        //}

        #endregion

        public IEnumerable<XbimGeometryData> GetGeometry(short typeId, int productLabel, XbimGeometryType geomType)
        {
            var geomTable = GetGeometryTable();
            try
            {
                using (geomTable.BeginReadOnlyTransaction())
                {
                    foreach (var item in geomTable.GeometryData(typeId, productLabel, geomType))
                    {
                        yield return item;
                    }
                }
            }
            finally
            {
                FreeTable(geomTable);
            }
        }

        /// <summary>
        /// Iterates over all the shape geoemtry
        /// This is a thread safe operation and can be accessed in background threads
        /// </summary>
        /// <param name="ofType"></param>
        /// <returns></returns>
        public IEnumerable<XbimGeometryData> GetGeometryData(XbimGeometryType ofType)
        {
            //Get a cached or open a new Table
            var geometryTable = GetGeometryTable();
            try
            {
                foreach (var shape in geometryTable.GetGeometryData(ofType))
                    yield return shape;
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal long GeometriesCount()
        {
            var geomTable = GetGeometryTable();
            try
            {
                return geomTable.RetrieveCount();
            }
            finally
            {
                FreeTable(geomTable);
            }
        }

        internal T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, XbimReadWriteTransaction txn, bool includeInverses, PropertyTranformDelegate propTransform = null, bool keepLabels = true) where T : IPersistEntity
        {
            //check if the transaction needs pulsing
            var toCopyHandle = toCopy.GetHandle();

            XbimInstanceHandle copyHandle;
            if (mappings.TryGetValue(toCopyHandle, out copyHandle))
            {
                var v = GetInstance(copyHandle);
                Debug.Assert(v != null);
                return (T)v;
            }
            txn.Pulse();
            var expressType = Model.Metadata.ExpressType(toCopy);
            var copyLabel = toCopy.EntityLabel;
            copyHandle = keepLabels ? InsertNew(expressType.Type, copyLabel) : InsertNew(expressType.Type);
            mappings.Add(toCopyHandle, copyHandle);

            var theCopy = _factory.New(_model, copyHandle.EntityType, copyHandle.EntityLabel, true);
            _read.TryAdd(copyHandle.EntityLabel, theCopy);
            CreatedNew.TryAdd(copyHandle.EntityLabel, theCopy);
            ModifiedEntities.TryAdd(copyHandle.EntityLabel, theCopy);


            var props = expressType.Properties.Values.Where(p => !p.EntityAttribute.IsDerived);
            if (includeInverses)
                props = props.Union(expressType.Inverses);

            foreach (var prop in props)
            {
                var value = propTransform != null ?
                    propTransform(prop, toCopy) :
                    prop.PropertyInfo.GetValue(toCopy, null);
                if (value == null) continue;

                var isInverse = (prop.EntityAttribute.Order == -1); //don't try and set the values for inverses
                var theType = value.GetType();
                //if it is an express type or a value type, set the value
                if (theType.IsValueType || typeof(ExpressType).IsAssignableFrom(theType))
                {
                    prop.PropertyInfo.SetValue(theCopy, value, null);
                }
                //else 
                else if (!isInverse && typeof(IPersistEntity).IsAssignableFrom(theType))
                {
                    prop.PropertyInfo.SetValue(theCopy, InsertCopy((IPersistEntity)value, mappings, txn, includeInverses, propTransform, keepLabels), null);
                }
                else if (!isInverse && typeof(IList).IsAssignableFrom(theType))
                {
                    var itemType = theType.GetItemTypeFromGenericType();

                    var copyColl = prop.PropertyInfo.GetValue(theCopy, null) as IList;
                    if (copyColl == null)
                        throw new XbimException(string.Format("Unexpected collection type ({0}) found", itemType.Name));

                    foreach (var item in (IExpressEnumerable)value)
                    {
                        var actualItemType = item.GetType();
                        if (actualItemType.IsValueType || typeof(ExpressType).IsAssignableFrom(actualItemType))
                            copyColl.Add(item);
                        else if (typeof(IPersistEntity).IsAssignableFrom(actualItemType))
                        {
                            var cpy = InsertCopy((IPersistEntity)item, mappings, txn, includeInverses, propTransform, keepLabels);
                            copyColl.Add(cpy);
                        }
                        else if (typeof(IList).IsAssignableFrom(actualItemType)) //list of lists
                        {
                            var listColl = (IList)item;
                            var getAt = copyColl.GetType().GetMethod("GetAt");
                            if (getAt == null) throw new Exception(string.Format("GetAt Method not found on ({0}) found", copyColl.GetType().Name));
                            var copyListColl = getAt.Invoke(copyColl, new object[] { copyColl.Count }) as IList;
                            foreach (var listItem in listColl)
                            {
                                var actualListItemType = listItem.GetType();
                                if (actualListItemType.IsValueType ||
                                    typeof(ExpressType).IsAssignableFrom(actualListItemType))
                                    copyListColl.Add(listItem);
                                else if (typeof(IPersistEntity).IsAssignableFrom(actualListItemType))
                                {
                                    var cpy = InsertCopy((IPersistEntity)listItem, mappings, txn, includeInverses, propTransform, keepLabels);
                                    copyListColl.Add(cpy);
                                }
                                else
                                    throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                        itemType.Name));
                            }
                        }
                        else
                            throw new XbimException(string.Format("Unexpected collection item type ({0}) found", itemType.Name));
                    }
                }
                else if (isInverse && value is IEnumerable<IPersistEntity>) //just an enumeration of IPersistEntity
                {
                    foreach (var ent in (IEnumerable<IPersistEntity>)value)
                    {
                        XbimInstanceHandle h;
                        if (!mappings.TryGetValue(ent.GetHandle(), out h))
                            InsertCopy(ent, mappings, txn, includeInverses, propTransform, keepLabels);
                    }
                }
                else if (isInverse && value is IPersistEntity) //it is an inverse and has a single value
                {
                    XbimInstanceHandle h;
                    var v = (IPersistEntity)value;
                    if (!mappings.TryGetValue(v.GetHandle(), out h))
                        InsertCopy(v, mappings, txn, includeInverses, propTransform, keepLabels);
                }
                else
                    throw new XbimException(string.Format("Unexpected item type ({0})  found", theType.Name));
            }
            //  if (rt != null) rt.OwnerHistory = this.OwnerHistoryAddObject;
            return (T)theCopy;
        }


        private IPersistEntity GetInstance(XbimInstanceHandle map)
        {
            return GetInstance(map.EntityLabel);
        }

        /// <summary>
        /// This function can only be called once the model is in a transaction
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityLabel"></param>
        /// <returns></returns>
        private XbimInstanceHandle InsertNew(Type type, int entityLabel)
        {
            return _model.GetTransactingCursor().AddEntity(type, entityLabel);
        }

        /// <summary>
        /// This function can only be called once the model is in a transaction
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private XbimInstanceHandle InsertNew(Type type)
        {
            return _model.GetTransactingCursor().AddEntity(type);
        }



        /// <summary>
        /// Adds an entity to the modified cache, if the entity is not already being edited
        /// Throws an exception if an attempt is made to edit a duplicate reference to the entity
        /// </summary>
        /// <param name="entity"></param>
        internal void AddModified(IPersistEntity entity)
        {
            //IPersistEntity editing;
            //if (modified.TryGetValue(entity.EntityLabel, out editing)) //it  already exists as edited
            //{
            //    if (!System.Object.ReferenceEquals(editing, entity)) //it is not the same object reference
            //        throw new XbimException("An attempt to edit a duplicate reference for #" + entity.EntityLabel + " error has occurred");
            //}
            //else
            ModifiedEntities.TryAdd(entity.EntityLabel, entity as IInstantiableEntity);
        }

        internal string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
            }

        }

        /// <summary>
        /// Returns an enumeration of all the instance labels in the model
        /// </summary>
        public IEnumerable<int> InstanceLabels
        {
            get
            {
                var entityTable = GetEntityTable();
                try
                {

                    int label;
                    if (entityTable.TryMoveFirstLabel(out label)) // we have something
                    {
                        do
                        {
                            yield return label;
                        }
                        while (entityTable.TryMoveNextLabel(out label));
                    }
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
        }


        /// <summary>
        /// Clears any cached objects and starts a new caching session
        /// </summary>
        internal void BeginCaching()
        {
            if (!_caching) _read.Clear();
            ModifiedEntities.Clear();
            CreatedNew.Clear();
            _previousCaching = _caching;
            _caching = true;
        }
        /// <summary>
        /// Clears any cached objects and terminates further caching
        /// </summary>
        internal void EndCaching()
        {
            if (!_previousCaching) _read.Clear();
            ModifiedEntities.Clear();
            CreatedNew.Clear();
            _caching = _previousCaching;
        }

        /// <summary>
        /// Writes the content of the modified cache to the table, assumes a transaction is in scope, modified and create new caches are cleared
        /// </summary>
        internal void Write(EsentEntityCursor entityTable)
        {
            foreach (var entity in ModifiedEntities.Values)
            {
                entityTable.UpdateEntity(entity);
            }
            ModifiedEntities.Clear();
            CreatedNew.Clear();
        }

        public bool HasDatabaseInstance
        {
            get
            {
                return _jetInstance != null;
            }
        }

        internal IEnumerable<IPersistEntity> Modified()
        {
            return ModifiedEntities.Values;
        }

        internal XbimGeometryHandleCollection GetGeometryHandles(XbimGeometryType geomType = XbimGeometryType.TriangulatedMesh, XbimGeometrySort sortOrder = XbimGeometrySort.OrderByIfcSurfaceStyleThenIfcType)
        {
            //Get a cached or open a new Table
            var geometryTable = GetGeometryTable();
            try
            {
                return geometryTable.GetGeometryHandles(geomType, sortOrder);
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal XbimGeometryData GetGeometryData(XbimGeometryHandle handle)
        {
            //Get a cached or open a new Table
            var geometryTable = GetGeometryTable();
            try
            {
                return geometryTable.GetGeometryData(handle);
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal IEnumerable<XbimGeometryData> GetGeometryData(IEnumerable<XbimGeometryHandle> handles)
        {
            //Get a cached or open a new Table
            var geometryTable = GetGeometryTable();
            try
            {
                foreach (var item in geometryTable.GetGeometryData(handles))
                {
                    yield return item;
                }
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal XbimGeometryHandle GetGeometryHandle(int geometryLabel)
        {
            var geometryTable = GetGeometryTable();
            try
            {
                return geometryTable.GetGeometryHandle(geometryLabel);
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal Instance JetInstance { get { return _jetInstance; } }

        internal IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {

            var ot = Model.Metadata.ExpressType(stringType.ToUpper());
            if (ot == null)
            {
                // it could be that we're searching for an interface
                //
                var implementingTypes = Model.Metadata.TypesImplementing(stringType);
                foreach (var implementingType in implementingTypes)
                {
                    foreach (var item in OfType<IPersistEntity>(activate: activate, overrideType: implementingType))
                        yield return item;
                }
            }
            else
            {
                foreach (var item in OfType<IPersistEntity>(activate: activate, overrideType: ot))
                    yield return item;

            }
        }
        /// <summary>
        /// Starts a read cache
        /// </summary>
        internal void CacheStart()
        {
            _caching = true;
        }
        /// <summary>
        /// Clears a read cache, do not call when a transaction is active
        /// </summary>
        internal void CacheClear()
        {
            Debug.Assert(ModifiedEntities.Count == 0 && CreatedNew.Count == 0);
            _read.Clear();
        }
        /// <summary>
        /// Clears a read cache, and ends further caching, do not call when a transaction is active
        /// </summary>
        internal void CacheStop()
        {
            Debug.Assert(ModifiedEntities.Count == 0 && CreatedNew.Count == 0);
            _read.Clear();
            _caching = false;
        }

        internal bool IsCaching
        {
            get
            {
                return _caching;
            }
        }

        public EsentModel Model
        {
            get
            {
                return _model;
            }
        }

        internal XbimGeometryData GetGeometryData(int geomLabel)
        {
            //Get a cached or open a new Table
            var geometryTable = GetGeometryTable();
            try
            {
                return geometryTable.GetGeometryData(geomLabel);
            }
            finally
            {
                FreeTable(geometryTable);
            }
        }

        internal EsentShapeGeometryCursor GetShapeGeometryTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null != _geometryTables[i] && _geometryTables[i] is EsentShapeGeometryCursor)
                    {
                        var table = _geometryTables[i];
                        _geometryTables[i] = null;
                        return (EsentShapeGeometryCursor)table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new EsentShapeGeometryCursor(_model, _databaseName, openMode);
        }

        internal bool DeleteJetTable(string name)
        {
            if (!HasTable(name))
                return true;
            try
            {
                Api.JetDeleteTable(_session, _databaseId, name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Deletes the geometric content of the model.
        /// </summary>
        /// <returns>True if successful.</returns>
        internal bool DeleteGeometry()
        {
            CleanTableArrays(true);
            var returnVal = true;
            returnVal &= DeleteJetTable(EsentShapeInstanceCursor.InstanceTableName);
            returnVal &= DeleteJetTable(XbimGeometryCursor.GeometryTableName);
            returnVal &= DeleteJetTable(EsentShapeGeometryCursor.GeometryTableName);
            return returnVal;
        }


        internal bool DatabaseHasInstanceTable()
        {
            return HasTable(EsentShapeInstanceCursor.InstanceTableName);
        }

        internal bool DatabaseHasGeometryTable()
        {
            return HasTable(XbimGeometryCursor.GeometryTableName);
        }

        internal bool HasTable(string name)
        {
            return HasTable(name, _session, _databaseId);
        }

        internal void Compact(string targetName)
        {
            using (var session = new Session(_jetInstance))
            {
                // For JetCompact to work the database has to be attached, but not opened 
                Api.JetAttachDatabase(session, _databaseName, AttachDatabaseGrbit.None);
                Api.JetCompact(session, _databaseName, targetName, null, null, CompactGrbit.None);
            }

        }
        private static bool HasTable(string name, Session sess, JET_DBID db)
        {
            JET_TABLEID t;
            var has = Api.TryOpenTable(sess, db, name, OpenTableGrbit.ReadOnly, out t);
            if (has)
                Api.JetCloseTable(sess, t);
            return has;
        }

        internal EsentShapeInstanceCursor GetShapeInstanceTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null != _geometryTables[i] && _geometryTables[i] is EsentShapeInstanceCursor)
                    {
                        var table = _geometryTables[i];
                        _geometryTables[i] = null;
                        return (EsentShapeInstanceCursor)table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new EsentShapeInstanceCursor(_model, _databaseName, openMode);
        }

        internal EsentEntityCursor GetWriteableEntityTable()
        {
            AttachedDatabase(); //make sure the database is attached           
            return new EsentEntityCursor(_model, _databaseName, OpenDatabaseGrbit.None);
        }
    }
}


