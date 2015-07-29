using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;
using Microsoft.Isam.Esent.Interop;
using Xbim.IO.Parser;
using System.IO;
using Xbim.Common.Exceptions;
using System.Xml;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.SelectTypes;
using System.Linq.Expressions;
using System.Reflection;
using Xbim.Ifc2x3.Kernel;
using System.Diagnostics;
using Xbim.Ifc2x3.GeometryResource;
using Microsoft.Isam.Esent.Interop.Windows7;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Concurrent;
using XbimGeometry.Interfaces;


namespace Xbim.IO
{   
    public class IfcPersistedInstanceCache : IDisposable
    {
         /// <summary>
        /// Holds a collection of all currently opened instances in this process
        /// </summary>
        static HashSet<IfcPersistedInstanceCache> openInstances;
       
        #region ESE Database 

        private  Instance _jetInstance;
        private Session _session;
        private JET_DBID _databaseId;
        static int cacheSizeInBytes = 128 * 1024 * 1024 ;
        private const int MaxCachedEntityTables = 32;
        private const int MaxCachedGeometryTables = 32;
        const int _transactionBatchSize = 100;
        static IfcPersistedInstanceCache()
        {
            SystemParameters.DatabasePageSize = 4096;
            SystemParameters.CacheSizeMin = cacheSizeInBytes / SystemParameters.DatabasePageSize;
            SystemParameters.CacheSizeMax = cacheSizeInBytes / SystemParameters.DatabasePageSize;
            SystemParameters.MaxInstances = 128; //maximum number of models that can be opened at once, the abs max is 1024
            openInstances = new HashSet<IfcPersistedInstanceCache>();
        }

        internal static int ModelOpenCount
        {
            get
            {
                return openInstances.Count();
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
        private ConcurrentDictionary<int, IPersistIfcEntity> read = new ConcurrentDictionary<int, IPersistIfcEntity>();

        internal ConcurrentDictionary<int, IPersistIfcEntity> Read
        {
            get { return read; }
           
        }
        protected ConcurrentDictionary<int, IPersistIfcEntity> modified = new ConcurrentDictionary<int, IPersistIfcEntity>();
        protected ConcurrentDictionary<int, IPersistIfcEntity> createdNew = new ConcurrentDictionary<int, IPersistIfcEntity>();
        private BlockingCollection<IfcForwardReference> forwardReferences = new BlockingCollection<IfcForwardReference>();

        internal BlockingCollection<IfcForwardReference> ForwardReferences
        {
            get { return forwardReferences; }
        }
        #endregion

        private string _databaseName;
        private XbimModel _model;
        private bool disposed = false;
        static private ComparePropertyInfo comparePropInfo = new ComparePropertyInfo();
        private bool caching = false;
        private bool previousCaching;
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

        public IfcPersistedInstanceCache(XbimModel model)
        {
            _jetInstance = CreateInstance("XbimInstance");
            this._lockObject = new Object();
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
                    XbimGeometryCursor.CreateTable(session, dbid);
                    XbimShapeGeometryCursor.CreateTable(session, dbid);
                    XbimShapeInstanceCursor.CreateTable(session, dbid);
                }
                catch (Exception e)
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                    lock (openInstances)
                    {
                        Api.JetDetachDatabase(session, fileName);
                        openInstances.Remove(this);
                    }
                    File.Delete(fileName);
                    throw e;
                }
            }
        }

        #region Table functions

