using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Common.Geometry;
using Xbim.Common.Logging;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using XbimGeometry.Interfaces;

namespace Xbim.IO.Esent
{
    /// <summary>
    /// General Model class for memory based model suport
    /// </summary>

    public class EsentModel: IModel, IFederatedModel, IDisposable
    {
        #region Fields

        #region Logging Fields

        internal readonly static ILogger Logger = LoggerFactory.GetLogger();

        #endregion

        #region Model state fields

        protected PersistedEntityInstanceCache InstanceCache;
        internal PersistedEntityInstanceCache Cache
        {
            get { return InstanceCache; }
        }

        private bool _disposed;

        private XbimEntityCursor _editTransactionEntityCursor;
        private bool _deleteOnClose;
        

        private int _codePageOverrideForStepFiles = -1;

        /// <summary>
        /// An identifier that an application can use to identify this model uniquely
        /// </summary>
        public short UserDefinedId { get; set; }

        #endregion

        //Object that manages geometry conversion etc
        private string _importFilePath;

        private IEntityFactory _factory;
        public IEntityFactory Factory {get { return _factory; }}

        public ExpressMetaData Metadata { get; private set; }
        #endregion

        /// <summary>
        /// Model wide factors, precision etc
        /// </summary>
        public IModelFactors ModelFactors { get; protected set; }


        public EsentModel(IEntityFactory factory)
        {
            Init(factory);
        }

        /// <summary>
        /// Only inherited models can call parameterless constructor and it is their responsibility to 
        /// call Init() as the very first thing.
        /// </summary>
        protected EsentModel()
        {
        }

        protected void Init(IEntityFactory factory)
        {
            _factory = factory;
            InstanceCache = new PersistedEntityInstanceCache(this, factory);
            InstancesLocal = new XbimInstanceCollection(this);
            var r = new Random();
            UserDefinedId = (short)r.Next(short.MaxValue); // initialise value at random to reduce chance of duplicates
            Metadata = ExpressMetaData.GetMetadata(factory.GetType().Module);
        }

        public string DatabaseName
        {
            get { return InstanceCache.DatabaseName; }
        }




        //sets or gets the Geometry Manager for this model
        public IGeometryManager GeometryManager { get; set; }

        static public int ModelOpenCount
        {
            get
            {
                return PersistedEntityInstanceCache.ModelOpenCount;
            }
        }

        /// <summary>
        /// Some applications do not comply with the standard and used the Windows codepage for text. This property gives the possibility to override the character encoding when reading ifc.
        /// default value = -1 - by standart http://www.buildingsmart-tech.org/implementation/get-started/string-encoding/string-encoding-decoding-summary
        /// </summary>
        /// <example>
        /// model.CodePageOverride = Encoding.Default.WindowsCodePage;
        /// </example>
        public int CodePageOverride
        {
           get { return _codePageOverrideForStepFiles; }
           set { _codePageOverrideForStepFiles = value; }
        }

        public XbimInstanceCollection InstancesLocal { get; private set; }

        /// <summary>
        /// Returns a collection of all instances in the model and all federated instances 
        /// </summary>
        public IEntityCollection Instances
        {
            get
            {
                return new XbimFederatedModelInstances(this);
            }
        }

        /// <summary>
        /// This event is fired every time new entity is created.
        /// </summary>
        public event NewEntityHandler EntityNew;

        /// <summary>
        /// This event is fired every time any entity is modified. If your model is not
        /// transactional it might not be called at all as the central point for all
        /// modifications is a transaction.
        /// </summary>
        public event ModifiedEntityHandler EntityModified;

        /// <summary>
        /// This event is fired every time when entity gets deleted from model.
        /// </summary>
        public event DeletedEntityHandler EntityDeleted;

