using System;
using System.Collections.Generic;
using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{
    public class IfcStore : IModel, IDisposable
    {
        private readonly IModel _model;
        private readonly IfcSchemaVersion _schema;
         
        private readonly bool _deleteModelOnClose;
        private readonly string _xbimFileName;
        public event NewEntityHandler EntityNew;
        public event ModifiedEntityHandler EntityModified;
        public event DeletedEntityHandler EntityDeleted;
        private bool _disposed;
        /// <summary>
        /// The default largest size in MB for an ifc file to be loaded into memory, above this size the store will choose to use the database storage media to mimise the memory footprint. This size can be set in the config file or in the open statement of this store 
        /// </summary>
        public static double DefaultIfcDatabaseSizeThreshHold = 30; //default size set to 30MB
        private IIfcOwnerHistory _ownerHistoryAddObject;
        private IIfcOwnerHistory _ownerHistoryModifyObject;

        private IIfcPersonAndOrganization _defaultOwningUser;
        private IIfcApplication _defaultOwningApplication;
        private readonly XbimEditorCredentials _editorDetails;


        protected IfcStore(IModel iModel, IfcSchemaVersion schema, XbimEditorCredentials editorDetails, string fileName = null, string xbimFileName = null,
            bool deleteOnClose = false)
        {
            _model = iModel;
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
            else _editorDetails = editorDetails;


            if (schema == IfcSchemaVersion.Ifc4)
            {
                _model.EntityNew += IfcRootInitIfc4;
                _model.EntityModified += IfcRootModifiedIfc4;
            }
            else //its 2x3
            {
                _model.EntityNew += IfcRootInitIfc2X3;
                _model.EntityModified += IfcRootModifiedIfc2X3;
            }

        }


        //public static IfcStore Open( Stream inputStream, XbimStorageType storageType, string xbimDbName, XbimDBAccess accessMode = XbimDBAccess.Read, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null)
        //{
        //    var ifcVersion = GetIfcSchemaVersion(inputStream);
        //    if (ifcVersion == IfcSchemaVersion.Unsupported)
        //        throw new FileLoadException(filePath + " is not a valid Ifc file format, ifc, ifcxml, ifczip and xBIM are supported");
        //    if (storageType == XbimStorageType.Xbim) //open the XbimFile
        //    {
        //        if (ifcVersion == IfcSchemaVersion.Ifc4)
        //        {
        //            var model = new EsentModel(new Ifc4.EntityFactory());
        //            model.Open(inputStream, accessMode, progDelegate);
        //            return new IfcStore(model);
        //        }
        //        else //it will be Ifc2x3
        //        {
        //            var model = new EsentModel(new Ifc2x3.EntityFactory());
        //            model.Open(inputStream, accessMode, progDelegate);
        //            return new IfcStore(model);
        //        }
        //    }
        //    else //it will be an Ifc file if we are at this point
        //    {
        //        var fInfo = new FileInfo(path);
        //        double ifcMaxLength = (ifcDatabaseSizeThreshHold ?? DefaultIfcDatabaseSizeThreshHold) * 1024 * 1024;
        //        if (fInfo.Length > ifcMaxLength) //we need to make an esent database
        //        {
        //            var tmpFileName = Path.GetTempFileName();
        //            if (ifcVersion == IfcSchemaVersion.Ifc4)
        //            {
        //                var model = new EsentModel(new Ifc4.EntityFactory());
        //                model.CreateFrom(path, tmpFileName, progDelegate, true);
        //                return new IfcStore(model);
        //            }
        //            else //it will be Ifc2x3
        //            {
        //                var model = new EsentModel(new Ifc2x3.EntityFactory());
        //                model.CreateFrom(path, tmpFileName, progDelegate, true);
        //                return new IfcStore(model);
        //            }
        //        }
        //        else //we can use a memory model
        //        {
        //            if (ifcVersion == IfcSchemaVersion.Ifc4)
        //            {
        //                var model = new MemoryModel<Ifc4.EntityFactory>();
        //                model.Open(path, progDelegate);
        //                return new IfcStore(model);
        //            }
        //            else //it will be Ifc2x3
        //            {
        //                var model = new MemoryModel<Ifc2x3.EntityFactory>();
        //                model.Open(path, progDelegate);
        //                return new IfcStore(model);
        //            }
        //        }
        //    }
        //}




        /// <summary>
        /// Opens an Ifc file, Ifcxml, IfcZip, xbim
        /// </summary>
        /// <param name="path">the file name of the ifc, ifczip, ifcxml or xbim file to be opened</param>
        /// <param name="accessMode">Read/write access mode for the Store</param>
        /// <param name="editorDetails">This is only required if the store is opened for editing</param>
        /// <param name="ifcDatabaseSizeThreshHold">if not defined the DefaultIfcDatabaseSizeThreshHold is used, Ifc files below this size will be opened in memory, above this size a database will be created. If -1 is specified a database will be created for all Ifc files that are opened. Xbim files are always opened as databases</param>
        /// <param name="progDelegate"></param>
        static public IfcStore Open(string path, XbimEditorCredentials editorDetails = null, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null)
        {
            var filePath = Path.GetFullPath(path);
            if (!Directory.Exists(Path.GetDirectoryName(filePath) ?? ""))
                throw new DirectoryNotFoundException(Path.GetDirectoryName(filePath) + " directory was not found");
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath + " file was not found");

            var ifcVersion = GetIfcSchemaVersion(path);
            if (ifcVersion == IfcSchemaVersion.Unsupported)
                throw new FileLoadException(filePath + " is not a valid Ifc file format, ifc, ifcxml, ifczip and xBIM are supported");
            var storageType = IfcStorageType(path);
            if (storageType == XbimStorageType.Xbim) //open the XbimFile
            {

                if (ifcVersion == IfcSchemaVersion.Ifc4)
                {
                    var model = new EsentModel(new Ifc4.EntityFactory());
                    model.Open(path, XbimDBAccess.Read, progDelegate);
                    return new IfcStore(model, ifcVersion, editorDetails, path);
                }
                else //it will be Ifc2x3
                {
                    var model = new EsentModel(new Ifc2x3.EntityFactory());
                    model.Open(path, XbimDBAccess.Read, progDelegate);
                    return new IfcStore(model, ifcVersion, editorDetails, path);
                }
            }
            else //it will be an Ifc file if we are at this point
            {
                var fInfo = new FileInfo(path);
                double ifcMaxLength = (ifcDatabaseSizeThreshHold ?? DefaultIfcDatabaseSizeThreshHold) * 1024 * 1024;
                if (fInfo.Length > ifcMaxLength) //we need to make an esent database
                {
                    var tmpFileName = Path.GetTempFileName();
                    if (ifcVersion == IfcSchemaVersion.Ifc4)
                    {
                        var model = new EsentModel(new Ifc4.EntityFactory());
                        model.CreateFrom(path, tmpFileName, progDelegate, true);
                        return new IfcStore(model, ifcVersion, editorDetails, path, tmpFileName, true);
                    }
                    else //it will be Ifc2x3
                    {
                        var model = new EsentModel(new Ifc2x3.EntityFactory());
                        model.CreateFrom(path, tmpFileName, progDelegate, true);
                        return new IfcStore(model, ifcVersion, editorDetails, path, tmpFileName, true);
                    }
                }
                else //we can use a memory model
                {
                    if (ifcVersion == IfcSchemaVersion.Ifc4)
                    {
                        var model = new MemoryModel<Ifc4.EntityFactory>();
                        model.Open(path, progDelegate);
                        return new IfcStore(model, ifcVersion, editorDetails, path);
                    }
                    else //it will be Ifc2x3
                    {
                        var model = new MemoryModel<Ifc2x3.EntityFactory>();
                        model.Open(path, progDelegate);
                        return new IfcStore(model, ifcVersion, editorDetails, path);
                    }
                }
            }
        }

        public static IfcSchemaVersion GetIfcSchemaVersion(string path)
        {
            var storageType = IfcStorageType(path);
            if (storageType == XbimStorageType.Invalid) return IfcSchemaVersion.Unsupported;
            var stepHeader = storageType == XbimStorageType.Xbim ? EsentModel.GetStepFileHeader(path) : MemoryModel.GetStepFileHeader(path);
            var stepSchema = stepHeader.FileSchema;
            foreach (var schema in stepSchema.Schemas)
            {
                if (String.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return IfcSchemaVersion.Ifc4;
                if (String.Compare(schema, "Ifc2x3", StringComparison.OrdinalIgnoreCase) == 0)
                    return IfcSchemaVersion.Ifc2X3;
            }

            return IfcSchemaVersion.Unsupported;
        }

        public static XbimStorageType IfcStorageType(string path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return XbimStorageType.Invalid;
            ext = ext.ToLowerInvariant();
            if (ext == ".ifc") return XbimStorageType.Ifc;
            if (ext == ".ifcxml") return XbimStorageType.IfcXml;
            if (ext == ".ifczip") return XbimStorageType.IfcZip;
            if (ext == ".xbim") return XbimStorageType.Xbim;
            return XbimStorageType.Invalid;
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


        public bool Activate(IPersistEntity owningEntity, bool write)
        {
            return _model.Activate(owningEntity, write);
        }

        public void Delete(IPersistEntity entity)
        {
            _model.Delete(entity);
        }

        public ITransaction BeginTransaction(string name = null)
        {
            var esentModel = _model as EsentModel;
            if (esentModel != null) //we need to do transaction handling on esent model, make sure we can write to it
            { 
                esentModel.Header.StampXbimApplication(_schema);
                return esentModel.BeginTransaction(name);
            }
            if (_schema == IfcSchemaVersion.Ifc4)
            {
                var memoryModel = _model as MemoryModel<Ifc4.EntityFactory>;
                if (memoryModel != null)
                {
                    memoryModel.Header.StampXbimApplication(_schema);
                    return memoryModel.BeginTransaction(name);                   
                }
            }
            else if (_schema == IfcSchemaVersion.Ifc2X3)
            {
                var memoryModel = _model as MemoryModel<Ifc2x3.EntityFactory>;
                if (memoryModel != null)
                {
                    memoryModel.Header.StampXbimApplication(_schema);
                    return memoryModel.BeginTransaction(name);
                }
            }
            throw new XbimException("Native store does not support transactions");
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


        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            _model.ForEach(source, body);
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
                        var disposeInterface = _model as IDisposable;
                        if (disposeInterface != null) disposeInterface.Dispose();
                    }
                    //unmanaged, mostly esent related                  
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
            Dispose();
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

        static public IfcStore Create(XbimEditorCredentials editorDetails = null, XbimStoreType storageType = XbimStoreType.InMemoryModel, IfcSchemaVersion ifcVersion = IfcSchemaVersion.Ifc4)
        {
            if (storageType == XbimStoreType.EsentDatabase)
            {
                if (ifcVersion == IfcSchemaVersion.Ifc4)
                {
                    var temporaryModel = EsentModel.CreateTemporaryModel(new Ifc4.EntityFactory());
                    return new IfcStore(temporaryModel, ifcVersion, editorDetails, temporaryModel.DatabaseName); //it will delete itself anyway
                }
                else //it will be Ifc2x3
                {
                    var temporaryModel = EsentModel.CreateTemporaryModel(new Ifc2x3.EntityFactory());
                    return new IfcStore(temporaryModel, ifcVersion, editorDetails, temporaryModel.DatabaseName); //it will delete itself anyway
                }
            }
            else //it will be memory model
            {

                if (ifcVersion == IfcSchemaVersion.Ifc4)
                {
                    var memoryModel = new MemoryModel<Ifc4.EntityFactory>();
                    return new IfcStore(memoryModel, ifcVersion, editorDetails);
                }
                else //it will be Ifc2x3
                {
                    var memoryModel = new MemoryModel<Ifc2x3.EntityFactory>();
                    return new IfcStore(memoryModel, ifcVersion, editorDetails);
                }
            }
        }

        #region OwnerHistory Management


        private void IfcRootModifiedIfc2X3(IPersistEntity entity)
        {

            var root = entity as Ifc2x3.Kernel.IfcRoot;

            if (root == null || root.OwnerHistory == (Ifc2x3.UtilityResource.IfcOwnerHistory)_ownerHistoryAddObject)
                return;

            if (root.OwnerHistory != (Ifc2x3.UtilityResource.IfcOwnerHistory)_ownerHistoryModifyObject)
                root.OwnerHistory = (Ifc2x3.UtilityResource.IfcOwnerHistory)OwnerHistoryModifyObject;
        }

        private void IfcRootInitIfc2X3(IPersistEntity entity)
        {
            var root = entity as Ifc2x3.Kernel.IfcRoot;
            if (root != null)
            {
                root.OwnerHistory = (Ifc2x3.UtilityResource.IfcOwnerHistory)OwnerHistoryAddObject;
            }
        }


        private void IfcRootModifiedIfc4(IPersistEntity entity)
        {
            var root = entity as Ifc4.Kernel.IfcRoot;
            if (root == null || root.OwnerHistory == (Ifc4.UtilityResource.IfcOwnerHistory)_ownerHistoryAddObject)
                return;

            if (root.OwnerHistory != (Ifc4.UtilityResource.IfcOwnerHistory)_ownerHistoryModifyObject)
                root.OwnerHistory = (Ifc4.UtilityResource.IfcOwnerHistory)OwnerHistoryModifyObject;
        }

        private void IfcRootInitIfc4(IPersistEntity entity)
        {
            var root = entity as Ifc4.Kernel.IfcRoot;
            if (root != null)
            {
                root.OwnerHistory = (Ifc4.UtilityResource.IfcOwnerHistory)OwnerHistoryAddObject;
            }
        }


        public IIfcPersonAndOrganization DefaultOwningUser
        {
            get
            {
                if (_defaultOwningUser != null) return _defaultOwningUser;
                if (_schema == IfcSchemaVersion.Ifc4)
                {
                    var person = Instances.New<Ifc4.ActorResource.IfcPerson>(p =>
                    {
                        p.GivenName = _editorDetails.EditorsGivenName;
                        p.FamilyName = _editorDetails.EditorsFamilyName;
                    });
                    var organization = Instances.New<Ifc4.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
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
                    var organization = Instances.New<Ifc2x3.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.EditorsOrganisationName);
                    _defaultOwningUser = Instances.New<Ifc2x3.ActorResource.IfcPersonAndOrganization>(po =>
                    {
                        po.TheOrganization = organization;
                        po.ThePerson = person;
                    });
                }
                return _defaultOwningUser;
            }
        }

        public IIfcApplication DefaultOwningApplication
        {
            get
            {
                if (_defaultOwningApplication != null) return _defaultOwningApplication;
                if (_schema == IfcSchemaVersion.Ifc4)
                    return _defaultOwningApplication ??
                         (_defaultOwningApplication =
                             Instances.New<Ifc4.UtilityResource.IfcApplication>(a =>
                             {
                                 a.ApplicationDeveloper = Instances.New<Ifc4.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.ApplicationDevelopersName);
                                 a.ApplicationFullName = _editorDetails.ApplicationFullName;
                                 a.ApplicationIdentifier = _editorDetails.ApplicationIdentifier;
                             }
                ));
                return _defaultOwningApplication ??
                        (_defaultOwningApplication =
                            Instances.New<Ifc2x3.UtilityResource.IfcApplication>(a =>
                            {
                                a.ApplicationDeveloper = Instances.New<Ifc2x3.ActorResource.IfcOrganization>(o => o.Name = _editorDetails.ApplicationDevelopersName);
                                a.ApplicationFullName = _editorDetails.ApplicationFullName;
                                a.ApplicationIdentifier = _editorDetails.ApplicationIdentifier;
                            }
                ));
            }
        }

        public IIfcOwnerHistory OwnerHistoryAddObject
        {
            get
            {
                if (_ownerHistoryAddObject == null)
                {
                    if (_schema == IfcSchemaVersion.Ifc4)
                    {
                        var histAdd = Instances.New<Ifc4.UtilityResource.IfcOwnerHistory>();
                        histAdd.OwningUser = (Ifc4.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                        histAdd.OwningApplication = (Ifc4.UtilityResource.IfcApplication)DefaultOwningApplication;
                        histAdd.ChangeAction = Ifc4.UtilityResource.IfcChangeActionEnum.ADDED;
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
                }
                return _ownerHistoryAddObject;
            }
        }


        internal IIfcOwnerHistory OwnerHistoryModifyObject
        {
            get
            {
                if (_ownerHistoryModifyObject == null)
                {
                    if (_schema == IfcSchemaVersion.Ifc4)
                    {
                        var histmod = Instances.New<Ifc4.UtilityResource.IfcOwnerHistory>();
                        histmod.OwningUser = (Ifc4.ActorResource.IfcPersonAndOrganization)DefaultOwningUser;
                        histmod.OwningApplication = (Ifc4.UtilityResource.IfcApplication)DefaultOwningApplication;
                        histmod.ChangeAction = Ifc4.UtilityResource.IfcChangeActionEnum.MODIFIED;
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
        public void SaveAs(string fileName, XbimStorageType? format = null, ReportProgressDelegate progDelegate = null)
        {
            var esentModel = _model as EsentModel;
            if (esentModel != null) 
                esentModel.SaveAs(fileName,format, progDelegate);
            if (_schema == IfcSchemaVersion.Ifc4)
            {
                var memoryModel = _model as MemoryModel<Ifc4.EntityFactory>;
                if (memoryModel != null)
                    memoryModel.Save(fileName);
            }
            else if (_schema == IfcSchemaVersion.Ifc2X3)
            {
                var memoryModel = _model as MemoryModel<Ifc2x3.EntityFactory>;
                if (memoryModel != null)
                    memoryModel.Save(fileName);
            }
        }
    }

}
