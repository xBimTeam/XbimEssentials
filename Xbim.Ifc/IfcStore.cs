using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;
using Xbim.IO.Step21;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;

namespace Xbim.Ifc
{
    public class IfcStore : IModel, IDisposable, IFederatedModel, IEquatable<IModel>
    {
        private readonly IModel _model;
        private readonly XbimSchemaVersion _schema;
        private const string RefDocument = "XbimReferencedModel";
        private readonly bool _deleteModelOnClose;
        private readonly string _xbimFileName;
        public event NewEntityHandler EntityNew;
        public event ModifiedEntityHandler EntityModified;
        public event DeletedEntityHandler EntityDeleted;

        public IInverseCache InverseCache
        {
            get { return _model.InverseCache; }
        }

        public object Tag { get; set; }

        private bool _disposed;
        /// <summary>
        /// The default largest size in MB for an IFC file to be loaded into memory, above this size the store will choose to use 
        /// the database storage media to minimise the memory footprint. This size can be set in the config file or in the open statement of this store 
        /// </summary>
        public static double DefaultIfcDatabaseSizeThreshHold = 100; //default size set to 100MB
        private IIfcOwnerHistory _ownerHistoryAddObject;
        private IIfcOwnerHistory _ownerHistoryModifyObject;

        private IIfcPersonAndOrganization _defaultOwningUser;
        private IIfcApplication _defaultOwningApplication;
        private readonly XbimEditorCredentials _editorDetails;
        private readonly ReferencedModelCollection _referencedModels = new ReferencedModelCollection();

        protected IfcStore(IModel iModel, XbimSchemaVersion schema, XbimEditorCredentials editorDetails, string fileName = null, string xbimFileName = null,
            bool deleteOnClose = false)
        {
            _model = iModel;
            _model.EntityNew += _model_EntityNew;
            _model.EntityDeleted += _model_EntityDeleted;
            _model.EntityModified += _model_EntityModified;
            _deleteModelOnClose = deleteOnClose;
            FileName = fileName;
            _xbimFileName = xbimFileName;
            _schema = schema;
            if (editorDetails == null)
                _editorDetails = new XbimEditorCredentials()
                {
                    ApplicationDevelopersName = "Unspecified",
                    ApplicationVersion = "Unspecified",
                    ApplicationFullName = "Unspecified",
                    EditorsFamilyName = Environment.UserName,
                    EditorsOrganisationName = "Unspecified",
                    EditorsGivenName = ""
                };
            else
                _editorDetails = editorDetails;

            _model.EntityNew += IfcRootInit;
            _model.EntityModified += IfcRootModified;

            LoadReferenceModels();
            CalculateModelFactors();
        }

        protected IfcStore(IModel iModel, XbimSchemaVersion schema)
        {
            _model = iModel;
            _model.EntityNew += _model_EntityNew;
            _model.EntityDeleted += _model_EntityDeleted;
            _model.EntityModified += _model_EntityModified;
            _deleteModelOnClose = true;
            FileName = null;
            _xbimFileName = null;
            _schema = schema;
            _editorDetails = null;
        }

        private void _model_EntityDeleted(IPersistEntity entity)
        {
            if (EntityDeleted != null) EntityDeleted.Invoke(entity);
        }

        private void _model_EntityNew(IPersistEntity entity)
        {
            if (EntityNew != null) EntityNew.Invoke(entity);
        }

        private void _model_EntityModified(IPersistEntity entity, int property)
        {
            if (EntityModified != null) EntityModified.Invoke(entity, property);
        }

        private static EsentModel CreateEsentModel(XbimSchemaVersion schema, int codePageOverride)
        {
            var ef = GetFactory(schema);
            var model = new EsentModel(ef)
            {
                CodePageOverride = codePageOverride
            };
            return model;
        }

        private static MemoryModel CreateMemoryModel(XbimSchemaVersion schema)
        {
            var ef = GetFactory(schema);
            return new MemoryModel(ef);
        }