        /// <summary>
        /// Returns a cached or new entity table, assumes the database filename has been specified
        /// </summary>
        /// <returns></returns>
        internal XbimEntityCursor GetEntityTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (this._lockObject)
            {
                for (int i = 0; i < this._entityTables.Length; ++i)
                {
                    if (null != this._entityTables[i] )
                    {
                        var table = this._entityTables[i];
                        this._entityTables[i] = null;
                        return table;
                    }
                }
            }
            OpenDatabaseGrbit openMode = AttachedDatabase();
            return new XbimEntityCursor(this._model, _databaseName, openMode);
        }

        private OpenDatabaseGrbit AttachedDatabase()
        {
            OpenDatabaseGrbit openMode = OpenDatabaseGrbit.None;
            if (_accessMode == XbimDBAccess.Read)
                openMode = OpenDatabaseGrbit.ReadOnly;
            if (_session == null)
            {
                lock (openInstances) //if a db is opened twice we use the same instance
                {
                    foreach (var cache in openInstances)
                    {

                        if (string.Compare(cache.DatabaseName, _databaseName, true) == 0)
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
                        ProcessStartInfo startInfo = new ProcessStartInfo("EsentUtl.exe");
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.UseShellExecute = false;
                        startInfo.CreateNoWindow = true;
                        startInfo.Arguments = String.Format("/p \"{0}\" /o ", _databaseName);
                        using (Process proc = Process.Start(startInfo))
                        {
                            if (proc.WaitForExit(60000) == false) //give in if it takes more than a minute
                            {
                                // timed out.
                                if (proc != null && !proc.HasExited)
                                {
                                    proc.Kill();
                                    // Give the process time to die, as we'll likely be reading files it has open next.
                                    System.Threading.Thread.Sleep(500);
                                }
                                XbimModel.Logger.WarnFormat("Repair failed {0} after dirty shutdown, time out", _databaseName);
                            }
                            else
                            {
                                XbimModel.Logger.WarnFormat("Repair success {0} after dirty shutdown", _databaseName);
                                proc.Close();
                                //try again
                                Api.JetAttachDatabase(_session, _databaseName, openMode == OpenDatabaseGrbit.ReadOnly ? AttachDatabaseGrbit.ReadOnly : AttachDatabaseGrbit.None);
                            }
                        }


                    }
                    openInstances.Add(this);
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
            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null != this._geometryTables[i] && this._geometryTables[i] is XbimGeometryCursor)
                    {
                        var table = this._geometryTables[i];
                        this._geometryTables[i] = null;
                        return (XbimGeometryCursor)table;
                    }
                }
            }
            OpenDatabaseGrbit openMode = AttachedDatabase();
            return new XbimGeometryCursor(this._model, _databaseName, openMode);
        }

        /// <summary>
        /// Free a table. This will cache the table if the cache isn't full
        /// and dispose of it otherwise.
        /// </summary>
        /// <param name="table">The cursor to free.</param>
        internal void FreeTable(XbimEntityCursor table)
        {
            Debug.Assert(null != table, "Freeing a null table");

            lock (this._lockObject)
            {
                for (int i = 0; i < this._entityTables.Length; ++i)
                {
                    if (null == this._entityTables[i])
                    {
                        this._entityTables[i] = table;
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

            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null == this._geometryTables[i])
                    {
                        this._geometryTables[i] = table;
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

            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null == this._geometryTables[i])
                    {
                        this._geometryTables[i] = table;
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

            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null == this._geometryTables[i])
                    {
                        this._geometryTables[i] = table;
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
        internal void Open(string filename, XbimDBAccess accessMode = XbimDBAccess.Read)
        {
            Close();
            _databaseName = Path.GetFullPath(filename); //success store the name of the DB file
            _accessMode = accessMode;
            caching = false;  
            XbimEntityCursor entTable = GetEntityTable();
            try
            {
                using (var transaction = entTable.BeginReadOnlyTransaction())
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
            int refCount = openInstances.Count(c => c.JetInstance == this.JetInstance);
            bool disposeTable = (refCount != 0); //only dispose if we have not terminated the instance
            for (int i = 0; i < this._entityTables.Length; ++i)
            {
                if (null != this._entityTables[i])
                {
                    if(disposeTable) this._entityTables[i].Dispose();
                    this._entityTables[i] = null;
                }
            }
            for (int i = 0; i < this._geometryTables.Length; ++i)
            {
                if (null != this._geometryTables[i])
                {
                    if(disposeTable) this._geometryTables[i].Dispose();
                    this._geometryTables[i] = null;
                }
            }
            EndCaching();

            if (_session != null )
            {
                Api.JetCloseDatabase(_session, _databaseId, CloseDatabaseGrbit.None);
                lock (openInstances)
                {
                    openInstances.Remove(this);
                    refCount = openInstances.Count(c => System.String.Compare(c.DatabaseName, this.DatabaseName, System.StringComparison.OrdinalIgnoreCase) == 0);
                    if (refCount == 0) //only detach if we have no more references
                        Api.JetDetachDatabase(_session, _databaseName);
                }
                this._databaseName = null;
                _session.Dispose();
                _session = null;
            }
        }

        /// <summary>
        /// Performs a set of actions on a collection of entities inside a single read only transaction
        /// This improves database  performance for retrieving and accessing complex and deep objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistIfcEntity
        {
            var table = GetEntityTable();
            try
            {
                using (var txn = table.BeginReadOnlyTransaction())
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
        /// Imports the contents of the ifc file into the named database, the resulting database is closed after success, use Open to access
        /// </summary>
        /// <param name="progressHandler"></param>
        /// <returns></returns>
        public void ImportIfc(string xbimDbName, string toImportIfcFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) this.CacheStart();
            try
            {
                using (FileStream reader = new FileStream(toImportIfcFilename, FileMode.Open, FileAccess.Read))
                {
                    forwardReferences = new BlockingCollection<IfcForwardReference>();
                    using (P21toIndexParser part21Parser = new P21toIndexParser(reader, table, this, codePageOverride))
                    {
                        if (progressHandler != null) part21Parser.ProgressStatus += progressHandler;
                        part21Parser.Parse();
                        _model.Header = part21Parser.Header;       
                        if (progressHandler != null) part21Parser.ProgressStatus -= progressHandler;
                    }
                }
                // the header used to be written a few lines above just after being assigned but should be ok here too.
                // todo: bonghi: ask SRL if it should be elsewhere
                using (var transaction = table.BeginLazyTransaction())
                {
                    table.WriteHeader(_model.Header);
                    transaction.Commit();
                }
                FreeTable(table);
                if (!keepOpen) Close();
            }
            catch (Exception e)
            {
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
                throw e;
            }
        }
        /// <summary>
        /// Imports an Ifc Zip file
        /// </summary>
        /// <param name="toImportFilename"></param>
        /// <param name="progressHandler"></param>
        public void ImportIfcZip(string xbimDbName, string toImportFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false, int codePageOverride = -1)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) this.CacheStart();
            try 
            {
                using (FileStream fileStream = File.OpenRead(toImportFilename))
                {
                    // used because - The ZipInputStream has one major advantage over using ZipFile to read a zip: 
                    // it can read from an unseekable input stream - such as a WebClient download
                    using (ZipInputStream zipStream = new ZipInputStream(fileStream))
                    {
                        ZipEntry entry = zipStream.GetNextEntry();
                        while (entry != null)
                        {
                            string ext = Path.GetExtension(entry.Name).ToLowerInvariant();
                            //look for a valid ifc supported file
                            if (entry.IsFile &&
                                (string.Compare(ext, ".ifc", true) == 0)
                                )
                            {
                                using (ZipFile zipFile = new ZipFile(toImportFilename))
                                {

                                    using (Stream reader = zipFile.GetInputStream(entry))
                                    {
                                        forwardReferences = new BlockingCollection<IfcForwardReference>();
                                        using (P21toIndexParser part21Parser = new P21toIndexParser(reader, table, this, codePageOverride))
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
                            else if(string.Compare(ext, ".ifcxml") == 0)
                            {
                                using (ZipFile zipFile = new ZipFile(toImportFilename))
                                {
                                    using (var transaction = table.BeginLazyTransaction())
                                    {
                                       // XmlReaderSettings settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = false };
                                        using (Stream xmlInStream = zipFile.GetInputStream(entry))
                                        {
                                            using (XmlTextReader xmlReader = new XmlTextReader(xmlInStream))
                                            {
                                                IfcXmlReader reader = new IfcXmlReader();
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

                            entry = zipStream.GetNextEntry(); //get next entry
                        }
                    }
                }
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
            }
            catch (Exception e)
            {
                FreeTable(table);
                Close();
                File.Delete(xbimDbName);
                throw e;
            }
        }

        /// <summary>
        /// Sets up the Esent directories, can only be call before the Init method of the instance
        /// </summary>
        
        internal static string GetXbimTempDirectory()
        {
            //Directories are setup using the following strategy
            //First look in the config file, then try and use windows temporary directory, then the current working directory
            string tempDirectory = System.Configuration.ConfigurationManager.AppSettings["XbimTempDirectory"];
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
            string tmpFileName = Guid.NewGuid().ToString();
            string fullTmpFileName = "";
            if (!string.IsNullOrWhiteSpace(tempDirectory))
            {
                tempDirectory = Path.GetFullPath(tempDirectory);
                bool deleteDir = false;
                try
                {

                    fullTmpFileName = Path.Combine(tempDirectory, tmpFileName);
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                        deleteDir = true;
                    }
                    using (FileStream fs = File.Create(fullTmpFileName)) { };
                    return true;
                }
                catch (Exception)
                {
                    tempDirectory = null;
                }
                finally
                {
                    File.Delete(fullTmpFileName);
                    if (deleteDir) Directory.Delete(tempDirectory);
                }
            }
            return false;
        }

        private Instance CreateInstance(string instanceName, string tempDirectory = null,  bool recovery = false)
        {
            string guid = Guid.NewGuid().ToString();
            var jetInstance = new Instance(instanceName+guid);           
           
            if (!string.IsNullOrWhiteSpace(tempDirectory)) //we haven't specified a path so make one
                _systemPath = tempDirectory;
            else //we are intending to use the global System Path
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
            jetInstance.Parameters.MaxTemporaryTables = 0; //ensures no temporary files are created
            jetInstance.Parameters.MaxVerPages = 4096;
            jetInstance.Parameters.NoInformationEvent = true;
            jetInstance.Parameters.WaypointLatency = 1;
            jetInstance.Parameters.MaxSessions = 512;
            jetInstance.Parameters.MaxOpenTables = 256;
           
            InitGrbit grbit = EsentVersion.SupportsWindows7Features
                                  ? Windows7Grbits.ReplayIgnoreLostLogs
                                  : InitGrbit.None;
            jetInstance.Parameters.Recovery = recovery; 
            jetInstance.Init(grbit);

            return jetInstance;
        }

        /// <summary>
        ///   Imports an Xml file memory model into the model server, only call when the database instances table is empty
        /// </summary>
        public void ImportIfcXml(string xbimDbName, string xmlFilename, ReportProgressDelegate progressHandler = null, bool keepOpen = false, bool cacheEntities = false)
        {
            CreateDatabase(xbimDbName);
            Open(xbimDbName, XbimDBAccess.Exclusive);
            var table = GetEntityTable();
            if (cacheEntities) this.CacheStart();
            try
            {
                using (var transaction = table.BeginLazyTransaction())
                {
                   
                    using (StreamReader xmlInStream = new StreamReader(xmlFilename,Encoding.GetEncoding("ISO-8859-9"))) //this is a work around to ensure latin character sets are read
                    {
                        using (XmlTextReader xmlTextReader = new XmlTextReader(xmlInStream))
                        {
                            XmlReaderSettings settings = new XmlReaderSettings();
                            settings.CheckCharacters = false; //has no impact
                            forwardReferences = new BlockingCollection<IfcForwardReference>();
                            XmlReader xmlReader = XmlReader.Create(xmlTextReader, settings);
                            settings.CheckCharacters = false;
                            IfcXmlReader reader = new IfcXmlReader();
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
                throw new Exception("Error importing IfcXml File " + xmlFilename, e);
            }
        }


        public bool Contains(IPersistIfcEntity instance)
        {
            return Contains(instance.EntityLabel);
        }

        public bool Contains(int entityLabel)
        {
            if (caching && this.read.ContainsKey(entityLabel)) //check if it is cached
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
        public long CountOf<TIfcType>() where TIfcType : IPersistIfcEntity
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
            HashSet<int> entityLabels = new HashSet<int>();
            IfcType ifcType = IfcMetaData.IfcType(theType);
            var entityTable = GetEntityTable();
            HashSet<short> typeIds = new HashSet<short>();
            //get all the type ids we are going to check for
            foreach (Type t in ifcType.NonAbstractSubTypes)
                typeIds.Add(IfcMetaData.IfcTypeId(t));
            try
            {

                XbimInstanceHandle ih;
                if (ifcType.IndexedClass)
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
            if (caching) //look in the createdNew cache and find the new ones only
            {
                foreach (var entity in createdNew.Where(m => m.Value.GetType() == theType))
                    entityLabels.Add(entity.Key);
                  
            }
            return entityLabels.Count;
        }

        public bool Any<TIfcType>() where TIfcType : IPersistIfcEntity
        {
            IfcType ifcType = IfcMetaData.IfcType(typeof(TIfcType));
            var entityTable = GetEntityTable();
            try
            {
                foreach (Type t in ifcType.NonAbstractSubTypes)
                {
                    short typeId = IfcMetaData.IfcTypeId(t);
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
                    if (caching) dbCount += createdNew.Count;
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
        internal IPersistIfcEntity CreateNew(Type t)
        {
            if (!caching)
                throw new XbimException("XbimModel.BeginTransaction must be called before editing a model");
            XbimEntityCursor cursor = _model.GetTransactingCursor();
            XbimInstanceHandle h = cursor.AddEntity(t);
            IPersistIfcEntity entity = (IPersistIfcEntity)Activator.CreateInstance(t);
            entity.Bind(_model, h.EntityLabel,true); //bind it, the object is new and empty so it is activated
            entity= this.read.GetOrAdd(h.EntityLabel, entity);
            modified.TryAdd(h.EntityLabel, entity);
            createdNew.TryAdd(h.EntityLabel, entity);
           
            return entity;
        }

        /// <summary>
        /// Creates a new instance, this is not a reversable action, and the instance is not cached
        /// It is for performance in import and export routines and should not be used in normal code
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal IPersistIfcEntity CreateNew(Type type, int label)
        {
            
            IPersistIfcEntity entity = (IPersistIfcEntity)Activator.CreateInstance(type);
            entity.Bind(_model, label, true); //bind it, the object is new and empty so it is activated
            return entity;
        }
     

        internal void AddForwardReference(IfcForwardReference forwardReference)
        {
            forwardReferences.Add(forwardReference);
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
            Type reqType = typeof(TIfcType);
            IfcType ifcType = IfcMetaData.IfcType(reqType);
            var entityTable = GetEntityTable();
            try
            {
                foreach (Type t in ifcType.NonAbstractSubTypes)
                {
                    short typeId = IfcMetaData.IfcTypeId(t);
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
        /// <returns></returns>
        public IPersistIfcEntity GetInstance(int label, bool loadProperties = false, bool unCached = false)
        {
           
            IPersistIfcEntity entity;
            if (caching && this.read.TryGetValue(label, out entity))
                return entity;
            else
                return GetInstanceFromStore(label, loadProperties, unCached);
        }


        /// <summary>
        /// Looks for this instance in the cache and returns it, if not found it creates a new instance and adds it to the cache
        /// </summary>
        /// <param name="label">Entity label to create</param>
        /// <param name="type">If not null creates an instance of this type, else creates an unknown Ifc Type</param>
        /// <param name="properties">if not null populates all properties of the instance</param>
        /// <returns></returns>
        public IPersistIfcEntity GetOrCreateInstanceFromCache(int label, Type type, byte[] properties)
        {
            Debug.Assert(caching); //must be caching to call this
           
            IPersistIfcEntity entity;
            if (!this.read.TryGetValue(label, out entity))
            {
                if (type.IsAbstract)
                {
                    XbimModel.Logger.ErrorFormat("Illegal Entity in the model #{0}, Type {1} is defined as Abstract and cannot be created", label, type.Name);
                    return null;
                }
                entity = (IPersistIfcEntity)Activator.CreateInstance(type);
                entity.Bind(_model, label, false); //bind it, the object is new and empty so not activated
                entity = read.GetOrAdd(label, entity); //might have been done by another
                lock (entity)
                {
                    if (!entity.Activated)
                    {
                        entity.Bind(_model, label,true); //stop recursive activation
                        entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false, true);
                    }
                }
            }
            return entity;
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
        private IPersistIfcEntity GetInstanceFromStore(int entityLabel, bool loadProperties = false, bool unCached = false)
        {
            var entityTable = GetEntityTable();
            try
            {
                using (var transaction = entityTable.BeginReadOnlyTransaction())
                {
                    
                    if (entityTable.TrySeekEntityLabel(entityLabel))
                    {
                        short currentIfcTypeId = entityTable.GetIfcType();
                        if (currentIfcTypeId == 0) // this should never happen (there's a test for it, but old xbim files might be incorrectly identified)
                            return null;
                        IPersistIfcEntity entity = (IPersistIfcEntity)Activator.CreateInstance(IfcMetaData.GetType(currentIfcTypeId));
                        if (loadProperties)
                        {
                            byte[] properties = entityTable.GetProperties();
                            entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), unCached);
                            entity.Bind(_model, entityLabel, true); //the attributes of this entity have been loaded 
                        }
                        else
                            entity.Bind(_model, entityLabel, false); //the attributes of this entity have not been loaded yet
                        if (caching && !unCached)
                            entity = this.read.GetOrAdd(entityLabel, entity);
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
        private IEnumerable<TIfcType> OfTypeUnindexed<TIfcType>(IfcType ifcType, bool activate = false) where TIfcType : IPersistIfcEntity
        {
            HashSet<int> entityLabels = new HashSet<int>();
            var entityTable = GetEntityTable();
            try
            {
                //get all the type ids we are going to check for
                HashSet<short> typeIds = new HashSet<short>();
                foreach (Type t in ifcType.NonAbstractSubTypes)
                    typeIds.Add(IfcMetaData.IfcTypeId(t));
                using (var transaction = entityTable.BeginReadOnlyTransaction())
                {
                    entityTable.MoveBeforeFirst();
                    while (entityTable.TryMoveNext())
                    {
                        XbimInstanceHandle ih = entityTable.GetInstanceHandle();
                        if (typeIds.Contains(ih.EntityTypeId))
                        {
                            IPersistIfcEntity entity;
                            if (caching && this.read.TryGetValue(ih.EntityLabel, out entity))
                            {
                                if (activate && !entity.Activated) //activate if required and not already done
                                {
                                    byte[] properties = entityTable.GetProperties();
                                    entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false);
                                    entity.Bind(_model, ih.EntityLabel,true); // the attributes of this entity have been loaded 
                                }
                                entityLabels.Add(entity.EntityLabel);
                                yield return (TIfcType)entity;
                            }
                            else
                            {
                                entity = (IPersistIfcEntity)Activator.CreateInstance(ih.EntityType);
                                if (activate)
                                {
                                    byte[] properties = entityTable.GetProperties();
                                    entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false);
                                    entity.Bind(_model, ih.EntityLabel,true); //the attributes of this entity have been loaded 
                                }
                                else
                                    entity.Bind(_model, ih.EntityLabel,false); //the attributes of this entity have not been loaded yet

                                if (caching) entity = this.read.GetOrAdd(ih.EntityLabel, entity);
                                entityLabels.Add(entity.EntityLabel);
                                yield return (TIfcType)entity;
                            }

                        }
                    }
                }
                if (caching) //look in the modified cache and find the new ones only
                {
                    foreach (var item in createdNew.Where(e => e.Value is TIfcType))//.ToList()) //force the iteration to avoid concurrency clashes
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
        /// <param name="overrideType">if specified this parameter overrides the ifcType used internally (but not TIfcType) for filtering purposes</param>
        /// <returns></returns>
        public IEnumerable<TIfcType> OfType<TIfcType>(bool activate = false, int? indexKey = null, IfcType overrideType = null) where TIfcType:IPersistIfcEntity 
        {
            //srl this needs to be removed, but preserves compatibility with old databases, the -1 should not be used in future
            int indexKeyAsInt;
            if (indexKey.HasValue) indexKeyAsInt = (int)indexKey.Value; //this is lossy and needs to be fixed if we get large databases
            else indexKeyAsInt = -1;
            IfcType SearchingIfcType;
            if (overrideType != null)
                SearchingIfcType = overrideType;
            else
                SearchingIfcType = IfcMetaData.IfcType(typeof(TIfcType));
            
            // when searching for Interface types SearchingIfcType is null
            //
            IEnumerable<Type> TypesToSearch;
            if (SearchingIfcType == null)
            {
                // not found in metadata, it's probably an interface.
                TypesToSearch = IfcMetaData.TypesImplementing(typeof(TIfcType));
            }
            else
            {
                TypesToSearch = SearchingIfcType.NonAbstractSubTypes;
            }

            if (SearchingIfcType == null || SearchingIfcType.IndexedClass)
            {
                //Set the IndexedClass Attribute of this class to ensure that seeking by index will work, this is a optimisation
                // Trying to look a class up by index that is not declared as indexeable
                HashSet<int> entityLabels = new HashSet<int>();
                var entityTable = GetEntityTable();
                try
                {
                    using (var transaction = entityTable.BeginReadOnlyTransaction())
                    {
                        foreach (Type t in TypesToSearch)
                        {
                            short typeId = IfcMetaData.IfcTypeId(t);
                            XbimInstanceHandle ih;
                            if (entityTable.TrySeekEntityType(typeId, out ih, indexKeyAsInt) && entityTable.TrySeekEntityLabel(ih.EntityLabel)) //we have the first instance
                            {
                                do
                                {
                                    IPersistIfcEntity entity;
                                    if (caching && this.read.TryGetValue(ih.EntityLabel, out entity))
                                    {
                                        if (activate && !entity.Activated) //activate if required and not already done
                                        {
                                            byte[] properties = entityTable.GetProperties();
                                            entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false);
                                            entity.Bind(_model, ih.EntityLabel,true); //the attributes of this entity have been loaded yet
                                        }
                                        entityLabels.Add(entity.EntityLabel);
                                        yield return (TIfcType)entity;
                                    }
                                    else
                                    {
                                        entity = (IPersistIfcEntity)Activator.CreateInstance(ih.EntityType);
                                        if (activate)
                                        {
                                            byte[] properties = entityTable.GetProperties();
                                            entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(properties)), false);
                                            entity.Bind(_model, ih.EntityLabel,true); //the attributes of this entity have been loaded yet
                                        }
                                        else
                                            entity.Bind(_model, ih.EntityLabel,false); // the attributes of this entity have not been loaded yet

                                        if (caching) entity = this.read.GetOrAdd(ih.EntityLabel, entity);
                                        entityLabels.Add(entity.EntityLabel);
                                        yield return (TIfcType)entity;
                                    }
                                } while (entityTable.TryMoveNextEntityType(out ih) && entityTable.TrySeekEntityLabel(ih.EntityLabel));
                            }
                        }
                    }
                    // todo: bonghi: check with SRL, I'm failing to understand the following behaviour when using indexkey.
                    // 
                    if (caching) //look in the createnew cache and find the new ones only
                    {
                        foreach (var item in createdNew.Where(e => e.Value is TIfcType))//.ToList())
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
                                if (SearchingIfcType != null && SearchingIfcType.GetIndexedValues(item.Value).Contains(indexKey.Value)) // get all types that match the index key
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
                foreach (var item in OfTypeUnindexed<TIfcType>(SearchingIfcType, activate))
                    yield return item;
            }
        }

        public void ImportXbim(string importFrom, ReportProgressDelegate progressHandler = null)
        {
            throw new NotImplementedException();  
        }

        public void Activate(IPersistIfcEntity entity)
        {
            byte[] bytes = GetEntityBinaryData(entity);
            if (bytes != null)
                entity.ReadEntityProperties(this, new BinaryReader(new MemoryStream(bytes)));
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        ~IfcPersistedInstanceCache()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                string dbName = this.DatabaseName;
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    Close();
 
                }
                try
                {

                    lock (openInstances)
                    {
                        openInstances.Remove(this);
                        int refCount = openInstances.Count(c => c.JetInstance == this.JetInstance);
                        if (refCount == 0) //only terminate if we have no more references
                            _jetInstance.Term();
                    }
                    if (Directory.Exists(_systemPath))
                        Directory.Delete(_systemPath, true);
                }
                catch (Exception) //just in case we cannot delete
                {

                }
                finally
                {
                    _jetInstance = null;
                }
            }
            disposed = true;
        }


        /// <summary>
        /// Gets the entities propertyData on binary stream
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal byte[] GetEntityBinaryData(IPersistIfcEntity entity)
        {
            var entityTable = GetEntityTable();
            try
            {
                using (var transaction = entityTable.BeginReadOnlyTransaction())
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

        public void SaveAs(XbimStorageType _storageType, string _storageFileName, ReportProgressDelegate progress = null, IDictionary<int, int> map = null)
        {
            switch (_storageType)
            {
                case XbimStorageType.IFCXML:
                    SaveAsIfcXml(_storageFileName);
                    break;
                case XbimStorageType.IFC:
                    SaveAsIfc(_storageFileName,map);
                    break;
                case XbimStorageType.IFCZIP:
                    SaveAsIfcZip(_storageFileName);
                    break;
                case XbimStorageType.XBIM:
                    Debug.Assert(false, "Incorrect call, see XbimModel.SaveAs");
                    break;
                case XbimStorageType.INVALID:
                default:
                    break;
            }

        }

        private void SaveAsIfcZip(string storageFileName)
        {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(storageFileName))) //make sure we have an extension
                storageFileName = Path.ChangeExtension(storageFileName, "IfcZip");
            string fileBody = Path.ChangeExtension(Path.GetFileName(storageFileName),"ifc");
            var entityTable = GetEntityTable();
            FileStream fs = null;
            ZipOutputStream zipStream = null;
            try
            {
                fs = new FileStream(storageFileName, FileMode.Create, FileAccess.Write);
                zipStream = new ZipOutputStream(fs);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
                ZipEntry newEntry = new ZipEntry(fileBody);
                newEntry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(newEntry);
                using (var transaction = entityTable.BeginReadOnlyTransaction())
                {
                    using (TextWriter tw = new StreamWriter(zipStream))
                    {
                        Part21FileWriter p21 = new Part21FileWriter();
                        p21.Write(_model, tw);
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
                using (var transaction = entityTable.BeginReadOnlyTransaction())
                {
                    using (TextWriter tw = new StreamWriter(storageFileName))
                    {
                        Part21FileWriter p21 = new Part21FileWriter();
                        p21.Write(_model, tw,map);
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
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter xmlWriter = XmlWriter.Create(xmlOutStream, settings))
                {
                    IfcXmlWriter writer = new IfcXmlWriter();
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



        public void Delete_Reversable(IPersistIfcEntity instance)
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


        private static MemberExpression GetIndexablePropertyOnLeft<T>(Expression leftSide)
        {
            MemberExpression mex = leftSide as MemberExpression;
            if (leftSide.NodeType == ExpressionType.Call)
            {
                MethodCallExpression call = leftSide as MethodCallExpression;
                if (call.Method.Name == "CompareString")
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
                MethodCallExpression call = leftSide as MethodCallExpression;
                if (call.Method.Name == "CompareString")
                {
                    LambdaExpression evalRight = Expression.Lambda(call.Arguments[1], null);
                    //Compile it, invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
                }
            }
            //rightside is where we get our hash...
            switch (rightSide.NodeType)
            {
                //shortcut constants, dont eval, will be faster
                case ExpressionType.Constant:
                    ConstantExpression constExp
                        = (ConstantExpression)rightSide;
                    return (constExp.Value);

                //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
                default:
                    //Lambdas can be created from expressions... yay
                    LambdaExpression evalRight = Expression.Lambda(rightSide, null);
                    //Compile and invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
            }
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistIfcEntity
        {
            bool indexFound = false;
            Type type = typeof(T);
            IfcType ifcType = IfcMetaData.IfcType(type);
           
            Func<T, bool> predicate = expr.Compile();
            if (ifcType.HasIndexedAttribute) //we can use a secondary index to look up
            {
                //our indexes work from the hash values of that which is indexed, regardless of type
                object hashRight = null;

                //indexes only work on equality expressions here
                //this  matches "Property" = "Value"
                if (expr.Body.NodeType == ExpressionType.Equal)
                {
                    //Equality is a binary expression
                    BinaryExpression binExp = (BinaryExpression)expr.Body;
                    //Get some aliases for either side
                    Expression leftSide = binExp.Left;
                    Expression rightSide = binExp.Right;

                    hashRight = GetRight(leftSide, rightSide);

                    //if we were able to create a hash from the right side (likely)
                    MemberExpression returnedEx = GetIndexablePropertyOnLeft<T>(leftSide);
                    if (returnedEx != null)
                    {
                        //cast to MemberExpression - it allows us to get the property
                        MemberExpression propExp = returnedEx;
                        
                        if (ifcType.IndexedProperties.Contains(propExp.Member)) //we have a primary key match
                        {
                            IPersistIfcEntity entity = hashRight as IPersistIfcEntity;
                            if (entity != null)
                            {
                                indexFound = true;
                                foreach (var item in OfType<T>(true, entity.EntityLabel))
                                {
                                    if (predicate(item))
                                        yield return item;
                                }
                            }
                        }
                    }
                }
                else if (expr.Body.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression callExp = (MethodCallExpression)expr.Body;
                    if (callExp.Method.Name == "Contains")
                    {
                        Expression keyExpr = callExp.Arguments[0];
                        if (keyExpr.NodeType == ExpressionType.Constant)
                        {
                            ConstantExpression constExp = (ConstantExpression)keyExpr;
                            object key = constExp.Value;
                            if (callExp.Object.NodeType == ExpressionType.MemberAccess)
                            {
                                MemberExpression memExp = (MemberExpression)callExp.Object;
                                PropertyInfo pInfo = (PropertyInfo)(memExp.Member);
                                if (ifcType.IndexedProperties.Contains(pInfo, comparePropInfo)) //we have a primary key match
                                {
                                    IPersistIfcEntity entity = key as IPersistIfcEntity;
                                    if (entity != null)
                                    {
                                        indexFound = true;
                                        foreach (var item in OfType<T>(true, entity.EntityLabel))
                                        {
                                            if (predicate(item))
                                                yield return item;
                                        }
                                    }
                                }
                            }
                        }
                    }
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
            XbimGeometryCursor geomTable = GetGeometryTable();
            try
            {
                using (var transaction = geomTable.BeginReadOnlyTransaction())
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
            XbimGeometryCursor geometryTable = GetGeometryTable();
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

        internal T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, XbimReadWriteTransaction txn, bool includeInverses) where T : IPersistIfcEntity
        {
            //check if the transaction needs pulsing
            
            XbimInstanceHandle toCopyHandle=toCopy.GetHandle();
          
            XbimInstanceHandle copyHandle;
            if (mappings.TryGetValue(toCopyHandle, out copyHandle))
            {
                var v = this.GetInstance(copyHandle);
                Debug.Assert(v!=null);
                return (T)v;
            }
            txn.Pulse();
            IfcType ifcType = IfcMetaData.IfcType(toCopy);
            int copyLabel = toCopy.EntityLabel;
            copyHandle = InsertNew(ifcType.Type);
            mappings.Add(toCopyHandle, copyHandle);
            if (typeof(IfcCartesianPoint) == ifcType.Type || typeof(IfcDirection) == ifcType.Type)//special cases for cartesian point and direction for efficiency
            {
                IPersistIfcEntity v = (IPersistIfcEntity)Activator.CreateInstance(ifcType.Type, new object[] { toCopy });      
                v.Bind(_model, copyHandle.EntityLabel,true);
                v.Activate(true);
                read.TryAdd(copyHandle.EntityLabel, v);
                createdNew.TryAdd(copyHandle.EntityLabel, v);
                return (T)v;
            }
            else
            {        
                IPersistIfcEntity theCopy = (IPersistIfcEntity)Activator.CreateInstance(copyHandle.EntityType);
                theCopy.Bind(_model, copyHandle.EntityLabel,true);
                read.TryAdd(copyHandle.EntityLabel, theCopy);
                createdNew.TryAdd(copyHandle.EntityLabel, theCopy);
                IfcRoot rt = theCopy as IfcRoot;
                IEnumerable<IfcMetaProperty> props = ifcType.IfcProperties.Values.Where(p => !p.IfcAttribute.IsDerivedOverride);
                if (includeInverses)
                    props = props.Union(ifcType.IfcInverses);
                if (rt != null) rt.OwnerHistory = _model.OwnerHistoryAddObject;
                foreach (IfcMetaProperty prop in props)
                {
                    if (rt != null && prop.PropertyInfo.Name == "OwnerHistory") //don't add the owner history in as this will be changed later
                        continue;
                    object value = prop.PropertyInfo.GetValue(toCopy, null);
                    if (value != null)
                    {
                        bool isInverse = (prop.IfcAttribute.Order == -1); //don't try and set the values for inverses
                        Type theType = value.GetType();
                        //if it is an express type or a value type, set the value
                        if (theType.IsValueType || typeof(ExpressType).IsAssignableFrom(theType))
                        {
                            prop.PropertyInfo.SetValue(theCopy, value, null);
                        }
                        //else 
                        else if (!isInverse && typeof(IPersistIfcEntity).IsAssignableFrom(theType))
                        {
                            prop.PropertyInfo.SetValue(theCopy, InsertCopy((IPersistIfcEntity)value, mappings, txn, includeInverses), null);
                        }
                        else if (!isInverse && typeof(ExpressEnumerable).IsAssignableFrom(theType))
                        {
                            Type itemType = theType.GetItemTypeFromGenericType();

                            ExpressEnumerable copyColl;
                            if (!theType.IsGenericType) //we have a class that inherits from a generic type
                                copyColl = (ExpressEnumerable)Activator.CreateInstance(theType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { theCopy }, null);
                            else
                            {
                                Type genericType = theType.GetGenericTypeDefinition();
                                Type gt = genericType.MakeGenericType(new Type[] { itemType });
                                copyColl = (ExpressEnumerable)Activator.CreateInstance(gt, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { theCopy }, null);
                            }
                            prop.PropertyInfo.SetValue(theCopy, copyColl, null);
                            foreach (var item in (ExpressEnumerable)value)
                            {
                                Type actualItemType = item.GetType();
                                if (actualItemType.IsValueType || typeof(ExpressType).IsAssignableFrom(actualItemType))
                                    copyColl.Add(item);
                                else if (typeof(IPersistIfcEntity).IsAssignableFrom(actualItemType))
                                {
                                    var cpy = InsertCopy((IPersistIfcEntity)item, mappings, txn, includeInverses);
                                    copyColl.Add(cpy);
                                }
                                else
                                    throw new XbimException(string.Format("Unexpected collection item type ({0}) found", itemType.Name));
                            }
                        }
                        else if (isInverse && value is IEnumerable<IPersistIfcEntity>) //just an enumeration of IPersistIfcEntity
                        {
                            foreach (var ent in (IEnumerable<IPersistIfcEntity>)value)
                                InsertCopy(ent, mappings, txn, includeInverses);
                        }
                        else if (isInverse && value is IPersistIfcEntity) //it is an inverse and has a single value
                            InsertCopy((IPersistIfcEntity)value, mappings, txn, includeInverses);
                        else
                            throw new XbimException(string.Format("Unexpected item type ({0})  found", theType.Name));
                    }
                }
              //  if (rt != null) rt.OwnerHistory = this.OwnerHistoryAddObject;
                return (T)theCopy;
            }
        }

        private IPersistIfcEntity GetInstance(XbimInstanceHandle map)
        {
            return GetInstance(map.EntityLabel);
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
        internal void AddModified(IPersistIfcEntity entity)
        {
            //IPersistIfcEntity editing;
            //if (modified.TryGetValue(entity.EntityLabel, out editing)) //it  already exists as edited
            //{
            //    if (!System.Object.ReferenceEquals(editing, entity)) //it is not the same object reference
            //        throw new XbimException("An attempt to edit a duplicate reference for #" + entity.EntityLabel + " error has occurred");
            //}
            //else
            modified.TryAdd(entity.EntityLabel, entity);
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
            if(!caching) read.Clear();
            modified.Clear();
            createdNew.Clear();
            previousCaching = caching;
            caching = true;
        }
        /// <summary>
        /// Clears any cached objects and terminates further caching
        /// </summary>
        internal void  EndCaching()
        {
            if(!previousCaching) read.Clear();
            modified.Clear();
            createdNew.Clear();
            caching = previousCaching;
        }

        /// <summary>
        /// Writes the content of the modified cache to the table, assumes a transaction is in scope, modified and createnew caches are cleared
        /// </summary>
        internal void Write(XbimEntityCursor entityTable)
        {
            foreach (var entity in modified.Values)
            {
                entityTable.UpdateEntity(entity);
            }
            modified.Clear();
            createdNew.Clear();
        }

        public bool HasDatabaseInstance
        {
            get
            {
                return _jetInstance != null;
            }
        }

        internal IEnumerable<IPersistIfcEntity> Modified()
        {
            return modified.Values;
        }
     
        internal XbimGeometryHandleCollection GetGeometryHandles(XbimGeometryType geomType=XbimGeometryType.TriangulatedMesh, XbimGeometrySort sortOrder=XbimGeometrySort.OrderByIfcSurfaceStyleThenIfcType)
        {
            //Get a cached or open a new Table
            XbimGeometryCursor geometryTable = GetGeometryTable();
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
            XbimGeometryCursor geometryTable = GetGeometryTable();
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
            XbimGeometryCursor geometryTable = GetGeometryTable();
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
            XbimGeometryCursor geometryTable = GetGeometryTable();
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

        internal IEnumerable<IPersistIfcEntity> OfType(string StringType, bool activate)
        {

            IfcType ot = IfcMetaData.IfcType(StringType.ToUpper());
            if (ot == null)
            {
                // it could be that we're searching for an interface
                //
                var ImplementingTypes = IfcMetaData.TypesImplementing(StringType);
                foreach (var ImplementingType in ImplementingTypes)
                {
                    foreach (var item in OfType<IPersistIfcEntity>(activate: activate, overrideType: ImplementingType))
                        yield return item;
                }
            }
            else
            {
                foreach (var item in OfType<IPersistIfcEntity>(activate: activate, overrideType: ot))
                    yield return item;

            }
        }
        /// <summary>
        /// Starts a read cache
        /// </summary>
        internal void CacheStart()
        {
            caching = true;
        }
        /// <summary>
        /// Clears a read cache, do not call when a transaction is active
        /// </summary>
        internal void CacheClear()
        {
            Debug.Assert(modified.Count == 0 && createdNew.Count==0);
            read.Clear();
        }
        /// <summary>
        /// Clears a read cache, and ends further caching, do not call when a transaction is active
        /// </summary>
        internal void CacheStop()
        {
            Debug.Assert(modified.Count == 0 && createdNew.Count == 0);
            read.Clear();
            caching = false;
        }

        internal bool IsCaching
        {
            get
            {
                return caching;
            }        
        }

        public XbimModel Model 
        {
            get
            {
                return _model;
            }
        }

        internal XbimGeometryData GetGeometryData(int geomLabel)
        {
            //Get a cached or open a new Table
            XbimGeometryCursor geometryTable = GetGeometryTable();
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
            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null != this._geometryTables[i] && this._geometryTables[i] is XbimShapeGeometryCursor)
                    {
                        var table = this._geometryTables[i];
                        this._geometryTables[i] = null;
                        return (XbimShapeGeometryCursor)table;
                    }
                }
            }
            OpenDatabaseGrbit openMode = AttachedDatabase();
            return new XbimShapeGeometryCursor(this._model, _databaseName, openMode);
        }

        internal bool deleteJetTable(string name)
        {
            if (!HasTable(name)) 
                return true;
            try
            {
                Api.JetDeleteTable(_session, _databaseId, name);
            }
            catch 
            {
                return false;
            }
            return true;
        }

        internal bool DeleteGeometry()
        {
            var ret = deleteJetTable(XbimShapeInstanceCursor.InstanceTableName);
            if (!deleteJetTable(XbimGeometryCursor.GeometryTableName))
                ret = false;
            if (!deleteJetTable(XbimShapeGeometryCursor.GeometryTableName))
                ret = false;
            return ret;
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
            JET_TABLEID t;
            var has = Api.TryOpenTable(this._session, this._databaseId, name, OpenTableGrbit.ReadOnly, out t);
            if (has)
                Api.JetCloseTable(this._session, t);
            return has;
        }

        internal XbimShapeInstanceCursor GetShapeInstanceTable()
        {
            Debug.Assert(!string.IsNullOrEmpty(_databaseName));
            lock (this._lockObject)
            {
                for (int i = 0; i < this._geometryTables.Length; ++i)
                {
                    if (null != this._geometryTables[i] && this._geometryTables[i] is XbimShapeInstanceCursor)
                    {
                        var table = this._geometryTables[i];
                        this._geometryTables[i] = null;
                        return (XbimShapeInstanceCursor)table;
                    }
                }
            }
            OpenDatabaseGrbit openMode = AttachedDatabase();
            return new XbimShapeInstanceCursor(this._model, _databaseName, openMode);
        }
    }
}


