using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Windows7;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.IO.Step21;
using Xbim.IO.Step21.Parser;
using Xbim.IO.Xml;
using XbimGeometry.Interfaces;

namespace Xbim.IO.Esent
{
    public class PersistedEntityInstanceCache : IDisposable
    {
         /// <summary>
        /// Holds a collection of all currently opened instances in this process
        /// </summary>
        static readonly HashSet<PersistedEntityInstanceCache> OpenInstances;
       
        #region ESE Database 

        private  Instance _jetInstance;
        private readonly IEntityFactory _factory;
        private Session _session;
        private JET_DBID _databaseId;
        static int cacheSizeInBytes = 128 * 1024 * 1024 ;
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
        private readonly XbimEntityCursor[] _entityTables;
        private readonly XbimCursor[] _geometryTables;
        private XbimDBAccess _accessMode;
        private string _systemPath;

       


       
        #endregion
        #region Cached data
        private readonly ConcurrentDictionary<int, IPersistEntity> _read = new ConcurrentDictionary<int, IPersistEntity>();

        internal ConcurrentDictionary<int, IPersistEntity> Read
        {
            get { return _read; }
           
        }
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
        static private readonly ComparePropertyInfo ComparePropInfo = new ComparePropertyInfo();
        private bool _caching;
        private bool _previousCaching;
        private class ComparePropertyInfo : IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x, PropertyInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(PropertyInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public PersistedEntityInstanceCache(EsentModel model, IEntityFactory factory)
        {
            _factory = factory;
            _jetInstance = CreateInstance("XbimInstance");
            _lockObject = new object();
            _model = model;
            _entityTables = new XbimEntityCursor[MaxCachedEntityTables];
            _geometryTables = new XbimCursor[MaxCachedGeometryTables];
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
                    XbimEntityCursor.CreateTable(session, dbid);
                    XbimCursor.CreateGlobalsTable(session, dbid); //create the gobals table
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

        private static bool EnsureGeometryTables(Session session, JET_DBID dbid)
        {
            
            if (!HasTable(XbimGeometryCursor.GeometryTableName, session, dbid))
                XbimGeometryCursor.CreateTable(session, dbid);
            if (!HasTable(XbimShapeGeometryCursor.GeometryTableName, session, dbid))
                XbimShapeGeometryCursor.CreateTable(session, dbid);
            if (!HasTable(XbimShapeInstanceCursor.InstanceTableName, session, dbid))
                XbimShapeInstanceCursor.CreateTable(session, dbid);
            return true;
        }

        #region Table functions

        /// <summary>
        /// Returns a cached or new entity table, assumes the database filename has been specified
        /// </summary>
        /// <returns></returns>
        internal XbimEntityCursor GetEntityTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _entityTables.Length; ++i)
                {
                    if (null != _entityTables[i] )
                    {
                        var table = _entityTables[i];
                        _entityTables[i] = null;
                        return table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new XbimEntityCursor(_model, _databaseName, openMode);
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

                        if (String.Compare(cache.DatabaseName, _databaseName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _jetInstance.Term();
                            _jetInstance = cache.JetInstance;
                            break;
                        }
                    }
                    _session = new Session(_jetInstance);
                    try
                    {
                        Api.JetAttachDatabase(_session, _databaseName, openMode == OpenDatabaseGrbit.ReadOnly ? AttachDatabaseGrbit.ReadOnly : AttachDatabaseGrbit.None);
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
                                    System.Threading.Thread.Sleep(500);
                                }
                                EsentModel.Logger.WarnFormat("Repair failed {0} after dirty shutdown, time out", _databaseName);
                            }
                            else
                            {
                                EsentModel.Logger.WarnFormat("Repair success {0} after dirty shutdown", _databaseName);
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
        internal void FreeTable(XbimEntityCursor table)
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
        public void FreeTable(XbimShapeGeometryCursor table)
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
        public void FreeTable(XbimShapeInstanceCursor table)
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
                    _model.Header = entTable.ReadHeader();
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
            var refCount = OpenInstances.Count(c => c.JetInstance == JetInstance);
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
            var tempDirectory = System.Configuration.ConfigurationManager.AppSettings["XbimTempDirectory"];
            if (!IsValidDirectory(ref tempDirectory))
            {
                tempDirectory = Path.Combine(Path.GetTempPath(), "Xbim." + Guid.NewGuid().ToString());
                if (!IsValidDirectory(ref tempDirectory))
                {
                    tempDirectory = Path.Combine(Directory.GetCurrentDirectory(),"Xbim."+ Guid.NewGuid().ToString());
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
            jetInstance.Parameters.MaxVerPages = 4096;
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
        public void ImportXbim(string importFrom, ReportProgressDelegate progressHandler = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Imports an Xml file memory model into the model server, only call when the database instances table is empty
        /// </summary>
        public void ImportIfcXml(string xbimDbName, string xmlFilename, ReportProgressDelegate progressHandler = null,bool keepOpen = false, bool cacheEntities = false)
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
            var table = GetEntityTable();
            if (cacheEntities) CacheStart();
            try
            {
                using (var transaction = table.BeginLazyTransaction())
                {

                    using (var xmlInStream = new StreamReader(inputStream, Encoding.GetEncoding("ISO-8859-9"))) //this is a work around to ensure latin character sets are read
                    {
                        using (var xmlTextReader = new XmlTextReader(xmlInStream))
                        {
                            var settings = new XmlReaderSettings {CheckCharacters = false};
                            //has no impact
                            _forwardReferences = new BlockingCollection<StepForwardReference>();
                            var xmlReader = XmlReader.Create(xmlTextReader, settings);
                            settings.CheckCharacters = false;
                            var reader = new IfcXmlReader();
                            _model.Header = reader.Read(this, table, xmlReader);
                            table.WriteHeader(_model.Header);

                        }
                    }
                    transaction.Commit();
                }
                FreeTable(table);
                if(!keepOpen) Close();
            }
            catch (Exception e)
            {
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
                throw new Exception("Error importing IfcXml file.", e);
            }
        }

        /// <summary>
        /// Imports the contents of the ifc file into the named database, the resulting database is closed after success, use Open to access
        /// </summary>
        /// <param name="toImportIfcFilename"></param>
        /// <param name="progressHandler"></param>
        /// <param name="xbimDbName"></param>
        /// <param name="keepOpen"></param>
        /// <param name="cacheEntities"></param>
        /// <param name="codePageOverride"></param>
        /// <returns></returns>
        public void ImportStep(string xbimDbName, string toImportIfcFilename,ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false,int codePageOverride = -1)
        {
            using (var reader = new FileStream(toImportIfcFilename, FileMode.Open, FileAccess.Read))
            {
                ImportStep(xbimDbName, reader, progressHandler, keepOpen, cacheEntities);
            }
        }

        internal void ImportStep(string xbimDbName, Stream stream, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {

            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) CacheStart();
            try
            {

                _forwardReferences = new BlockingCollection<StepForwardReference>();
                using (var part21Parser = new P21toIndexParser(stream, table, this, codePageOverride))
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

        public void ImportStepZip(string xbimDbName, string toImportFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
                using (var fileStream = File.OpenRead(toImportFilename))
                    ImportStepZip(xbimDbName, fileStream, progressHandler, keepOpen, cacheEntities, codePageOverride);
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
        internal void ImportStepZip(string xbimDbName, Stream fileStream, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) CacheStart();
            try
            {
                // used because - The ZipInputStream has one major advantage over using ZipFile to read a zip: 
                // it can read from an unseekable input stream - such as a WebClient download
                using (var zipStream = new ZipInputStream(fileStream))
                {
                    var entry = zipStream.GetNextEntry();
                    while (entry != null)
                    {
                        var extension = Path.GetExtension(entry.Name);
                        if (extension != null)
                        {
                            var ext = extension.ToLowerInvariant();
                            //look for a valid ifc supported file
                            if (entry.IsFile &&
                                (
                                    string.Compare(ext, ".ifc", StringComparison.OrdinalIgnoreCase) == 0 ||
                                    string.Compare(ext, ".step21", StringComparison.OrdinalIgnoreCase) == 0 ||
                                    string.Compare(ext, ".stp", StringComparison.OrdinalIgnoreCase) == 0
                                )
                                )
                            {
                                using (var zipFile = new ZipFile(fileStream))
                                {

                                    using (var reader = zipFile.GetInputStream(entry))
                                    {
                                        _forwardReferences = new BlockingCollection<StepForwardReference>();
                                        using (var part21Parser = new P21toIndexParser(reader, table, this, codePageOverride))
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
                            }
                            if (string.CompareOrdinal(ext, ".ifcxml") == 0)
                            {
                                using (var zipFile = new ZipFile(fileStream))
                                {
                                    using (var transaction = table.BeginLazyTransaction())
                                    {
                                        // XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = false };
                                        using (var xmlInStream = zipFile.GetInputStream(entry))
                                        {
                                            using (var xmlReader = new XmlTextReader(xmlInStream))
                                            {
                                                var reader = new IfcXmlReader();
                                                _model.Header = reader.Read(this, table, xmlReader);
                                                table.WriteHeader(_model.Header);
                                            }
                                        }
                                        transaction.Commit();
                                    }
                                    FreeTable(table);
                                    if (!keepOpen) Close();
                                    return;
                                }
                            }
                        }

                        entry = zipStream.GetNextEntry(); //get next entry
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
                typeIds.Add(Model.Metadata.ExpressTypeId(t));
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
                    var typeId = Model.Metadata.ExpressTypeId(t);
                    XbimInstanceHandle ih;
                    if (!entityTable.TrySeekEntityType(typeId,out ih))
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
                    long dbCount =  entityTable.RetrieveCount();
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
            entity= _read.GetOrAdd(h.EntityLabel, entity);
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
            return _factory.New(_model, type, label, true) ;
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
                    var typeId = Model.Metadata.ExpressTypeId(t);
                    XbimInstanceHandle ih;
                    if (entityTable.TrySeekEntityType(typeId, out ih))
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
                EsentModel.Logger.ErrorFormat("Illegal Entity in the model #{0}, Type {1} is defined as Abstract and cannot be created", label, type.Name);
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
           // Debug.WriteLine(Any<Xbim.Ifc2x3.SharedBldgElements.IfcWall>());
            //Debug.WriteLine(Count<Xbim.Ifc2x3.SharedBldgElements.IfcWall>());
            //IEnumerable<IfcElement> elems = OfType<IfcElement>();
            //foreach (var elem in elems)
            //{
            //    IEnumerable<IfcRelVoidsElement> rels = elem.HasOpenings;
            //    bool written = false;
            //    foreach (var rel in rels)
            //    {
            //        if (!written) { Debug.Write(elem.EntityLabel + " = "); written = true; }
            //        Debug.Write(rel.EntityLabel +", ");
            //    }
            //    if (written) Debug.WriteLine(";");
            //}
        }
        private IEnumerable<TIfcType> OfTypeUnindexed<TIfcType>(ExpressType expressType, bool activate = false) where TIfcType : IPersistEntity
        {
            var entityLabels = new HashSet<int>();
            var entityTable = GetEntityTable();
            try
            {
                //get all the type ids we are going to check for
                var typeIds = new HashSet<short>();
                foreach (var t in expressType.NonAbstractSubTypes)
                    typeIds.Add(Model.Metadata.ExpressTypeId(t));
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
                                if (activate && entity.ActivationStatus == ActivationStatus.NotActivated) //activate if required and not already done
                                {
                                    entity.Activate(() =>
                                    {
                                        var properties = entityTable.GetProperties();
                                        entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)));
                                    });
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
                    foreach (var item in CreatedNew.Where(e => e.Value is TIfcType))//.ToList()) //force the iteration to avoid concurrency clashes
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


        /// <summary>
        /// Enumerates of all instances of the specified type. The values are cached, if activate is true all the properties of the entity are loaded
        /// </summary>
        /// <typeparam name="TIfcType"></typeparam>
        /// <param name="activate">if true loads the properties of the entity</param>
        /// <param name="indexKey">if the entity has a key object, optimises to search for this handle</param>
        /// <param name="overrideType">if specified this parameter overrides the expressType used internally (but not TIfcType) for filtering purposes</param>
        /// <returns></returns>
        public IEnumerable<TIfcType> OfType<TIfcType>(bool activate = false, int? indexKey = null, ExpressType overrideType = null) where TIfcType:IPersistEntity 
        {
            //srl this needs to be removed, but preserves compatibility with old databases, the -1 should not be used in future
            int indexKeyAsInt;
            if (indexKey.HasValue) indexKeyAsInt = indexKey.Value; //this is lossy and needs to be fixed if we get large databases
            else indexKeyAsInt = -1;
            var searchingIfcType = overrideType ?? Model.Metadata.ExpressType(typeof(TIfcType));
            
            // when searching for Interface types SearchingIfcType is null
            //
            var typesToSearch = 
                searchingIfcType == null ?
                Model.Metadata.TypesImplementing(typeof(TIfcType)) : 
                searchingIfcType.NonAbstractSubTypes;

            if (searchingIfcType == null || searchingIfcType.IndexedClass)
            {
                //Set the IndexedClass Attribute of this class to ensure that seeking by index will work, this is a optimisation
                // Trying to look a class up by index that is not declared as indexeable
                var entityLabels = new HashSet<int>();
                var entityTable = GetEntityTable();
                try
                {
                    using (entityTable.BeginReadOnlyTransaction())
                    {
                        foreach (var t in typesToSearch)
                        {
                            var typeId = Model.Metadata.ExpressTypeId(t);
                            XbimInstanceHandle ih;
                            if (entityTable.TrySeekEntityType(typeId, out ih, indexKeyAsInt) && entityTable.TrySeekEntityLabel(ih.EntityLabel)) //we have the first instance
                            {
                                do
                                {
                                    IPersistEntity entity;
                                    if (_caching && _read.TryGetValue(ih.EntityLabel, out entity))
                                    {
                                        if (activate && entity.ActivationStatus == ActivationStatus.NotActivated) //activate if required and not already done
                                        {
                                            var properties = entityTable.GetProperties();
                                            entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, true);
                                            entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)));
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
                                            // the attributes of this entity have not been loaded yet
                                            entity = _factory.New(_model, ih.EntityType, ih.EntityLabel, false);

                                        if (_caching) entity = _read.GetOrAdd(ih.EntityLabel, entity);
                                        entityLabels.Add(entity.EntityLabel);
                                        yield return (TIfcType)entity;
                                    }
                                } while (entityTable.TryMoveNextEntityType(out ih) && entityTable.TrySeekEntityLabel(ih.EntityLabel));
                            }
                        }
                    }
                    // todo: bonghi: check with SRL, I'm failing to understand the following behaviour when using indexkey.
                    // 
                    if (_caching) //look in the createnew cache and find the new ones only
                    {
                        foreach (var item in CreatedNew.Where(e => e.Value is TIfcType))//.ToList())
                        {
                            if (!indexKey.HasValue) //get all of the type
                            {
                                if (entityLabels.Add(item.Key))
                                    yield return (TIfcType)item.Value;
                            }
                            else
                            {
                                // todo: bonghi: note the following ( SearchingIfcType != null ) has been added for cases when querying interfaces, but is probably not correct
                                //
                                if (searchingIfcType != null && searchingIfcType.GetIndexedValues(item.Value).Contains(indexKey.Value)) // get all types that match the index key
                                {
                                    if (entityLabels.Add(item.Key))
                                        yield return (TIfcType)item.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    FreeTable(entityTable);
                }
            }
            else
            {
                Debug.Assert(indexKeyAsInt == -1, "Trying to look a class up by index key, but the class is not indexed");
                foreach (var item in OfTypeUnindexed<TIfcType>(searchingIfcType, activate))
                    yield return item;
            }
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

        public void SaveAs(XbimStorageType storageType, string storageFileName, ReportProgressDelegate progress = null, IDictionary<int, int> map = null)
        {
            switch (storageType)
            {
                case XbimStorageType.IFCXML:
                    SaveAsIfcXml(storageFileName);
                    break;
                case XbimStorageType.Step21:
                    SaveAsIfc(storageFileName,map);
                    break;
                case XbimStorageType.Step21Zip:
                    SaveAsIfcZip(storageFileName);
                    break;
                case XbimStorageType.XBIM:
                    Debug.Assert(false, "Incorrect call, see XbimModel.SaveAs");
                    break;
            }

        }

        private void SaveAsIfcZip(string storageFileName)
        {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(storageFileName))) //make sure we have an extension
                storageFileName = Path.ChangeExtension(storageFileName, "IfcZip");
            var fileBody = Path.ChangeExtension(Path.GetFileName(storageFileName),"ifc");
            var entityTable = GetEntityTable();
            FileStream fs = null;
            ZipOutputStream zipStream = null;
            try
            {
                fs = new FileStream(storageFileName, FileMode.Create, FileAccess.Write);
                zipStream = new ZipOutputStream(fs);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
                var newEntry = new ZipEntry(fileBody) {DateTime = DateTime.Now};
                zipStream.PutNextEntry(newEntry);
                using (entityTable.BeginReadOnlyTransaction())
                {
                    using (TextWriter tw = new StreamWriter(zipStream))
                    {
                        var p21 = new Part21FileWriter();
                        p21.Write(_model, tw, Model.Metadata);
                        tw.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to write IfcZip file " + storageFileName, e);
            }
            finally
            {
                if (fs != null) fs.Close();
                if (zipStream != null) zipStream.Close();
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
                        var p21 = new Part21FileWriter();
                        p21.Write(_model, tw, Model.Metadata, map);
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
            FileStream xmlOutStream = null;
            try
            {
                xmlOutStream = new FileStream(storageFileName, FileMode.Create, FileAccess.ReadWrite);
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(xmlOutStream, settings))
                {
                    var writer = new IfcXmlWriter();
                    writer.Write(_model, xmlWriter);
                }
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to write IfcXml file " + storageFileName, e);
            }
            finally
            {
                if (xmlOutStream != null) xmlOutStream.Close();
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


        private static MemberExpression GetIndexablePropertyOnLeft(Expression leftSide)
        {
            var mex = leftSide as MemberExpression;
            if (leftSide.NodeType == ExpressionType.Call)
            {
                var call = leftSide as MethodCallExpression;
                if (call != null && call.Method.Name == "CompareString")
                {
                    mex = call.Arguments[0] as MemberExpression;
                }
            }
            return mex;
        }


        private static object GetRight(Expression leftSide, Expression rightSide)
        {
            if (leftSide.NodeType == ExpressionType.Call)
            {
                var call = leftSide as MethodCallExpression;
                if (call != null && call.Method.Name == "CompareString")
                {
                    var evalRight = Expression.Lambda(call.Arguments[1], null);
                    //Compile it, invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
                }
            }
            //rightside is where we get our hash...
            switch (rightSide.NodeType)
            {
                //shortcut constants, dont eval, will be faster
                case ExpressionType.Constant:
                    var constExp
                        = (ConstantExpression)rightSide;
                    return (constExp.Value);

                //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
                default:
                    //Lambdas can be created from expressions... yay
                    var evalRight = Expression.Lambda(rightSide, null);
                    //Compile and invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
            }
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            var indexFound = false;
            var type = typeof(T);
            var expressType = Model.Metadata.ExpressType(type);
           
            var predicate = expr.Compile();
            if (expressType.HasIndexedAttribute) //we can use a secondary index to look up
            {
                //our indexes work from the hash values of that which is indexed, regardless of type

                //indexes only work on equality expressions here
                //this  matches "Property" = "Value"
                switch (expr.Body.NodeType)
                {
                    case ExpressionType.Equal:
                        //Equality is a binary expression
                        var binExp = (BinaryExpression)expr.Body;
                        //Get some aliases for either side
                        var leftSide = binExp.Left;
                        var rightSide = binExp.Right;

                        var hashRight = GetRight(leftSide, rightSide);

                        //if we were able to create a hash from the right side (likely)
                        var returnedEx = GetIndexablePropertyOnLeft(leftSide);
                        if (returnedEx != null)
                        {
                            //cast to MemberExpression - it allows us to get the property
                            var propExp = returnedEx;
                        
                            if (expressType.IndexedProperties.Contains(propExp.Member)) //we have a primary key match
                            {
                                var entity = hashRight as IPersistEntity;
                                if (entity != null)
                                {
                                    indexFound = true;
                                    foreach (var item in OfType<T>(true, entity.EntityLabel).Where(item => predicate(item)))
                                    {
                                        yield return item;
                                    }
                                }
                            }
                        }
                        break;
                    case ExpressionType.Call:
                        var callExp = (MethodCallExpression)expr.Body;
                        if (callExp.Method.Name == "Contains")
                        {
                            var keyExpr = callExp.Arguments[0];
                            if (keyExpr.NodeType == ExpressionType.Constant)
                            {
                                var constExp = (ConstantExpression)keyExpr;
                                var key = constExp.Value;
                                if (callExp.Object != null && callExp.Object.NodeType == ExpressionType.MemberAccess)
                                {
                                    var memExp = (MemberExpression)callExp.Object;
                                    var pInfo = (PropertyInfo)(memExp.Member);
                                    if (expressType.IndexedProperties.Contains(pInfo, ComparePropInfo)) //we have a primary key match
                                    {
                                        var entity = key as IPersistEntity;
                                        if (entity != null)
                                        {
                                            indexFound = true;
                                            foreach (var item in OfType<T>(true, entity.EntityLabel).Where(item => predicate(item)))
                                            {
                                                yield return item;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            //we cannot optimise so just do it
            if (!indexFound)
            {
                foreach (var item in OfType<T>(true))
                {
                    if (predicate(item)) 
                        yield return item;
                }
            }
        }

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

        internal T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, XbimReadWriteTransaction txn, bool includeInverses, PropertyTranformDelegate propTransform = null) where T : IPersistEntity
        {
            //check if the transaction needs pulsing
            var toCopyHandle = toCopy.GetHandle();

            XbimInstanceHandle copyHandle;
            if (mappings.TryGetValue(toCopyHandle, out copyHandle))
            {
                var v = this.GetInstance(copyHandle);
                Debug.Assert(v != null);
                return (T)v;
            }
            txn.Pulse();
            var expressType = Model.Metadata.ExpressType(toCopy);
            var copyLabel = toCopy.EntityLabel;
            copyHandle = InsertNew(expressType.Type, copyLabel);
            mappings.Add(toCopyHandle, copyHandle);

            var theCopy = _factory.New(_model, copyHandle.EntityType, copyHandle.EntityLabel, true);
            _read.TryAdd(copyHandle.EntityLabel, theCopy);
            CreatedNew.TryAdd(copyHandle.EntityLabel, theCopy);
            ModifiedEntities.TryAdd(copyHandle.EntityLabel, theCopy);

            
            var props = expressType.Properties.Values.Where(p => !p.EntityAttribute.IsDerivedOverride);
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
                    prop.PropertyInfo.SetValue(theCopy, InsertCopy((IPersistEntity)value, mappings, txn, includeInverses, propTransform), null);
                }
                else if (!isInverse && typeof(IList).IsAssignableFrom(theType))
                {
                    var itemType = theType.GetItemTypeFromGenericType();

                    var copyColl = prop.PropertyInfo.GetValue(theCopy, null) as IList;
                    if(copyColl == null)
                        throw new XbimException(string.Format("Unexpected collection type ({0}) found", itemType.Name));

                    foreach (var item in (IExpressEnumerable)value)
                    {
                        var actualItemType = item.GetType();
                        if (actualItemType.IsValueType || typeof(ExpressType).IsAssignableFrom(actualItemType))
                            copyColl.Add(item);
                        else if (typeof(IPersistEntity).IsAssignableFrom(actualItemType))
                        {
                            var cpy = InsertCopy((IPersistEntity)item, mappings, txn, includeInverses, propTransform);
                            copyColl.Add(cpy);
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
                            InsertCopy(ent, mappings, txn, includeInverses, propTransform);
                    }
                }
                else if (isInverse && value is IPersistEntity) //it is an inverse and has a single value
                {
                    XbimInstanceHandle h;
                    var v = (IPersistEntity)value;
                    if (!mappings.TryGetValue(v.GetHandle(), out h))
                        InsertCopy(v, mappings, txn, includeInverses, propTransform);
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

        public string DatabaseName 
        {
            get
            {
                return _databaseName;
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
        internal void  BeginCaching()
        {
            if(!_caching) _read.Clear();
            ModifiedEntities.Clear();
            CreatedNew.Clear();
            _previousCaching = _caching;
            _caching = true;
        }
        /// <summary>
        /// Clears any cached objects and terminates further caching
        /// </summary>
        internal void  EndCaching()
        {
            if(!_previousCaching) _read.Clear();
            ModifiedEntities.Clear();
            CreatedNew.Clear();
            _caching = _previousCaching;
        }

        /// <summary>
        /// Writes the content of the modified cache to the table, assumes a transaction is in scope, modified and createnew caches are cleared
        /// </summary>
        internal void Write(XbimEntityCursor entityTable)
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
     
        internal XbimGeometryHandleCollection GetGeometryHandles(XbimGeometryType geomType=XbimGeometryType.TriangulatedMesh, XbimGeometrySort sortOrder=XbimGeometrySort.OrderByIfcSurfaceStyleThenIfcType)
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
            Debug.Assert(ModifiedEntities.Count == 0 && CreatedNew.Count==0);
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

        internal XbimShapeGeometryCursor GetShapeGeometryTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null != _geometryTables[i] && _geometryTables[i] is XbimShapeGeometryCursor)
                    {
                        var table = _geometryTables[i];
                        _geometryTables[i] = null;
                        return (XbimShapeGeometryCursor)table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new XbimShapeGeometryCursor(_model, _databaseName, openMode);
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
            var  returnVal  = true;
            returnVal &= DeleteJetTable(XbimShapeInstanceCursor.InstanceTableName);
            returnVal &= DeleteJetTable(XbimGeometryCursor.GeometryTableName);
            returnVal &= DeleteJetTable(XbimShapeGeometryCursor.GeometryTableName);                
            return returnVal;
        }


        internal bool DatabaseHasInstanceTable()
        {
            return HasTable(XbimShapeInstanceCursor.InstanceTableName);
        }

        internal bool DatabaseHasGeometryTable()
        {
            return HasTable(XbimGeometryCursor.GeometryTableName);
        }
        
        private bool HasTable(string name)
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

        internal XbimShapeInstanceCursor GetShapeInstanceTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (_lockObject)
            {
                for (var i = 0; i < _geometryTables.Length; ++i)
                {
                    if (null != _geometryTables[i] && _geometryTables[i] is XbimShapeInstanceCursor)
                    {
                        var table = _geometryTables[i];
                        _geometryTables[i] = null;
                        return (XbimShapeInstanceCursor)table;
                    }
                }
            }
            var openMode = AttachedDatabase();
            return new XbimShapeInstanceCursor(_model, _databaseName, openMode);
        }
    }
}