        internal void HandleEntityChange(ChangeType changeType, IPersistEntity entity)
        {
            switch (changeType)
            {
                case ChangeType.New:
                    if (EntityNew != null)
                        EntityNew(entity);
                    break;
                case ChangeType.Deleted:
                    if (EntityDeleted != null)
                        EntityDeleted(entity);
                    break;
                case ChangeType.Modified:
                    if (EntityModified != null)
                        EntityModified(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("changeType", changeType, null);
            }
        }

        
        /// <summary>
        /// Starts a transaction to allow bulk updates on the geometry table, FreeGeometry Table should be called when no longer required
        /// </summary>
        /// <returns></returns>
        public XbimGeometryCursor GetGeometryTable()
        {
            return InstanceCache.GetGeometryTable();
        }

        /// <summary>
        /// Returns the table to the cache for reuse
        /// </summary>
        /// <param name="table"></param>
        public void FreeTable(XbimGeometryCursor table)
        {
            InstanceCache.FreeTable(table);
        }

        /// <summary>
        /// Returns the table to the cache for reuse
        /// </summary>
        /// <param name="table"></param>
        public void FreeTable(XbimEntityCursor table)
        {
            InstanceCache.FreeTable(table);
        }
        /// <summary>
        /// Returns the table to the cache for reuse
        /// </summary>
        /// <param name="table"></param>
        public void FreeTable(XbimShapeGeometryCursor table)
        {
            InstanceCache.FreeTable(table);
        }
        /// <summary>
        /// Returns the table to the cache for reuse
        /// </summary>
        /// <param name="table"></param>
        public void FreeTable(XbimShapeInstanceCursor table)
        {
            InstanceCache.FreeTable(table);
        }
        //Loads the property data of an entity, if it is not already loaded
        bool IModel.Activate(IPersistEntity entity, bool write)
        {
            if (write) //we want to activate for reading
            {
                //if (!Transaction.IsRollingBack)
                InstanceCache.AddModified(entity);
            }
            //else //we want to read so load from db if necessary
            {
                InstanceCache.Activate(entity);
            }
            return true;
        }

        #region Transaction support



      

        public XbimReadWriteTransaction BeginTransaction()
        {
            return BeginTransaction(null);
        }

        public bool IsTransacting
        {
            get
            {
                return _editTransactionEntityCursor != null;
            }
        }

        public XbimReadWriteTransaction BeginTransaction(string operationName)
        {
            if (_editTransactionEntityCursor != null) 
                throw new XbimException("Attempt to begin another transaction whilst one is already running");
            try
            {
                _editTransactionEntityCursor = InstanceCache.GetEntityTable();
                InstanceCache.BeginCaching();
                var txn = new XbimReadWriteTransaction(this, _editTransactionEntityCursor.BeginLazyTransaction(), operationName);
                CurrentTransaction = txn;
                return txn;
            }
            catch (Exception e)
            {

                throw new XbimException("Failed to create ReadWrite transaction", e);
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
            InstanceCache.ForEach(source, body);
        }


        #endregion

        #region IModel interface implementation

        /// <summary>
        /// Registers an entity for deletion
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public void Delete(IPersistEntity instance)
        {
            InstanceCache.Delete_Reversable(instance);
        }

        /// <summary>
        /// Returns an instance from the Model with the corresponding label but does not keep a cache of it
        /// This is a dangerous call as duplicate instances of the same object could happen
        /// Ony use when interating over the whole database for export etc
        /// The properties of the object are also loaded to improve performance
        /// If the instance is in the cache it is returned
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        internal IPersistEntity GetInstanceVolatile(int label)
        {
            return InstanceCache.GetInstance(label, true, true);
        }

        /// <summary>
        /// Returns the total number of Geometry objects in the model
        /// </summary>
        public virtual long GeometriesCount
        {
            get
            {
                return InstanceCache.GeometriesCount();
            }
        }

        /// <summary>
        /// Creates a new Model and populates with instances from the specified file, Ifc, IfcXML, IfcZip and Xbim are all supported.
        /// </summary>
        /// <param name="importFrom">Name of the file containing the instances to import</param>
        /// /// <param name="xbimDbName">Name of the xbim file that will be created. 
        /// If null the contents are loaded into memory and are not persistent
        /// </param>
        /// <param name="progDelegate"></param>
        /// <param name="keepOpen"></param>
        /// <param name="cacheEntities"></param>
        /// <returns></returns>
        public virtual bool CreateFrom(string importFrom, string xbimDbName = null, ReportProgressDelegate progDelegate = null, bool keepOpen = false, bool cacheEntities = false)
        {
            Close();
            _importFilePath = Path.GetFullPath(importFrom);
            if (!Directory.Exists(Path.GetDirectoryName(_importFilePath) ?? ""))
                throw new DirectoryNotFoundException(Path.GetDirectoryName(importFrom) + " directory was not found");
            if (!File.Exists(_importFilePath))
                throw new FileNotFoundException(_importFilePath + " file was not found");
            if (string.IsNullOrWhiteSpace(xbimDbName))
                xbimDbName = Path.ChangeExtension(importFrom, "xBIM");
            
            var toImportStorageType = StorageType(importFrom);
            switch (toImportStorageType)
            {
                case XbimStorageType.IFCXML:
                    InstanceCache.ImportIfcXml(xbimDbName, importFrom, progDelegate, keepOpen, cacheEntities);
                    break;
                case XbimStorageType.Step21:
                    InstanceCache.ImportStep(xbimDbName, importFrom, progDelegate, keepOpen, cacheEntities, _codePageOverrideForStepFiles);
                    break;
                case XbimStorageType.Step21Zip:
                    InstanceCache.ImportStepZip(xbimDbName, importFrom, progDelegate, keepOpen, cacheEntities, _codePageOverrideForStepFiles);
                    break;
                case XbimStorageType.XBIM:
                    InstanceCache.ImportXbim(importFrom, progDelegate);
                    break;
                default:
                    return false;
            }
            return true;
        }

        public virtual bool CreateFrom(Stream inputStream, XbimStorageType streamType, string xbimDbName, ReportProgressDelegate progDelegate = null, bool keepOpen = false, bool cacheEntities = false)
        {
            Close();

            switch (streamType)
            {
                case XbimStorageType.IFCXML:
                    Cache.ImportIfcXml(xbimDbName, inputStream, progDelegate, keepOpen, cacheEntities);
                    break;
                case XbimStorageType.Step21:
                    Cache.ImportStep(xbimDbName, inputStream, progDelegate, keepOpen, cacheEntities, _codePageOverrideForStepFiles);
                    break;
                case XbimStorageType.Step21Zip:
                    Cache.ImportStepZip(xbimDbName, inputStream, progDelegate, keepOpen, cacheEntities, _codePageOverrideForStepFiles);
                    break;
                default:
                    return false;
            }
            
            return true;
        }
        /// <summary>
        /// Creates an empty model using a temporary filename, the model will be deleted on close, unless SaveAs is called
        /// It will be returned open for read write operations
        /// </summary>
        /// <returns></returns>
        static public EsentModel CreateTemporaryModel(IEntityFactory factory)
        {
            
            var tmpFileName = Path.GetTempFileName();
            try
            {
                var model = new EsentModel(factory);
                model.CreateDatabase(tmpFileName);  
                model.Open(tmpFileName, XbimDBAccess.ReadWrite, true);
                model.Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults);
                model.Header.FileSchema.Schemas.AddRange(factory.SchemasIds);
                return model;
            }
            catch (Exception e)
            {

                throw new XbimException("Failed to create and open temporary xBIM file \'" + tmpFileName + "\'\n" + e.Message, e);
            }
           
        }

        protected void CreateDatabase(string tmpFileName)
        {
            InstanceCache.CreateDatabase(tmpFileName);
        }

        /// <summary>
        ///  Creates and opens a new Xbim Database
        /// </summary>
        /// <param name="factory">Entity factory to be used for deserialization</param>
        /// <param name="dbFileName">Name of the Xbim file</param>
        /// <param name="access"></param>
        /// <returns></returns>
        static public EsentModel CreateModel(IEntityFactory factory, string dbFileName, XbimDBAccess access = XbimDBAccess.ReadWrite)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Path.GetExtension(dbFileName)))
                    dbFileName += ".xBIM";
                var model = new EsentModel(factory);
                model.CreateDatabase(dbFileName); 
                model.Open(dbFileName, access);
                model.Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults) { FileName = {Name = dbFileName} };
                model.Header.FileSchema.Schemas.AddRange(factory.SchemasIds);
                return model;
            }
            catch (Exception e)
            {
                throw new XbimException("Failed to create and open xBIM file \'" + dbFileName + "\'\n" + e.Message, e);
            }
        }

        #endregion

        public byte[] GetEntityBinaryData(IInstantiableEntity entity)
        {
            if (entity.ActivationStatus != ActivationStatus.NotActivated) //we have it in memory but not written to store yet
            {
                var entityStream = new MemoryStream(4096);
                var entityWriter = new BinaryWriter(entityStream);
                entity.WriteEntity(entityWriter, Metadata);
                return entityStream.ToArray();
            }
            return InstanceCache.GetEntityBinaryData(entity);
        }

        public IStepFileHeader Header {  get; set; }

        #region Validation

        /// <summary>
        /// Validates all entities in the model
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="validateLevel"></param>
        /// <returns></returns>
        public int Validate(TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            return InstancesLocal.Sum(entity => Validate(entity as IInstantiableEntity, tw, validateLevel));
        }

        public int Validate(IEnumerable<IInstantiableEntity> entities, TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            return entities.Sum(entity => Validate(entity, tw, validateLevel));
        }

        public int Validate(IPersistEntity ent, TextWriter tw, ValidationFlags validateLevel = ValidationFlags.Properties)
        {
            var itw = new IndentedTextWriter(tw);
            if (validateLevel == ValidationFlags.None) return 0; //nothing to do
            var expressType = Metadata.ExpressType(ent);
            var notIndented = true;
            var errors = 0;
            if (validateLevel == ValidationFlags.Properties || validateLevel == ValidationFlags.All)
            {
                foreach (var ifcProp in expressType.Properties.Values)
                {
                    var err = GetSchemaError(ent as IInstantiableEntity, ifcProp);
                    if (string.IsNullOrEmpty(err)) continue;
                    if (notIndented)
                    {
                        itw.WriteLine("#{0} - {1}", ent.EntityLabel, expressType.Type.Name);
                        itw.Indent++;
                        notIndented = false;
                    }
                    itw.WriteLine(err.Trim('\n'));
                    errors++;
                }
            }
            if (validateLevel == ValidationFlags.Inverses || validateLevel == ValidationFlags.All)
            {
                foreach (var ifcInv in expressType.Inverses)
                {
                    var err = GetSchemaError(ent as IInstantiableEntity, ifcInv);
                    if (string.IsNullOrEmpty(err)) continue;
                    if (notIndented)
                    {
                        itw.WriteLine("#{0} - {1}", ent.EntityLabel, expressType.Type.Name);
                        itw.Indent++;
                        notIndented = false;
                    }
                    itw.WriteLine(err.Trim('\n'));
                    errors++;
                }
            }

            string str = ent.WhereRule();
            if (!string.IsNullOrEmpty(str))
            {
                if (notIndented)
                {
                    itw.WriteLine("#{0} - {1}", ent.EntityLabel, expressType.Type.Name);
                    itw.Indent++;
                    notIndented = false;
                }
                itw.WriteLine(str.Trim('\n'));
                errors++;
            }
            if (!notIndented) itw.Indent--;
            return errors;
        }

        private static string GetSchemaError(IPersist instance, ExpressMetaProperty prop)
        {
            //IfcAttribute ifcAttr, object instance, object propVal, string propName

            var entAttr = prop.EntityAttribute;
            var propVal = prop.PropertyInfo.GetValue(instance, null);
            var propName = prop.PropertyInfo.Name;

            

            if (entAttr.State == EntityAttributeState.Mandatory && propVal == null)
                return string.Format("{0}.{1} is not optional", instance.GetType().Name, propName);
            if (entAttr.State == EntityAttributeState.Optional && propVal == null)
                //if it is null and optional then it is ok
                return null;

            //if it is IPersist (either IPersistEntity or IExpressValueType) check WhereRule() defined in schema. 
            var persist = propVal as IPersist;
            if (persist != null)
            {
                var err = persist.WhereRule();
                if (!string.IsNullOrEmpty(err)) return err;
            }

            if (entAttr.EntityType != EntityAttributeType.Set && entAttr.EntityType != EntityAttributeType.List &&
                entAttr.EntityType != EntityAttributeType.ListUnique && entAttr.EntityType != EntityAttributeType.Array && 
                entAttr.EntityType != EntityAttributeType.ArrayUnique && entAttr.EntityType != EntityAttributeType.Bag) 
                return null;

            if (entAttr.MinCardinality < 1 && entAttr.MaxCardinality < 0) //we don't care how many so don't check
                return null;
            var coll = propVal as ICollection;
            var count = 0;
            if (coll != null)
                count = coll.Count;
            else
            {
                var en = (IEnumerable)propVal;

                // ReSharper disable once UnusedVariable
                foreach (var item in en)
                {
                    count++;
                    if (count >= entAttr.MinCardinality && entAttr.MaxCardinality == -1)
                        //we have met the requirements
                        break;
                    if (entAttr.MaxCardinality > -1 && count > entAttr.MaxCardinality) //we are out of bounds
                        break;
                }
            }

            if (count < entAttr.MinCardinality)
            {
                return string.Format("{0}.{1} must have at least {2} item(s). It has {3}", instance.GetType().Name,
                    propName, entAttr.MinCardinality, count);
            }
            if (entAttr.MaxCardinality > -1 && count > entAttr.MaxCardinality)
            {
                return string.Format("{0}.{1} must have no more than {2} item(s). It has at least {3}",
                    instance.GetType().Name, propName, entAttr.MaxCardinality, count);
            }
            return null;
        }

        #endregion



        #region General Model operations

        /// <summary>
        /// Closes the current model and releases all resources and instances
        /// </summary>
        public virtual void Close()
        {
            var dbName = DatabaseName;
            ModelFactors = null;          
            Header = null;
            
            if (_editTransactionEntityCursor != null)
                EndTransaction();
            InstanceCache.Close();

            //dispose any referenced models
            foreach (var refModel in _referencedModels.Select(r => r.Model).OfType<IDisposable>())
                refModel.Dispose();
            _referencedModels.Clear();

            try //try and tidy up if required
            {
                if (_deleteOnClose && File.Exists(dbName))
                    File.Delete(dbName);
            }
            catch (Exception)
            {
                // ignored
            }
            _deleteOnClose = false;
        }
        #endregion

        protected void Open(string fileName, XbimDBAccess accessMode, bool deleteOnClose)
        {      
            Open(fileName, accessMode);
            _deleteOnClose = deleteOnClose;
        }

        /// <summary>
        /// Begins a cache of all data read from the model, improves performance where data is read many times
        /// </summary>
        public void CacheStart()
        {
            if (_editTransactionEntityCursor == null) //if we are in a transaction caching is on anyway
                 InstanceCache.CacheStart();
        }
        /// <summary>
        /// Clears all read data in the cache
        /// </summary>
        public void CacheClear()
        {
            if (_editTransactionEntityCursor == null) //if we are in a transaction do not clear
                InstanceCache.CacheClear();
        }

        /// <summary>
        /// Stops further caching of data and clears the current cache
        /// </summary>
        public void CacheStop()
        {
            if (_editTransactionEntityCursor == null)  //if we are in a transaction do not stop
                InstanceCache.CacheStop();
        }

        /// <summary>
        /// Opens an Xbim model only, to open Ifc, IfcZip and IfcXML files use the CreatFrom method
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="accessMode"></param>
        /// <param name="progDelegate"></param>
        /// <returns>True if successful</returns>
        public virtual bool Open(string fileName, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null)
        {
            try
            {
                Close();
                InstanceCache.Open(fileName, accessMode); //opens the database
                return true;
            }
            catch (Exception e)
            {
                throw new XbimException(string.Format("Error opening file {0}\n{1}", fileName, e.Message), e);
            }
        }

        public bool CanEdit
        {
            get
            {
                return Cache.AccessMode == XbimDBAccess.ReadWrite || Cache.AccessMode == XbimDBAccess.Exclusive;
            }
        }

        public bool SaveAs(string outputFileName, XbimStorageType? storageType = null, ReportProgressDelegate progress = null, IDictionary<int, int> map = null)
        {

            try
            {
                if (!storageType.HasValue)
                    storageType = StorageType(outputFileName);
                if (storageType.Value == XbimStorageType.INVALID)
                {
                    var ext = Path.GetExtension(outputFileName);
                    if(string.IsNullOrWhiteSpace(ext))
                        throw new XbimException("Invalid file type, no extension specified in file " + outputFileName);
                    throw new XbimException("Invalid file type ." + ext.ToUpper() + " in file " + outputFileName);
                }
                if (storageType.Value == XbimStorageType.XBIM && DatabaseName != null) //make a copy
                {
                    var srcFile = DatabaseName;
                    if(string.Compare(srcFile, outputFileName, true, CultureInfo.InvariantCulture) == 0)
                        throw new XbimException("Cannot save file to the same name, " + outputFileName);
                    var deleteOnClose = _deleteOnClose;
                    var accessMode = InstanceCache.AccessMode;
                    try
                    {
                       
                        _deleteOnClose = false; //regardless we need to keep it to copy it
                        Close(); 
                        File.Copy(srcFile, outputFileName);
                        
                        if (deleteOnClose)
                            File.Delete(srcFile);
                        srcFile = outputFileName;
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw new XbimException("Failed to save file as outputFileName" , e);
                    }
                    finally
                    {
                        Open(srcFile, accessMode);
                    }
                }
                InstanceCache.SaveAs(storageType.Value, outputFileName, progress, map);
                return true;
            }
            catch (Exception e)
            {
                throw new XbimException(string.Format("Failed to Save file as {0}\n{1}", outputFileName, e.Message), e);
            }
        }

        // Extract first ifc file from zipped file and save in the same directory
        internal string ExportZippedIfc(string inputIfcFile)
        {
            try
            {
                using (var zis = new ZipInputStream(File.OpenRead(inputIfcFile)))
                {
                    var zs = zis.GetNextEntry();
                    while (zs != null)
                    {
                        var fileName = Path.GetFileName(zs.Name);
                        if (fileName == null || !fileName.ToLower().EndsWith(".ifc"))
                        {
                            zs = zis.GetNextEntry();
                            continue;
                        }
                        using (var fs = File.Create(fileName))
                        {
                            var i = 2048;
                            var b = new byte[i];
                            while (true)
                            {
                                i = zis.Read(b, 0, b.Length);
                                if (i > 0)
                                    fs.Write(b, 0, i);
                                else
                                    break;
                            }
                        }
                        return fileName;
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating Ifc File from ZIP = " + inputIfcFile, e);
            }
            return "";
        }



        #region Helpers

        private XbimStorageType StorageType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return XbimStorageType.INVALID;
            var ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".xbim":
                case ".xbimf":
                    return XbimStorageType.XBIM;
                case ".ifc":
                case ".stp":
                case ".step21":
                    return XbimStorageType.Step21;
                case ".ifcxml":
                    return XbimStorageType.IFCXML;
                case ".zip":
                case ".ifczip":
                case ".stpzip":
                case ".step21zip":
                    return XbimStorageType.Step21Zip;
            }
            return XbimStorageType.INVALID;
        }

        #endregion


        public void Print()
        {
            InstanceCache.Print();
        }

        

        ~EsentModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                   //managed resources
                    Close();
                }
                //unmanaged, mostly esent related
                InstanceCache.Dispose();
            }
                catch
                {
                    // ignored
                }
            }
            _disposed = true;
        }

        public void CheckMaps()
        {
            foreach (var mesh in GetGeometryData(XbimGeometryType.TriangulatedMesh))
            {
                Debug.WriteLine("{0}, hash = {1}", mesh.GeometryLabel, mesh.GeometryHash);
            }
        }

        public XbimGeometryHandleCollection GetGeometryHandles(XbimGeometryType geomType=XbimGeometryType.TriangulatedMesh, XbimGeometrySort sortOrder=XbimGeometrySort.OrderByIfcSurfaceStyleThenIfcType)
        {
            return InstanceCache.GetGeometryHandles(geomType,sortOrder);
        }

        public XbimGeometryHandle GetGeometryHandle(int geometryLabel)
        {
            return InstanceCache.GetGeometryHandle(geometryLabel);
        }

        /// <summary>
        /// Returns all the geometries for the geometry type
        /// Typically bounding box returns a single object, triangulated mesh mes may return multiple geometry meshes
        /// where an object is made of multiple materials
        /// </summary>
        /// <param name="productLabel"></param>
        /// <param name="geomType"></param>
        /// <returns></returns>
        public IEnumerable<XbimGeometryData> GetGeometryData(int productLabel, XbimGeometryType geomType)
        {
            var entity = InstanceCache.GetInstance(productLabel, false, true);
            if (entity != null)
            {
                foreach (var item in InstanceCache.GetGeometry(Metadata.ExpressTypeId(entity), productLabel, geomType))
                {
                    yield return item;
                }
            }

            // RefencedModels must NOT be iterated because of potential entityLabel clashes.
            // identity needs instead to be tested at the model level of children first, then call this function on the matching child.

            //else // look in referenced models
            //{
            //    foreach (XbimReferencedModel refModel in this.RefencedModels)
            //    {
            //        foreach (var item in refModel.Model.GetGeometryData(productLabel, geomType))
            //        {
            //            yield return item;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Returns the level of geometry supported in the model
        /// 0 = No geometry has been compiled in the model
        /// 1 = Triangulated Mesh only
        /// 2 = Polygonal  geometry meshes supporting maps
        /// </summary>
        public int GeometrySupportLevel
        {
            get
            {
                if (DatabaseHasInstanceTable())
                {
                    using (var i = GetShapeInstanceTable())
                    {
                        if (i.RetrieveCount() > 0)
                            return 2;
                    }
                }
                else if (DatabaseHasGeometryTable() && GetGeometryData(XbimGeometryType.TriangulatedMesh).Any()) 
                    return 1;
                return 0;
            }
        }

        public IEnumerable<XbimGeometryData> GetGeometryData(XbimGeometryType ofType)
        {
            return InstanceCache.GetGeometryData(ofType);
        }

        internal XbimEntityCursor GetEntityTable()
        {
            return InstanceCache.GetEntityTable();
        }

        public void Compact(string targetModelName)
        {
            Cache.Compact(targetModelName);
        }

        /// <summary>
        /// Inserts a deep copy of the toCopy object into this model
        /// All property values are copied to the maximum depth
        /// Inverse properties are not copied
        /// </summary>
        /// <param name="toCopy">Instance to copy</param>
        /// <param name="mappings">Supply a dictionary of mappings if repeat copy insertions are to be made</param>
        /// <param name="txn"></param>
        /// <param name="includeInverses"></param>
        /// <returns></returns>
        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, XbimReadWriteTransaction txn, bool includeInverses = false) where T : IPersistEntity
        {
            return Cache.InsertCopy(toCopy, mappings, txn, includeInverses);
        }

        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, XbimReadWriteTransaction txn, PropertyTranformDelegate propTransform, bool includeInverses = false) where T : IPersistEntity
        {
            return Cache.InsertCopy(toCopy, mappings, txn, includeInverses, propTransform);
        }

        internal void EndTransaction()
        {
            FreeTable(_editTransactionEntityCursor); //release the cursor back to the pool
            InstanceCache.EndCaching();
            _editTransactionEntityCursor = null;
        }
       
        internal void Flush()
        {
            InstanceCache.Write(_editTransactionEntityCursor);
        }

        internal XbimEntityCursor GetTransactingCursor()
        {
            Debug.Assert(_editTransactionEntityCursor != null);
            return _editTransactionEntityCursor;
        }

     

        public XbimGeometryData GetGeometryData(XbimGeometryHandle handle)
        {
            return InstanceCache.GetGeometryData(handle);
        }

        public XbimGeometryData GetGeometryData(int geomLabel)
        {
            return InstanceCache.GetGeometryData(geomLabel);
        }

        public IEnumerable<XbimGeometryData> GetGeometryData(IEnumerable<XbimGeometryHandle> handles)
        {
            return InstanceCache.GetGeometryData(handles);
        }

        
        



       
        /// <summary>
        /// Returns an enumerable of the handles to only the entities in this model
        /// Note this do NOT include entities that are in any federated models
        /// </summary>
        public IEnumerable<XbimInstanceHandle> InstanceHandles 
        {
            get {
                return InstanceCache.InstanceHandles;
            }
        }

        internal IPersistEntity GetInstanceVolatile(XbimInstanceHandle item)
        {
          return item.Model.GetInstanceVolatile(item.EntityLabel);
        }



        public object Tag { get; set; }
        
        public XbimShapeGeometryCursor GetShapeGeometryTable()
        {
            return InstanceCache.GetShapeGeometryTable();
        }

        public XbimShapeInstanceCursor GetShapeInstanceTable()
        {
            return InstanceCache.GetShapeInstanceTable();
        }

        /// <summary>
        /// Invoke the function before meshing to ensure database structure is available
        /// </summary>
        /// <returns></returns>
        public bool EnsureGeometryTables()
        {
            return InstanceCache.EnsureGeometryTables();
        }

        public bool DeleteGeometryCache()
        {
            return InstanceCache.DeleteGeometry();
        }

        public bool DatabaseHasGeometryTable()
        {
            return InstanceCache.DatabaseHasGeometryTable();
        }

        public bool DatabaseHasInstanceTable()
        {
            return InstanceCache.DatabaseHasInstanceTable();
        }

        bool IModel.IsTransactional
        {
            get { return true; }
        }

        ITransaction IModel.BeginTransaction(string name)
        {
            return BeginTransaction(name);
        }

        /// <summary>
        /// Weak reference allows garbage collector to collect transaction once it goes out of the scope
        /// even if it is still referenced from model. This is important for the cases where the transaction
        /// is both not commited and not rolled back either.
        /// </summary>
        private WeakReference _transactionReference;


        public ITransaction CurrentTransaction
        {
            get
            {
                if (_transactionReference == null || !_transactionReference.IsAlive)
                    return null;
                return _transactionReference.Target as ITransaction;
            }
            internal set
            {
                if (value == null)
                {
                    _transactionReference = null;
                    return;
                }
                if (_transactionReference == null)
                    _transactionReference = new WeakReference(value);
                else
                    _transactionReference.Target = value;
            }
        }

        #region Federation 
        private readonly XbimReferencedModelCollection _referencedModels = new XbimReferencedModelCollection();

        public IEnumerable<IReferencedModel> ReferencedModels
        {
            get
            {
                return _referencedModels.AsEnumerable();
            }
        }

        public void AddModelReference(IReferencedModel model)
        {
            _referencedModels.Add(model);
        }

        IReadOnlyEntityCollection IFederatedModel.Instances
        {
            get { return Instances; }
        }

        /// <summary>
        /// Returns true if the model contains reference models or the model has extension xBIMf
        /// </summary>
        public virtual bool IsFederation
        {
            get
            {
                return _referencedModels.Any() || string.Compare(Path.GetExtension(InstanceCache.DatabaseName), ".xbimf", StringComparison.OrdinalIgnoreCase) == 0; 
            }
        }

        protected string NextReferenceIdentifier()
        {
            return _referencedModels.NextIdentifer();
        }

        /// <summary>
        /// Returns an enumerable of the handles to all entities in the model
        /// Note this includes entities that are in any federated models
        /// </summary>
        public IEnumerable<XbimInstanceHandle> AllInstancesHandles
        {
            get
            {
                foreach (var h in InstanceHandles)
                    yield return h;
                foreach (var refModel in ReferencedModels.Where(r => r.Model is EsentModel).Select(r => r.Model as EsentModel))
                    foreach (var h in refModel.AllInstancesHandles)
                        yield return h;
            }
        }

        public void EnsureUniqueUserDefinedId()
        {
            short iId = 0;
            var allModels =
                (new[] {this}).Concat(ReferencedModels.Where(rm => rm.Model is EsentModel).Select(rm => rm.Model))
                    .Cast<EsentModel>();
            foreach (var model in allModels)
            {
                model.UserDefinedId = iId++;
            }
        }
        #endregion
    }

    public delegate object PropertyTranformDelegate(ExpressMetaProperty property, object parentObject);
    
}