        /// <summary>
        /// You can use this function to open IFC model from stream. You need to know file type (IFC, IFCZIP, IFCXML) and schema type (IFC2x3 or IFC4) to be able to use this function.
        /// If you don't know you should the overloaded function which takes file pats as an argument. That will automatically detect schema and file type. If you want to open *.xbim
        /// file you should also use the path based overload because Esent database needs to operate on the file and this function will have to create temporal file if it is not a file stream.
        /// If it is a FileStream this will close it to keep exclusive access.
        /// </summary>
        /// <param name="data">Stream of data</param>
        /// <param name="dataType">Type of data (*.ifc, *.ifcxml, *.ifczip)</param>
        /// <param name="schema">IFC schema (IFC2x3, IFC4). Other schemas are not supported by this class.</param>
        /// <param name="modelType">Type of morel to be used. You can choose between EsentModel and MemoryModel</param>
        /// <param name="editorDetails">Optional details. You should always pass these if you are going to change the data.</param>
        /// <param name="accessMode">Access mode to the stream. This is only important if you choose EsentModel. MemoryModel is completely in memory so this is not relevant</param>
        /// <param name="progDelegate">Progress reporting delegate</param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used accoring to the Ifc specification. </param>/// <returns></returns>
        public static IfcStore Open(Stream data, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, XbimEditorCredentials editorDetails = null, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            //any Esent model needs to run from the file so we need to create a temporal one
            var xbimFilePath = Path.GetTempFileName();
            xbimFilePath = Path.ChangeExtension(xbimFilePath, ".xbim");

            switch (dataType)
            {
                case StorageType.Xbim:
                    //xBIM file has to be opened from the file so we need to create temporal file if it is not a local file stream
                    var fStream = data as FileStream;
                    var localFile = false;
                    if (fStream != null)
                    {
                        var name = fStream.Name;
                        //if it is an existing local file, just use it
                        if (File.Exists(name))
                        {
                            xbimFilePath = name;
                            //close the stream from argument to have an exclusive access to the file
                            data.Close();
                            localFile = true;
                        }
                    }
                    if (!localFile)
                    {
                        using (var tempFile = File.Create(xbimFilePath))
                        {
                            data.CopyTo(tempFile);
                            tempFile.Close();
                        }
                    }
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        model.Open(xbimFilePath, accessMode, progDelegate);
                        return new IfcStore(model, schema, editorDetails, xbimFilePath);
                    }
                case StorageType.IfcXml:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return new IfcStore(model, schema, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadXml(data, data.Length, progDelegate);
                        return new IfcStore(model, schema, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                case StorageType.Stp:
                case StorageType.Ifc:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return new IfcStore(model, schema, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadStep21(data, data.Length, progDelegate);
                        return new IfcStore(model, schema, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                case StorageType.IfcZip:
                case StorageType.StpZip:
                case StorageType.Zip:
                    if (modelType == XbimModelType.EsentModel)
                    {
                        var model = CreateEsentModel(schema, codePageOverride);
                        if (model.CreateFrom(data, data.Length, dataType, xbimFilePath, progDelegate, true, true))
                            return new IfcStore(model, schema, editorDetails, xbimFilePath);
                        else
                            throw new XbimException("Failed to create Esent model");
                    }
                    if (modelType == XbimModelType.MemoryModel)
                    {
                        var model = CreateMemoryModel(schema);
                        model.LoadZip(data, progDelegate);
                        return new IfcStore(model, schema, editorDetails);
                    }
                    throw new ArgumentOutOfRangeException("IfcStore only supports EsentModel and MemoryModel");
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        //public static IfcStore Open(string path, XbimEditorCredentials editorDetails,bool writeAccess = true,
        //double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null)
        //{
        //    return Open(path, editorDetails, ifcDatabaseSizeThreshHold, progDelegate, writeAccess?XbimDBAccess.ReadWrite : XbimDBAccess.Read);
        //}

        /// <summary>
        /// Opens an IFC file, Ifcxml, IfcZip, xbim
        /// </summary>
        /// <param name="path">the file name of the ifc, ifczip, ifcxml or xbim file to be opened</param>
        /// <param name="editorDetails">This is only required if the store is opened for editing</param>
        /// <param name="ifcDatabaseSizeThreshHold">Expressed in MB. If not defined the DefaultIfcDatabaseSizeThreshHold is used, 
        /// IFC files below this size will be opened in memory, above this size a database will be created. If -1 is specified an in memory model will be 
        /// created for all IFC files that are opened. Xbim files are always opened as databases</param>
        /// <param name="progDelegate"></param>
        /// <param name="accessMode"></param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used accoring to the Ifc specification. </param>
        public static IfcStore Open(string path, XbimEditorCredentials editorDetails = null, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
        {
            path = Path.GetFullPath(path);

            if (!Directory.Exists(Path.GetDirectoryName(path) ?? ""))
                throw new DirectoryNotFoundException(Path.GetDirectoryName(path) + " directory was not found");
            if (!File.Exists(path))
                throw new FileNotFoundException(path + " file was not found");
            var storageType = path.StorageType();
            var ifcVersion = GetXbimSchemaVersion(path);
            if (ifcVersion == XbimSchemaVersion.Unsupported)
            {
                throw new FileLoadException(path + " is not a valid IFC file format, ifc, ifcxml, ifczip and xBIM are supported.");
            }

            if (storageType == StorageType.Xbim) //open the XbimFile
            {
                var model = CreateEsentModel(ifcVersion, codePageOverride);
                model.Open(path, accessMode, progDelegate);
                return new IfcStore(model, ifcVersion, editorDetails, path);
            }
            else //it will be an IFC file if we are at this point
            {
                var fInfo = new FileInfo(path);
                double ifcMaxLength = (ifcDatabaseSizeThreshHold ?? DefaultIfcDatabaseSizeThreshHold) * 1024 * 1024;
                if (ifcMaxLength >= 0 && fInfo.Length > ifcMaxLength) //we need to make an Esent database, if ifcMaxLength<0 we use in memory
                {
                    var tmpFileName = Path.GetTempFileName();
                    var model = CreateEsentModel(ifcVersion, codePageOverride);
                    if (model.CreateFrom(path, tmpFileName, progDelegate, true))
                        return new IfcStore(model, ifcVersion, editorDetails, path, tmpFileName, true);
                    throw new FileLoadException(path + " file was not a valid IFC format");
                }
                else //we can use a memory model
                {
                    var ef = GetFactory(ifcVersion);
                    var model = new MemoryModel(ef);
                    if (storageType.HasFlag(StorageType.IfcZip) || storageType.HasFlag(StorageType.Zip))
                    {
                        model.LoadZip(path, progDelegate);
                    }
                    else if (storageType.HasFlag(StorageType.Ifc))
                        model.LoadStep21(path, progDelegate);
                    else if (storageType.HasFlag(StorageType.IfcXml))
                        model.LoadXml(path, progDelegate);

                    // if we are looking at a memory model loaded from a file it might be safe to fix the file name in the 
                    // header with the actual file loaded
                    //
                    FileInfo f = new FileInfo(path);
                    model.Header.FileName.Name = f.FullName;
                    return new IfcStore(model, ifcVersion, editorDetails, path);
                }
            }
        }

        private static IEntityFactory GetFactory(XbimSchemaVersion type)
        {
            switch (type)
            {
                case XbimSchemaVersion.Ifc4:
                    return new Ifc4.EntityFactoryIfc4();
                case XbimSchemaVersion.Ifc4x1:
                    return new Ifc4.EntityFactoryIfc4x1();
                case XbimSchemaVersion.Ifc2X3:
                    return new Ifc2x3.EntityFactoryIfc2x3();
                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new NotSupportedException("Schema '" + type + "' is not supported");
            }
        }

        public static XbimSchemaVersion GetXbimSchemaVersion(string path)
        {
            var storageType = path.StorageType();
            if (storageType == StorageType.Invalid)
            {
                return XbimSchemaVersion.Unsupported;
            }
            IList<string> schemas = null;
            if (storageType != StorageType.Xbim)
            {
                return MemoryModel.GetSchemaVersion(path);
            }

            var stepHeader = EsentModel.GetStepFileHeader(path);
            schemas = stepHeader.FileSchema.Schemas;
            var schemaIdentifier = string.Join(", ", schemas);
            foreach (var schema in schemas)
            {
                if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4;
                if (string.Compare(schema, "Ifc4x1", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4x1;
                if (string.Compare(schema, "Ifc2x3", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc2X3;
                if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                    return XbimSchemaVersion.Ifc2X3;

            }

            return XbimSchemaVersion.Unsupported;
        }

        public int UserDefinedId
        {
            get { return _model.UserDefinedId; }
            set { _model.UserDefinedId = value; }
        }

        public IGeometryStore GeometryStore
        {
            get { return _model.GeometryStore; }
        }

        public IStepFileHeader Header
        {
            get { return _model.Header; }
        }

        public bool IsTransactional
        {
            get { return _model.IsTransactional; }
        }

        public IEntityCollection Instances
        {
            get { return _model.Instances; }
        }

        bool IModel.Activate(IPersistEntity owningEntity)
        {
            return _model.Activate(owningEntity);
        }

        public void Delete(IPersistEntity entity)
        {
            _model.Delete(entity);
        }

        public ITransaction BeginTransaction(string name = null)
        {
            var esentModel = _model as EsentModel;
            if (esentModel != null) //we need to do transaction handling on Esent model, make sure we can write to it
                return esentModel.BeginTransaction(name);

            var memoryModel = _model as MemoryModel;
            if (memoryModel == null)
                throw new XbimException("Native store does not support transactions");
            return memoryModel.BeginTransaction(name);
        }

        public ITransaction CurrentTransaction
        {
            get { return _model.CurrentTransaction; }
        }

        public ExpressMetaData Metadata
        {
            get { return _model.Metadata; }
        }

        public IModelFactors ModelFactors
        {
            get { return _model.ModelFactors; }
        }

        public string FileName { get; set; }

        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
            bool keepLabels) where T : IPersistEntity
        {
            return _model.InsertCopy(toCopy, mappings, propTransform, includeInverses, keepLabels);
        }

        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            _model.ForEach(source, body);
        }

        public void Dispose()
        {
            Close();
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
                        //release event handlers
                        if (_model != null)
                        {
                            _model.EntityDeleted -= _model_EntityDeleted;
                            _model.EntityNew -= _model_EntityNew;
                            _model.EntityModified -= _model_EntityModified;
                        }

                        //managed resources
                        var disposeInterface = _model as IDisposable;
                        if (disposeInterface != null)
                            disposeInterface.Dispose();
                    }
                    //unmanaged, mostly Esent related                  
                }
                catch
                {
                    // ignored
                }
            }
            _disposed = true;
        }



        /// <summary>
        /// Closes the store and disposes of all resources, the store is invalid after this call
        /// </summary>
        public void Close()
        {
            foreach (var referencedModel in _referencedModels)
            {
                AttemptClose(referencedModel.Model);
            }
            AttemptClose(_model);

            try //try and tidy up if required
            {
                if (_deleteModelOnClose && !string.IsNullOrWhiteSpace(_xbimFileName) && File.Exists(_xbimFileName))
                    File.Delete(_xbimFileName);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void AttemptClose(IModel referencedModel)
        {
            var esentSub = referencedModel as EsentModel;
            if (esentSub != null)
                esentSub.Close();

            var ifcStoreSub = referencedModel as IfcStore;
            if (ifcStoreSub != null)
                ifcStoreSub.Close();
        }

        /// <summary>
        /// Creates an Database store at the specified location
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="editorDetails"></param>
        /// <param name="ifcVersion"></param>
        /// <returns></returns>
        public static IfcStore Create(string filePath, XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion)
        {
            var ef = GetFactory(ifcVersion);
            var temporaryModel = EsentModel.CreateModel(ef, filePath);
            return new IfcStore(temporaryModel, ifcVersion, editorDetails, temporaryModel.DatabaseName);
        }

        public static IfcStore Create(XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var ef = GetFactory(ifcVersion);
            if (storageType == XbimStoreType.EsentDatabase)
            {
                var temporaryModel = EsentModel.CreateTemporaryModel(ef);
                return new IfcStore(temporaryModel, ifcVersion, editorDetails, temporaryModel.DatabaseName); //it will delete itself anyway
            }

            var memoryModel = new MemoryModel(ef);
            return new IfcStore(memoryModel, ifcVersion, editorDetails);
        }

        public static IfcStore Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            var ef = GetFactory(ifcVersion);
            if (storageType == XbimStoreType.EsentDatabase)
            {
                var temporaryModel = EsentModel.CreateTemporaryModel(ef);
                return new IfcStore(temporaryModel, ifcVersion); //it will delete itself anyway
            }

            var memoryModel = new MemoryModel(ef);
            return new IfcStore(memoryModel, ifcVersion);

        }
        #region OwnerHistory Management


        private void IfcRootModified(IPersistEntity entity, int property)
        {
            var root = entity as IIfcRoot;
            if (root == null || root.OwnerHistory == _ownerHistoryAddObject)
                return;

            if (root.OwnerHistory != _ownerHistoryModifyObject)
            {
                root.OwnerHistory = OwnerHistoryModifyObject;
                OwnerHistoryModifyObject.LastModifiedDate = DateTime.Now;
            }
        }

        private void IfcRootInit(IPersistEntity entity)
        {
            var root = entity as IIfcRoot;
            if (root != null)
            {
                root.OwnerHistory = OwnerHistoryAddObject;
                root.GlobalId = Guid.NewGuid().ToPart21();
                OwnerHistoryAddObject.LastModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Returns default user used to fill in owner history on new or modified objects. This object is only populated if
        /// you provide XbimEditorCredentials in one of constructors
        /// </summary>
        public IIfcPersonAndOrganization DefaultOwningUser
        {
            get
            {
                if (_defaultOwningUser != null) return _defaultOwningUser;

                //data wasn't supplied to create default user and application
                if (_editorDetails == null)
                    return null;

                if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    var person = Instances.New<Ifc4.ActorResource.IfcPerson>(p =>
                    {
                        p.GivenName = _editorDetails.EditorsGivenName;
                        p.FamilyName = _editorDetails.EditorsFamilyName;
                    });
                    var organization = Instances.OfType<Ifc4.ActorResource.IfcOrganization>().FirstOrDefault(o => o.Name == _editorDetails.EditorsOrganisationName)
                        ?? Instances.New<Ifc4.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
                    _defaultOwningUser = Instances.New<Ifc4.ActorResource.IfcPersonAndOrganization>(po =>
                    {
                        po.TheOrganization = organization;
                        po.ThePerson = person;
                    });
                }
                else
                {
                    var person = Instances.New<Ifc2x3.ActorResource.IfcPerson>(p =>
                    {
                        p.GivenName = _editorDetails.EditorsGivenName;
                        p.FamilyName = _editorDetails.EditorsFamilyName;
                    });
                    var organization = Instances.OfType<Ifc2x3.ActorResource.IfcOrganization>().FirstOrDefault(o => o.Name == _editorDetails.EditorsOrganisationName)
                        ?? Instances.New<Ifc2x3.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
                    _defaultOwningUser = Instances.New<Ifc2x3.ActorResource.IfcPersonAndOrganization>(po =>
                    {
                        po.TheOrganization = organization;
                        po.ThePerson = person;
                    });
                }
                return _defaultOwningUser;
            }
        }

        /// <summary>
        /// Returns default application used to fill in owner history on new or modified objects. This object is only populated if
        /// you provide XbimEditorCredentials in one of constructors
        /// </summary>
        public IIfcApplication DefaultOwningApplication
        {
            get
            {
                if (_defaultOwningApplication != null) return _defaultOwningApplication;

                //data wasn't supplied to create default user and application
                if (_editorDetails == null)
                    return null;

                if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                    return _defaultOwningApplication ??
                         (_defaultOwningApplication =
                             Instances.New<Ifc4.UtilityResource.IfcApplication>(a =>
                             {
                                 a.ApplicationDeveloper = Instances.OfType<Ifc4.ActorResource.IfcOrganization>().FirstOrDefault(o => o.Name == _editorDetails.EditorsOrganisationName)
                                 ?? Instances.New<Ifc4.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
                                 a.ApplicationFullName = _editorDetails.ApplicationFullName;
                                 a.ApplicationIdentifier = _editorDetails.ApplicationIdentifier;
                                 a.Version = _editorDetails.ApplicationVersion;
                             }
                ));
                return _defaultOwningApplication ??
                        (_defaultOwningApplication =
                            Instances.New<Ifc2x3.UtilityResource.IfcApplication>(a =>
                            {
                                a.ApplicationDeveloper = Instances.OfType<Ifc2x3.ActorResource.IfcOrganization>().FirstOrDefault(o => o.Name == _editorDetails.EditorsOrganisationName)
                                ?? Instances.New<Ifc2x3.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
                                a.ApplicationFullName = _editorDetails.ApplicationFullName;
                                a.ApplicationIdentifier = _editorDetails.ApplicationIdentifier;
                                a.Version = _editorDetails.ApplicationVersion;
                            }
                ));
            }
        }

        public IIfcOwnerHistory OwnerHistoryAddObject
        {
            get
            {
                if (_ownerHistoryAddObject != null)
                    return _ownerHistoryAddObject;
                if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    var histAdd = Instances.New<Ifc4.UtilityResource.IfcOwnerHistory>();
                    histAdd.OwningUser = (Ifc4.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                    histAdd.OwningApplication = (Ifc4.UtilityResource.IfcApplication)DefaultOwningApplication;
                    histAdd.ChangeAction = IfcChangeActionEnum.ADDED;
                    _ownerHistoryAddObject = histAdd;
                }
                else
                {
                    var histAdd = Instances.New<Ifc2x3.UtilityResource.IfcOwnerHistory>();
                    histAdd.OwningUser = (Ifc2x3.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                    histAdd.OwningApplication = (Ifc2x3.UtilityResource.IfcApplication)DefaultOwningApplication;
                    histAdd.ChangeAction = Ifc2x3.UtilityResource.IfcChangeActionEnum.ADDED;
                    _ownerHistoryAddObject = histAdd;
                }
                return _ownerHistoryAddObject;
            }
        }

        internal IIfcOwnerHistory OwnerHistoryModifyObject
        {
            get
            {
                if (_ownerHistoryModifyObject != null)
                    return _ownerHistoryModifyObject;
                if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    var histmod = Instances.New<Ifc4.UtilityResource.IfcOwnerHistory>();
                    histmod.OwningUser = (Ifc4.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                    histmod.OwningApplication = (Ifc4.UtilityResource.IfcApplication)DefaultOwningApplication;
                    histmod.ChangeAction = IfcChangeActionEnum.MODIFIED;
                    _ownerHistoryModifyObject = histmod;
                }
                else
                {
                    var histmod = Instances.New<Ifc2x3.UtilityResource.IfcOwnerHistory>();
                    histmod.OwningUser = (Ifc2x3.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                    histmod.OwningApplication = (Ifc2x3.UtilityResource.IfcApplication)DefaultOwningApplication;
                    histmod.ChangeAction = Ifc2x3.UtilityResource.IfcChangeActionEnum.MODIFIED;
                    _ownerHistoryModifyObject = histmod;
                }
                return _ownerHistoryModifyObject;
            }
        }

        protected XbimEditorCredentials EditorDetails
        {
            get { return _editorDetails; }
        }

        #endregion

        #region Transaction support


        #endregion

        /// <summary>
        /// Saves the model to the specified file
        /// </summary>
        /// <param name="fileName">Name of the file to save to, if no format is specified the extension is used to determine the format</param>
        /// <param name="format">if specified saves in the required format and changes the extension to the correct one</param>
        /// <param name="progDelegate">reports on progress</param>
        public void SaveAs(string fileName, StorageType? format = null, ReportProgressDelegate progDelegate = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            StorageType actualFormat = StorageType.Invalid;
            if (format.HasValue)
            {
                if (format.Value.HasFlag(StorageType.IfcZip))
                {
                    extension = ".ifczip";
                    actualFormat = StorageType.IfcZip;
                    if (format.Value.HasFlag(StorageType.IfcXml)) //set it to default to Ifc
                        actualFormat |= StorageType.IfcXml;
                    else
                        actualFormat |= StorageType.Ifc;
                }
                else if (format.Value.HasFlag(StorageType.Ifc))
                {
                    extension = ".ifc";
                    actualFormat = StorageType.Ifc;
                }
                else if (format.Value.HasFlag(StorageType.IfcXml))
                {
                    extension = ".ifcxml";
                    actualFormat = StorageType.IfcXml;
                }
                else if (format.Value.HasFlag(StorageType.Xbim))
                {
                    extension = ".xbim";
                    actualFormat = StorageType.Xbim;
                }
            }
            else
            {
                if (extension == ".ifczip")
                {
                    actualFormat = StorageType.IfcZip;
                    actualFormat |= StorageType.Ifc; //the default
                }
                else if (extension == ".ifcxml")
                    actualFormat = StorageType.IfcXml;
                else if (extension == ".xbim")
                    actualFormat = StorageType.Xbim;
                else if (extension == ".ifc")
                {
                    extension = ".ifc";
                    actualFormat = StorageType.Ifc; //the default
                }
                else
                {
                    // we don't want to loose the original extension required by the user, but we need to add .ifc 
                    // and set StorageType.Ifc as default
                    extension = extension + ".ifc";
                    actualFormat = StorageType.Ifc; //the default
                }
            }
            var actualFileName = Path.ChangeExtension(fileName, extension);
            var esentModel = _model as EsentModel;
            if (esentModel != null)
            {
                var xbimTarget = !string.IsNullOrEmpty(extension) &&
                                 string.Compare(extension, ".xbim", StringComparison.OrdinalIgnoreCase) == 0;
                if ((format.HasValue && format.Value == StorageType.Xbim) || (!format.HasValue && xbimTarget))
                {
                    var fullSourcePath = Path.GetFullPath(esentModel.DatabaseName);
                    var fullTargetPath = Path.GetFullPath(fileName);
                    if (string.Compare(fullSourcePath, fullTargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                        return; //do nothing it is already saved
                }
            }
            SaveAs(actualFileName, actualFormat, progDelegate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualFileName"></param>
        /// <param name="actualFormat">this will be correctly set</param>
        /// <param name="progDelegate"></param>
        private void SaveAs(string actualFileName, StorageType actualFormat, ReportProgressDelegate progDelegate)
        {
            if (actualFormat.HasFlag(StorageType.Xbim)) //special case for xbim
            {
                var ef = GetFactory(SchemaVersion);
                using (var esentDb = new EsentModel(ef))
                {
                    esentDb.CreateFrom(_model, actualFileName, progDelegate);
                    esentDb.Close();
                }
            }
            else
            {
                using (var fileStream = new FileStream(actualFileName, FileMode.Create, FileAccess.Write))
                {
                    if (actualFormat.HasFlag(StorageType.IfcZip))
                        //do zip first so that xml and ifc are not confused by the combination of flags
                        SaveAsIfcZip(fileStream, Path.GetFileName(actualFileName), actualFormat, progDelegate);
                    else if (actualFormat.HasFlag(StorageType.Ifc))
                        SaveAsIfc(fileStream, progDelegate);
                    else if (actualFormat.HasFlag(StorageType.IfcXml))
                        SaveAsIfcXml(fileStream, progDelegate);

                }
            }
        }

        public void SaveAsIfcXml(Stream stream, ReportProgressDelegate progDelegate = null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                if (_schema == XbimSchemaVersion.Ifc2X3)
                {
                    var writer = new IfcXmlWriter3();
                    writer.Write(_model, xmlWriter, _model.Instances);

                }
                else if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    var writer = new XbimXmlWriter4(XbimXmlSettings.IFC4Add2);
                    var project = _model.Instances.OfType<Ifc4.Kernel.IfcProject>();
                    var products = _model.Instances.OfType<Ifc4.Kernel.IfcObject>();
                    var relations = _model.Instances.OfType<Ifc4.Kernel.IfcRelationship>();

                    var all =
                        new IPersistEntity[] { }
                        //start from root
                            .Concat(project)
                            //add all products not referenced in the project tree
                            .Concat(products)
                            //add all relations which are not inversed
                            .Concat(relations)
                            //make sure all other objects will get written
                            .Concat(_model.Instances);

                    writer.Write(_model, xmlWriter, all);
                }
                xmlWriter.Close();
            }
        }

        public void SaveAsIfc(Stream stream, ReportProgressDelegate progDelegate = null)
        {

            using (TextWriter tw = new StreamWriter(stream))
            {
                Part21Writer.Write(_model, tw, _model.Metadata, null, progDelegate);
                tw.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">The stream will be closed and flushed on return</param>
        /// <param name="zipEntryName">The name of the file zipped inside the file</param>
        /// <param name="storageType">Specify IfcZip and then either IfcXml or Ifc</param>
        /// <param name="progDelegate"></param>
        public void SaveAsIfcZip(Stream stream, string zipEntryName, StorageType storageType, ReportProgressDelegate progDelegate = null)
        {
            Debug.Assert(storageType.HasFlag(StorageType.IfcZip));
            var fileBody = Path.ChangeExtension(zipEntryName,
                storageType.HasFlag(StorageType.IfcXml) ? "ifcXml" : "ifc"
                );

            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                var newEntry = zipStream.CreateEntry(fileBody);
                using (var writer = newEntry.Open())
                {

                    if (storageType.HasFlag(StorageType.IfcXml))
                        SaveAsIfcXml(writer, progDelegate);
                    else //assume it is Ifc
                        SaveAsIfc(writer, progDelegate);
                }

            }
        }

        /// <summary>
        /// If translation is defined, returns matrix translated by the vector
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <param name="translation">Translation</param>
        /// <returns>Translated matrix</returns>
        private XbimMatrix3D Translate(XbimMatrix3D matrix, IVector3D translation)
        {
            if (translation == null) return matrix;
            var translationMatrix = XbimMatrix3D.CreateTranslation(translation.X, translation.Y, translation.Z);
            return XbimMatrix3D.Multiply(matrix, translationMatrix);
        }

        /// <summary>
        /// This function is used to generate the .wexbim model files.
        /// </summary>
        /// <param name="binaryStream">An open writable streamer.</param>
        /// <param name="products">Optional products to be written to the wexBIM file. If null, all products from the model will be saved</param>
        public void SaveAsWexBim(BinaryWriter binaryStream, IEnumerable<IIfcProduct> products = null, IVector3D translation = null)
        {
            products = products ?? Instances.OfType<IIfcProduct>();
            // ReSharper disable RedundantCast
            if (GeometryStore == null) throw new XbimException("Geometry store has not been initialised");
            // ReSharper disable once CollectionNeverUpdated.Local
            var colourMap = new XbimColourMap();
            using (var geomRead = GeometryStore.BeginRead())
            {

                var lookup = geomRead.ShapeGeometries;
                var styles = geomRead.StyleIds;
                var regions = geomRead.ContextRegions.SelectMany(r => r).ToList();
                //we need to get all the default styles for various products
                var defaultStyles = geomRead.ShapeInstances.Select(i => -(int)i.IfcTypeId).Distinct();
                var allStyles = defaultStyles.Concat(styles).ToList();
                int numberOfGeometries = 0;
                int numberOfVertices = 0;
                int numberOfTriangles = 0;
                int numberOfMatrices = 0;
                int numberOfProducts = 0;
                int numberOfStyles = allStyles.Count;
                //start writing out

                binaryStream.Write((Int32)WexBimId); //magic number

                binaryStream.Write((byte)2); //version of stream, arrays now packed as doubles
                var start = (int)binaryStream.Seek(0, SeekOrigin.Current);
                binaryStream.Write((Int32)0); //number of shapes
                binaryStream.Write((Int32)0); //number of vertices
                binaryStream.Write((Int32)0); //number of triangles
                binaryStream.Write((Int32)0); //number of matrices
                binaryStream.Write((Int32)0); //number of products
                binaryStream.Write((Int32)numberOfStyles); //number of styles
                binaryStream.Write(Convert.ToSingle(_model.ModelFactors.OneMetre));
                //write out conversion to meter factor

                binaryStream.Write(Convert.ToInt16(regions.Count)); //write out the population data
                var t = XbimMatrix3D.Identity;
                t = Translate(t, translation);
                foreach (var r in regions)
                {
                    binaryStream.Write((Int32)(r.Population));
                    var bounds = r.ToXbimRect3D();
                    var centre = t.Transform(r.Centre);
                    //write out the centre of the region
                    binaryStream.Write((Single)centre.X);
                    binaryStream.Write((Single)centre.Y);
                    binaryStream.Write((Single)centre.Z);
                    //bounding box of largest region
                    binaryStream.Write(bounds.ToFloatArray());
                }
                //textures

                foreach (var styleId in allStyles)
                {
                    XbimColour colour;
                    if (styleId > 0)
                    {
                        var ss = (IIfcSurfaceStyle)Instances[styleId];
                        var texture = XbimTexture.Create(ss);
                        colour = texture.ColourMap.FirstOrDefault();
                    }
                    else //use the default in the colour map for the enetity type
                    {
                        var theType = _model.Metadata.GetType((short)Math.Abs(styleId));
                        colour = colourMap[theType.Name];
                    }
                    if (colour == null) colour = XbimColour.DefaultColour;
                    binaryStream.Write((Int32)styleId); //style ID                       
                    binaryStream.Write((Single)colour.Red);
                    binaryStream.Write((Single)colour.Green);
                    binaryStream.Write((Single)colour.Blue);
                    binaryStream.Write((Single)colour.Alpha);

                }

                //write out all the product bounding boxes
                var prodIds = new HashSet<int>();
                foreach (var product in products)
                {
                    if (product is IIfcFeatureElement) continue;
                    prodIds.Add(product.EntityLabel);

                    var bb = XbimRect3D.Empty;
                    foreach (var si in geomRead.ShapeInstancesOfEntity(product))
                    {
                        var transformation = Translate(si.Transformation, translation);
                        var bbPart = XbimRect3D.TransformBy(si.BoundingBox, transformation);
                        //make sure we put the box in the right place and then convert to axis aligned
                        if (bb.IsEmpty) bb = bbPart;
                        else
                            bb.Union(bbPart);
                    }
                    //do not write out anything with no geometry
                    if (bb.IsEmpty) continue;

                    binaryStream.Write((Int32)product.EntityLabel);
                    binaryStream.Write((UInt16)_model.Metadata.ExpressTypeId(product));
                    binaryStream.Write(bb.ToFloatArray());
                    numberOfProducts++;
                }

                //projections and openings have already been applied, 

                var toIgnore = new short[4];
                toIgnore[0] = _model.Metadata.ExpressTypeId("IFCOPENINGELEMENT");
                toIgnore[1] = _model.Metadata.ExpressTypeId("IFCPROJECTIONELEMENT");
                if (SchemaVersion == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    toIgnore[2] = _model.Metadata.ExpressTypeId("IFCVOIDINGFEATURE");
                    toIgnore[3] = _model.Metadata.ExpressTypeId("IFCSURFACEFEATURE");
                }

                foreach (var geometry in lookup)
                {
                    if (geometry.ShapeData.Length <= 0) //no geometry to display so don't write out any products for it
                        continue;
                    var instances = geomRead.ShapeInstancesOfGeometry(geometry.ShapeLabel);



                    var xbimShapeInstances = instances.Where(si => !toIgnore.Contains(si.IfcTypeId) &&
                                                                 si.RepresentationType ==
                                                                 XbimGeometryRepresentationType
                                                                     .OpeningsAndAdditionsIncluded && prodIds.Contains(si.IfcProductLabel)).ToList();
                    if (!xbimShapeInstances.Any()) continue;
                    numberOfGeometries++;
                    binaryStream.Write(xbimShapeInstances.Count); //the number of repetitions of the geometry
                    if (xbimShapeInstances.Count > 1)
                    {
                        foreach (IXbimShapeInstanceData xbimShapeInstance in xbimShapeInstances)
                        //write out each of the ids style and transforms
                        {
                            binaryStream.Write(xbimShapeInstance.IfcProductLabel);
                            binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                            binaryStream.Write((UInt32)xbimShapeInstance.InstanceLabel);
                            binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                                ? xbimShapeInstance.StyleLabel
                                : xbimShapeInstance.IfcTypeId * -1);

                            var transformation = Translate(XbimMatrix3D.FromArray(xbimShapeInstance.Transformation), translation);
                            binaryStream.Write(transformation.ToArray());
                            numberOfTriangles +=
                                XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                            numberOfMatrices++;
                        }
                        numberOfVertices +=
                            XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        // binaryStream.Write(geometry.ShapeData);
                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();

                        tr.Write(binaryStream);
                    }
                    else //now do the single instances
                    {
                        var xbimShapeInstance = xbimShapeInstances[0];

                        // IXbimShapeGeometryData geometry = ShapeGeometry(kv.Key);
                        binaryStream.Write((Int32)xbimShapeInstance.IfcProductLabel);
                        binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                        binaryStream.Write((Int32)xbimShapeInstance.InstanceLabel);
                        binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                            ? xbimShapeInstance.StyleLabel
                            : xbimShapeInstance.IfcTypeId * -1);

                        //Read all vertices and normals in the geometry stream and transform

                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();
                        var transformation = Translate(xbimShapeInstance.Transformation, translation);
                        var trTransformed = tr.Transform(transformation);
                        trTransformed.Write(binaryStream);
                        numberOfTriangles += XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        numberOfVertices += XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                    }
                }


                binaryStream.Seek(start, SeekOrigin.Begin);
                binaryStream.Write((Int32)numberOfGeometries);
                binaryStream.Write((Int32)numberOfVertices);
                binaryStream.Write((Int32)numberOfTriangles);
                binaryStream.Write((Int32)numberOfMatrices);
                binaryStream.Write((Int32)numberOfProducts);
                binaryStream.Seek(0, SeekOrigin.End); //go back to end
                // ReSharper restore RedundantCast
            }
        }

        public const int WexBimId = 94132117;

        /// <summary>
        /// Calculates and sets the model factors, call every time a unit of measurement is changed
        /// </summary>
        public void CalculateModelFactors()
        {
            double angleToRadiansConversionFactor = 1; //assume radians
            double lengthToMetresConversionFactor = 1; //assume metres
            var instOfType = Instances.OfType<IIfcUnitAssignment>();
            var ua = instOfType.FirstOrDefault();
            if (ua != null)
            {
                foreach (var unit in ua.Units)
                {
                    var value = 1.0;
                    var cbUnit = unit as IIfcConversionBasedUnit;
                    var siUnit = unit as IIfcSIUnit;
                    if (cbUnit != null)
                    {
                        var mu = cbUnit.ConversionFactor;
                        var component = mu.UnitComponent as IIfcSIUnit;
                        if (component != null)
                            siUnit = component;
                        var et = ((IExpressValueType)mu.ValueComponent);

                        if (et.UnderlyingSystemType == typeof(double))
                            value *= (double)et.Value;
                        else if (et.UnderlyingSystemType == typeof(int))
                            value *= (int)et.Value;
                        else if (et.UnderlyingSystemType == typeof(long))
                            value *= (long)et.Value;
                    }
                    if (siUnit == null) continue;
                    value *= siUnit.Power;
                    switch (siUnit.UnitType)
                    {
                        case IfcUnitEnum.LENGTHUNIT:
                            lengthToMetresConversionFactor = value;
                            break;
                        case IfcUnitEnum.PLANEANGLEUNIT:
                            angleToRadiansConversionFactor = value;
                            //need to guarantee precision to avoid errors in boolean operations
                            if (Math.Abs(angleToRadiansConversionFactor - (Math.PI / 180)) < 1e-9)
                                angleToRadiansConversionFactor = Math.PI / 180;
                            break;
                    }
                }
            }

            var gcs =
                Instances.OfType<IIfcGeometricRepresentationContext>();
            var defaultPrecision = 1e-5;
            //get the Model precision if it is correctly defined
            foreach (var gc in gcs.Where(g => !(g is IIfcGeometricRepresentationSubContext)))
            {
                if (!gc.ContextType.HasValue || string.Compare(gc.ContextType.Value, "model", true) != 0) continue;
                if (!gc.Precision.HasValue) continue;
                if (gc.Precision == 0) continue;
                defaultPrecision = gc.Precision.Value;
                break;
            }
            //sort out precision, esp for some legacy models
            if (defaultPrecision < 1e-7) //sometimes found in old revit models where the precision should really be 1e-5
                defaultPrecision = 1e-5;
            //check if angle units are incorrectly defined, this happens in some old models
            if (Math.Abs(angleToRadiansConversionFactor - 1) < 1e-10)
            {
                var trimmed = Instances.Where<IIfcTrimmedCurve>(trimmedCurve => trimmedCurve.BasisCurve is IIfcConic);
                foreach (var trimmedCurve in trimmed)
                {
                    if (trimmedCurve.MasterRepresentation != IfcTrimmingPreference.PARAMETER)
                        continue;
                    if (
                        !trimmedCurve.Trim1.Concat(trimmedCurve.Trim2)
                            .OfType<IfcParameterValue>()
                            .Select(trim => (double)trim.Value)
                            .Any(val => val > Math.PI * 2)) continue;
                    angleToRadiansConversionFactor = Math.PI / 180;
                    break;
                }
            }

            ModelFactors.Initialise(angleToRadiansConversionFactor, lengthToMetresConversionFactor,
                defaultPrecision);

            SetWorkArounds();
        }

        /// <summary>
        /// Code to determine model specific workarounds (BIM tool IFC exporter quirks)
        /// </summary>
        private void SetWorkArounds()
        {
            //try Revit first
            string revitPattern = @"- Exporter\s(\d*.\d*.\d*.\d*)";
            if (Header.FileName == null || string.IsNullOrWhiteSpace(Header.FileName.OriginatingSystem))
                return; //nothing to do
            var matches = Regex.Matches(Header.FileName.OriginatingSystem, revitPattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0) //looks like Revit
            {
                if (matches[0].Groups.Count == 2) //we have the build versions
                {
                    if (Version.TryParse(matches[0].Groups[1].Value, out Version modelVersion))
                    {
                        //SurfaceOfLinearExtrusion bug found in version 17.2.0 and earlier
                        var surfaceOfLinearExtrusionVersion = new Version(17, 2, 0, 0);
                        if (modelVersion <= surfaceOfLinearExtrusionVersion)
                            ((XbimModelFactors)ModelFactors).AddWorkAround("#SurfaceOfLinearExtrusion");
                    }

                }
            }
        }

        #region Referenced Models functions

        /// <summary>
        /// Adds a model as a reference or federated model, do not call inside a transaction
        /// </summary>
        /// <param name="refModelPath"></param>
        /// <param name="organisationName"></param>
        /// <param name="organisationRole"></param>
        /// <returns></returns>
        public XbimReferencedModel AddModelReference(string refModelPath, string organisationName, string organisationRole)
        {
            XbimReferencedModel retVal;
            using (var txn = BeginTransaction())
            {
                if (_schema == XbimSchemaVersion.Ifc4 || _schema == XbimSchemaVersion.Ifc4x1)
                {
                    var role = Instances.New<Ifc4.ActorResource.IfcActorRole>();
                    role.RoleString = organisationRole;
                    // the string is converted appropriately by the IfcActorRoleClass
                    var org = Instances.New<Ifc4.ActorResource.IfcOrganization>();
                    org.Name = organisationName;
                    org.Roles.Add(role);
                    retVal = AddModelReference(refModelPath, org);
                }
                else
                {
                    var role = Instances.New<Ifc2x3.ActorResource.IfcActorRole>();
                    role.RoleString = organisationRole;
                    // the string is converted appropriately by the IfcActorRoleClass
                    var org = Instances.New<Ifc2x3.ActorResource.IfcOrganization>();
                    org.Name = organisationName;
                    org.Roles.Add(role);
                    retVal = AddModelReference(refModelPath, org);
                }
                txn.Commit();
            }
            return retVal;
        }

        /// <summary>
        /// adds a model as a reference model can be called inside a transaction
        /// </summary>
        /// <param name="refModelPath">the file path of the xbim model to reference, this must be an xbim file</param>
        /// <param name="owner">the actor who supplied the model</param>
        /// <returns></returns>
        private XbimReferencedModel AddModelReference(string refModelPath, Ifc2x3.ActorResource.IfcOrganization owner)
        {
            XbimReferencedModel retVal;
            if (CurrentTransaction == null)
            {
                using (var txn = BeginTransaction())
                {
                    var docInfo = Instances.New<Ifc2x3.ExternalReferenceResource.IfcDocumentInformation>();
                    docInfo.DocumentId = new Ifc2x3.MeasureResource.IfcIdentifier(_referencedModels.NextIdentifer());
                    docInfo.Name = refModelPath;
                    docInfo.DocumentOwner = owner;
                    docInfo.IntendedUse = RefDocument;
                    retVal = new XbimReferencedModel(docInfo);
                    AddModelReference(retVal);
                    txn.Commit();
                }
            }
            else
            {
                var docInfo = Instances.New<Ifc2x3.ExternalReferenceResource.IfcDocumentInformation>();
                docInfo.DocumentId = new Ifc2x3.MeasureResource.IfcIdentifier(_referencedModels.NextIdentifer());
                docInfo.Name = refModelPath;
                docInfo.DocumentOwner = owner;
                docInfo.IntendedUse = RefDocument;
                retVal = new XbimReferencedModel(docInfo);
                AddModelReference(retVal);
            }
            return retVal;
        }

        /// <summary>
        /// adds a model as a reference model can be called inside a transaction
        /// </summary>
        /// <param name="refModelPath">the file path of the xbim model to reference, this must be an xbim file</param>
        /// <param name="owner">the actor who supplied the model</param>
        /// <returns></returns>
        private XbimReferencedModel AddModelReference(string refModelPath, Ifc4.ActorResource.IfcOrganization owner)
        {
            XbimReferencedModel retVal;
            if (CurrentTransaction == null)
            {
                using (var txn = BeginTransaction())
                {
                    var docInfo = Instances.New<Ifc4.ExternalReferenceResource.IfcDocumentInformation>();
                    docInfo.Identification = new IfcIdentifier(_referencedModels.NextIdentifer());
                    docInfo.Name = refModelPath;
                    docInfo.DocumentOwner = owner;
                    docInfo.IntendedUse = RefDocument;
                    retVal = new XbimReferencedModel(docInfo);
                    AddModelReference(retVal);
                    txn.Commit();
                }
            }
            else
            {
                var docInfo = Instances.New<Ifc4.ExternalReferenceResource.IfcDocumentInformation>();
                docInfo.Identification = new IfcIdentifier(_referencedModels.NextIdentifer());
                docInfo.Name = refModelPath;
                docInfo.DocumentOwner = owner;
                docInfo.IntendedUse = RefDocument;
                retVal = new XbimReferencedModel(docInfo);
                AddModelReference(retVal);
            }
            return retVal;
        }

        /// <summary>
        /// All reference models are opened in a readonly mode.
        /// Their children reference models is invoked iteratively.
        /// 
        /// Loading referenced models defaults to avoiding Exception on file not found; in this way the federated model can still be opened and the error rectified.
        /// </summary>
        /// <param name="throwErrorOnReferenceModelExceptions">Set to true to enable your own error handling</param>
        private void LoadReferenceModels(bool throwErrorOnReferenceModelExceptions = false)
        {
            var docInfos = Instances.OfType<IIfcDocumentInformation>().Where(d => d.IntendedUse == RefDocument);
            foreach (var docInfo in docInfos)
            {
                if (throwErrorOnReferenceModelExceptions)
                {
                    // throw exception on referenceModel Creation
                    AddModelReference(new XbimReferencedModel(docInfo));
                }
                else
                {
                    // do not throw exception on referenceModel Creation
                    try
                    {
                        AddModelReference(new XbimReferencedModel(docInfo));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(
                            string.Format("Ignored exception on modelreference load for #{0}.", docInfo.EntityLabel),
                            ex);
                    }
                }
            }
        }

        #endregion
        #region Federation

        public IEnumerable<IReferencedModel> ReferencedModels
        {
            get { return _referencedModels.AsEnumerable(); }
        }

        public void AddModelReference(IReferencedModel model)
        {
            _referencedModels.Add(model);
        }

        /// <summary>
        /// Returns true if the model contains reference models or the model has extension xBIMf
        /// </summary>
        public virtual bool IsFederation
        {
            get { return _referencedModels.Any(); }
        }

        ///// <summary>
        ///// Returns an enumerable of the handles to all entities in the model
        ///// Note this includes entities that are in any federated models
        ///// </summary>
        //public IEnumerable<XbimInstanceHandle> AllInstancesHandles
        //{
        //    get
        //    {
        //        foreach (var h in InstanceHandles)
        //            yield return h;
        //        foreach (var refModel in ReferencedModels.Where(r => r.Model is EsentModel).Select(r => r.Model as EsentModel))
        //            foreach (var h in refModel.AllInstancesHandles)
        //                yield return h;
        //    }
        //}

        public void EnsureUniqueUserDefinedId()
        {
            short iId = 0;
            var allModels =
                (new[] { this }).Concat(ReferencedModels.Select(rm => rm.Model));
            foreach (var model in allModels)
            {
                model.UserDefinedId = iId++;
            }
        }
        #endregion

        public IModel ReferencingModel
        {
            get { return _model; }
        }

        public IReadOnlyEntityCollection FederatedInstances
        {
            get { return new FederatedModelInstances(this); }
        }

        public IList<XbimInstanceHandle> FederatedInstanceHandles
        {
            get
            {
                var allModels = ReferencedModels.Select(r => r.Model).Concat(new[] { this });
                return allModels.SelectMany(m => m.InstanceHandles).ToList();
            }
        }

        /// <summary>
        /// Returns a list of the handles to only the entities in this model
        /// Note this do NOT include entities that are in any federated models
        /// </summary>

        public IList<XbimInstanceHandle> InstanceHandles
        {
            get { return _model.InstanceHandles.ToList(); }
        }

        public ILogger Logger { get => _model.Logger; set => _model.Logger = value; }

        public IEntityCache EntityCache => _model.EntityCache;

        XbimSchemaVersion SchemaVersion => _model.SchemaVersion;

        XbimSchemaVersion IModel.SchemaVersion => throw new NotImplementedException();

        #region Insert products with context

        private List<IIfcProduct> _primaryElements = new List<IIfcProduct>();
        private readonly List<IIfcProduct> _decomposition = new List<IIfcProduct>();
        private bool _includeGeometry;

        /// <summary>
        /// This is a higher level function which uses InsertCopy function alongside with the knowledge of IFC schema to copy over
        /// products with their types and other related information (classification, aggregation, documents, properties) and optionally
        /// geometry. It will also bring in spatial hierarchy relevant to selected products. However, resulting model is not guaranteed 
        /// to be compliant with any Model View Definition unless you explicitly check the compliance. Context of a single product tend to 
        /// consist from hundreds of objects which need to be identified and copied over so this operation might be potentially expensive.
        /// You should never call this function more than once between two models. It not only selects objects to be copied over but also
        /// excludes other objects from being coppied over so that it doesn't bring the entire model in a chain dependencies. This means
        /// that some objects are modified (like spatial relations) and won't get updated which would lead to an inconsistent copy.
        /// </summary>
        /// <param name="products">Products from other model to be inserted into this model</param>
        /// <param name="includeGeometry">If TRUE, geometry of the products will be copied over.</param>
        /// <param name="keepLabels">If TRUE, entity labels from original model will be used. Always set this to FALSE
        /// if you are going to insert products from multiple source models or if you are going to insert products to a non-empty model</param>
        /// <param name="mappings">Mappings to avoid multiple insertion of objects. Keep a single instance for insertion between two models.
        /// If you also use InsertCopy() function for some other insertions, use the same instance of mappings.</param>
        public void InsertCopy(IEnumerable<IIfcProduct> products, bool includeGeometry, bool keepLabels, XbimInstanceHandleMap mappings)
        {
            _primaryElements.Clear();
            _decomposition.Clear();
            _includeGeometry = includeGeometry;

            var roots = products.Cast<IPersistEntity>().ToList();
            //return if there is nothing to insert
            if (!roots.Any())
                return;

            var source = roots.First().Model;
            if (source == this)
                //don't do anything if the source and target are the same
                return;

            var toInsert = GetEntitiesToInsert(source, roots);
            //create new cache is none is defined
            var cache = mappings ?? new XbimInstanceHandleMap(source, this);

            foreach (var entity in toInsert)
                InsertCopy(entity, cache, Filter, true, keepLabels);
        }

        private IEnumerable<IPersistEntity> GetEntitiesToInsert(IModel model, List<IPersistEntity> roots)
        {
            _primaryElements = roots.OfType<IIfcProduct>().ToList();

            //add any aggregated elements. For example IfcRoof is typically aggregation of one or more slabs so we need to bring
            //them along to have all the information both for geometry and for properties and materials.
            //This has to happen before we add spatial hierarchy or it would bring in full hierarchy which is not an intention
            var decompositionRels = GetAggregations(_primaryElements.ToList(), model).ToList();
            _primaryElements.AddRange(_decomposition);
            roots.AddRange(decompositionRels);

            //we should add spatial hierarchy right here so it brings its attributes as well
            var spatialRels = model.Instances.Where<IIfcRelContainedInSpatialStructure>(
                r => _primaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var spatialRefs =
                model.Instances.Where<IIfcRelReferencedInSpatialStructure>(
                    r => _primaryElements.Any(e => r.RelatedElements.Contains(e))).ToList();
            var bottomSpatialHierarchy =
                spatialRels.Select(r => r.RelatingStructure).Union(spatialRefs.Select(r => r.RelatingStructure)).ToList();
            var spatialAggregations = GetUpstreamHierarchy(bottomSpatialHierarchy, model).ToList();

            //add all spatial elements from bottom and from upstream hierarchy
            _primaryElements.AddRange(bottomSpatialHierarchy);
            _primaryElements.AddRange(spatialAggregations.Select(r => r.RelatingObject).OfType<IIfcProduct>());
            roots.AddRange(spatialAggregations);
            roots.AddRange(spatialRels);
            roots.AddRange(spatialRefs);

            //we should add any feature elements used to subtract mass from a product
            var featureRels = GetFeatureRelations(_primaryElements).ToList();
            var openings = featureRels.Select(r => r.RelatedOpeningElement);
            _primaryElements.AddRange(openings);
            roots.AddRange(featureRels);

            //object types and properties for all primary products (elements and spatial elements)
            roots.AddRange(_primaryElements.SelectMany(p => p.IsDefinedBy));
            roots.AddRange(_primaryElements.SelectMany(p => p.IsTypedBy));



            //assignmnet to groups will bring in all system aggregarions if defined in the file
            roots.AddRange(_primaryElements.SelectMany(p => p.HasAssignments));

            //associations with classification, material and documents
            roots.AddRange(_primaryElements.SelectMany(p => p.HasAssociations));

            return roots;
        }

        private object Filter(ExpressMetaProperty property, object parentObject)
        {
            //ignore inverses except for style
            if (property.IsInverse)
                return property.Name == "StyledByItem" ? property.PropertyInfo.GetValue(parentObject, null) : null;

            if (_primaryElements != null && _primaryElements.Any())
            {
                if (typeof(IIfcProduct).IsAssignableFrom(property.PropertyInfo.PropertyType))
                {
                    var element = property.PropertyInfo.GetValue(parentObject, null) as IIfcProduct;
                    if (element != null && _primaryElements.Contains(element))
                        return element;
                    return null;
                }
                if (property.EnumerableType != null && !property.EnumerableType.IsValueType && property.EnumerableType != typeof(string))
                {
                    //this can either be a list of IPersistEntity or select type. The very base type is IPersist
                    var entities = property.PropertyInfo.GetValue(parentObject, null) as IEnumerable<IPersist>;
                    if (entities != null)
                    {
                        var persistEntities = entities as IList<IPersist> ?? entities.ToList();
                        var elementsToRemove = persistEntities.OfType<IIfcProduct>().Where(e => !_primaryElements.Contains(e)).ToList();
                        //if there are no IfcElements return what is in there with no care
                        if (elementsToRemove.Any())
                            //return original values excluding elements not included in the primary set
                            return persistEntities.Except(elementsToRemove).ToList();
                    }
                }
            }

            //if geometry is to be included don't filter it out
            if (_includeGeometry)
                return property.PropertyInfo.GetValue(parentObject, null);

            //leave out geometry and placement of products
            if (parentObject is IIfcProduct &&
                (property.PropertyInfo.Name == "Representation" || property.PropertyInfo.Name == "ObjectPlacement")
                )
                return null;

            //leave out representation maps
            if (parentObject is IIfcTypeProduct && property.PropertyInfo.Name == "RepresentationMaps")
                return null;

            //leave out eventual connection geometry
            if (parentObject is IIfcRelSpaceBoundary && property.PropertyInfo.Name == "ConnectionGeometry")
                return null;

            //return the value for anything else
            return property.PropertyInfo.GetValue(parentObject, null);
        }

        private static IEnumerable<IIfcRelVoidsElement> GetFeatureRelations(IEnumerable<IIfcProduct> products)
        {
            var elements = products.OfType<IIfcElement>().ToList();
            if (!elements.Any()) yield break;
            var model = elements.First().Model;
            var rels = model.Instances.Where<IIfcRelVoidsElement>(r => elements.Any(e => Equals(e, r.RelatingBuildingElement)));
            foreach (var rel in rels)
                yield return rel;
        }

        private IEnumerable<IIfcRelDecomposes> GetAggregations(List<IIfcProduct> products, IModel model)
        {
            _decomposition.Clear();
            while (true)
            {
                if (!products.Any())
                    yield break;

                var products1 = products;
                var rels = model.Instances.Where<IIfcRelDecomposes>(r =>
                {
                    var aggr = r as IIfcRelAggregates;
                    if (aggr != null)
                        return products1.Any(p => Equals(aggr.RelatingObject, p));
                    var nest = r as IIfcRelNests;
                    if (nest != null)
                        return products1.Any(p => Equals(nest.RelatingObject, p));
                    var prj = r as IIfcRelProjectsElement;
                    if (prj != null)
                        return products1.Any(p => Equals(prj.RelatingElement, p));
                    var voids = r as IIfcRelVoidsElement;
                    if (voids != null)
                        return products1.Any(p => Equals(voids.RelatingBuildingElement, p));
                    return false;

                }).ToList();
                var relatedProducts = rels.SelectMany(r =>
                {
                    var aggr = r as IIfcRelAggregates;
                    if (aggr != null)
                        return aggr.RelatedObjects.OfType<IIfcProduct>();
                    var nest = r as IIfcRelNests;
                    if (nest != null)
                        return nest.RelatedObjects.OfType<IIfcProduct>();
                    var prj = r as IIfcRelProjectsElement;
                    if (prj != null)
                        return new IIfcProduct[] { prj.RelatedFeatureElement };
                    var voids = r as IIfcRelVoidsElement;
                    if (voids != null)
                        return new IIfcProduct[] { voids.RelatedOpeningElement };
                    return null;
                }).Where(p => p != null).ToList();

                foreach (var rel in rels)
                    yield return rel;

                products = relatedProducts;
                _decomposition.AddRange(products);
            }
        }

        private static IEnumerable<IIfcRelAggregates> GetUpstreamHierarchy(IEnumerable<IIfcSpatialElement> spatialStructureElements, IModel model)
        {
            while (true)
            {
                var elements = spatialStructureElements.ToList();
                if (!elements.Any())
                    yield break;

                var rels = model.Instances.Where<IIfcRelAggregates>(r => elements.Any(s => r.RelatedObjects.Contains(s))).ToList();
                var decomposing = rels.Select(r => r.RelatingObject).OfType<IIfcSpatialStructureElement>();

                foreach (var rel in rels)
                    yield return rel;

                spatialStructureElements = decomposing;
            }
        }
        #endregion

        #region Equality

        /// <summary>
        /// Returns true if it is another reference to this or if is is an embeded model
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IModel other)
        {
            return ReferenceEquals(this, other) || ReferenceEquals(other, _model);
        }

        /// <summary>
        /// Returns true if it is another reference to this or if is is an embeded model
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return _model.Equals(obj) || ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return _model.GetHashCode();
        }

        public IInverseCache BeginInverseCaching()
        {
            return _model.BeginInverseCaching();
        }

        public IEntityCache BeginEntityCaching()
        {
            return _model.BeginEntityCaching();
        }

        public static bool operator ==(IfcStore store, IModel model)
        {
            if (ReferenceEquals(store, model))
                return true;
            if (ReferenceEquals(store, null))
                return false;
            if (ReferenceEquals(model, null))
                return false;

            return store._model.Equals(model);
        }

        public static bool operator !=(IfcStore store, IModel model)
        {
            return !(store == model);
        }

        #endregion
    }
}
