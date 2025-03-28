﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Model;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO;
using Xbim.IO.Memory;
using Xbim.IO.Step21;

[assembly: InternalsVisibleTo("Xbim.Essentials.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010029a3c6da60efcb3ebe48c3ce14a169b5fa08ffbf5f276392ffb2006a9a2d596f5929cf0e68568d14ac7cbe334440ca0b182be7fa6896d2a73036f24bca081b2427a8dec5689a97f3d62547acd5d471ee9f379540f338bbb0ae6a165b44b1ae34405624baa4388404bce6d3e30de128cec379147af363ce9c5845f4f92d405ed0")]
namespace Xbim.Ifc
{
    /// <summary>
    /// The <see cref="IfcStore"/> is the main entry point for working with Model files of any format.
    /// 
    /// IfcStore handles opening, parsing, export, and (optionally persistence) of Ifc files in any format,
    /// as well as accessing internal XBIM formats (such as *.xbim) files. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note: the Store capabilities may be affected by the ModelProvider implementation - some stores may not
    /// implement all capabilities. e.g. An In-Memory store will not permit reading or writing to XBIM format.
    /// </para>
    /// <para>
    /// IMPORTANT: The exact IModel implementation used depends on the <see cref="IModelProvider"/> resolved. EsentModel
    /// is only supported on Windows operating systems, so you may need to provide an alternative (SqlLite etc)
    /// through the configuration system.
    /// </para>
    /// <para>
    /// You can define the Model Provider used through the internal Services Collection at application startup:
    /// </para>
    /// <code>
    /// XbimServices.Current.ConfigureServices(s =&gt; s.AddXbimToolkit(opt =&gt; opt.UseModelProvider&lt;MyCustomProvider&gt;()));
    /// </code>
    /// 
    /// </remarks>
    public class IfcStore : IModel, IDisposable, IFederatedModel, IEquatable<IModel>
    {
        private const string RefDocument = "XbimReferencedModel";
        public event NewEntityHandler EntityNew;
        public event ModifiedEntityHandler EntityModified;
        public event DeletedEntityHandler EntityDeleted;


        private bool _disposed;

        private IIfcOwnerHistory _ownerHistoryAddObject;
        private IIfcOwnerHistory _ownerHistoryModifyObject;

        private IIfcPersonAndOrganization _defaultOwningUser;
        private IIfcApplication _defaultOwningApplication;
        
        private readonly ReferencedModelCollection _referencedModels = new ReferencedModelCollection();

        static IfcStore()
        {
            if(!XbimServices.Current.IsBuilt)
            {
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));
                }
                else
                {
                    XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddMemoryModel()));
                }
            }
            
        }

        // Internal Constructor for reading
        protected IfcStore()
        {
            ModelProvider = XbimServices.Current.ServiceProvider.GetRequiredService<IModelProvider>();
            Logger = XbimServices.Current.ServiceProvider.GetRequiredService<ILogger<IfcStore>>();
        }

        /// <summary>
        /// Constructor used to create a new persistent model with specified path
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="ifcVersion"></param>
        /// <param name="editorDetails"></param>
        protected IfcStore(string filepath, XbimSchemaVersion ifcVersion, XbimEditorCredentials editorDetails) : this()
        {
            var model = ModelProvider.Create(ifcVersion, filepath);
            AssignModel(model, editorDetails, ifcVersion, filepath);
        }

        /// <summary>
        /// Constructor used to create a new model for edit
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="ifcVersion"></param>
        /// <param name="editorDetails"></param>
        protected IfcStore(XbimStoreType storageType, XbimSchemaVersion ifcVersion, XbimEditorCredentials editorDetails) : this()
        {
            var model = ModelProvider.Create(ifcVersion, storageType);
            AssignModel(model, editorDetails, ifcVersion);
        }

        /// <summary>
        /// Gets the Model
        /// </summary>
        public IModel Model
        {
            get;
            protected set;
        }

        public XbimEditorCredentials EditorDetails { get; private set; }

        /// <summary>
        /// Provides access to model persistence capabilities
        /// </summary>
        public IModelProvider ModelProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Factory to create ModelProvider instances. 
        /// </summary>
        /// <remarks><see cref="ModelProviderFactory"/> is no longer in use. Update your initialisation code as follows.
        /// <example>
        /// For example here we change the default Mode to be the EsentModel:
        /// <code>
        /// XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.UseEsentModel()));
        /// </code>
        /// </example>
        /// </remarks>
        [Obsolete("Redundant. Model Providers are now provided by internal dependency injection with the ServicesProvider on XbimServices")]
        public static IModelProviderFactory ModelProviderFactory
        {
            get;
            set;
        }

        private void AssignModel(IModel model, XbimEditorCredentials editorDetails, XbimSchemaVersion schema, string modelPath = null)
        {
            Model = model;
            Model.EntityNew += Model_EntityNew;
            Model.EntityDeleted += Model_EntityDeleted;
            Model.EntityModified += Model_EntityModified;
            FileName = modelPath ?? Model.Header.FileName.Name;
            SetupEditing(editorDetails);

            LoadReferenceModels(modelPath);
            IO.Memory.MemoryModel.CalculateModelFactors(model);
        }

        /// <summary>
        /// Sets up the model to track changes and apply an editor/ownerhistory
        /// </summary>
        /// <param name="editorDetails"></param>
        private void SetupEditing(XbimEditorCredentials editorDetails)
        {
            if (editorDetails == null)
            {
                EditorDetails = new XbimEditorCredentials()
                {
                    ApplicationDevelopersName = "Unspecified",
                    ApplicationVersion = "Unspecified",
                    ApplicationFullName = "Unspecified",
                    EditorsFamilyName = Environment.UserName,
                    EditorsOrganisationName = "Unspecified",
                    EditorsGivenName = ""
                };
            }
            else
            {
                EditorDetails = editorDetails;
            }

            Model.EntityNew += IfcRootInit;
            Model.EntityModified += IfcRootModified;
        }


        /// <summary>
        /// Creates a Database store at the specified location
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="editorDetails"></param>
        /// <param name="ifcVersion"></param>
        /// <returns></returns>
        public static IfcStore Create(string filePath, XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion)
        {
            return new IfcStore(filePath, ifcVersion, editorDetails);
        }

        public static IfcStore Create(XbimEditorCredentials editorDetails, XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            return new IfcStore(storageType, ifcVersion, editorDetails);
        }

        public static IfcStore Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
        {
            return new IfcStore(storageType, ifcVersion, null);
        }
        /// <summary>
        /// You can use this function to open IFC model from a <see cref="Stream"/>. 
        /// You need to know file type (IFC, IFCZIP, IFCXML) and schema type (IFC2x3, IFC4 IFC4x3 etc) to be able to use this function.
        /// If you don't know, you should use the alternate <see cref="Open(string, XbimEditorCredentials, double?, ReportProgressDelegate, XbimDBAccess, int)"/>
        /// method which takes file paths as an argument, and can automatically detect schema and file type. 
        /// If are opening an *.xbim file you should also use the path-based overload because Esent database needs to operate 
        /// on the file and this function will have to create temporal file if it is not a file stream.
        /// If the input is a FileStream, be aware this method may call <see cref="Stream.Close"/> on it to keep exclusive access.
        /// </summary>
        /// <param name="stream">Stream of data</param>
        /// <param name="dataType">Type of data (*.ifc, *.ifcxml, *.ifczip)</param>
        /// <param name="schema">IFC schema. Non IFC schemas are not supported by this class.</param>
        /// <param name="modelType">Type of model to be used. You can choose between EsentModel and MemoryModel</param>
        /// <param name="editorDetails">Optional details. You should always pass these if you are going to change the data.</param>
        /// <param name="accessMode">Access mode to the stream. This is only important if you choose EsentModel. MemoryModel is completely in memory so this is not relevant</param>
        /// <param name="progDelegate">Progress reporting delegate</param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used according to the Ifc specification. </param>
        /// <returns></returns>
        public static IfcStore Open(Stream stream, StorageType dataType, XbimSchemaVersion schema, XbimModelType modelType, XbimEditorCredentials editorDetails = null, 
            XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
        {
            var newStore = new IfcStore();
            var model = newStore.ModelProvider.Open(stream, dataType, schema, modelType, accessMode, progDelegate, codePageOverride);

            newStore.AssignModel(model, editorDetails, schema);
            return newStore;

        }

        /// <summary>
        /// Open an IFC model from a <see cref="Stream"/> inferring the Schema from automatically. 
        /// You need to know IFC file format (IFC, IFCZIP, IFCXML) to use this function.
        /// If you don't know, you should use the alternate <see cref="Open(string, XbimEditorCredentials, double?, ReportProgressDelegate, XbimDBAccess, int)"/>
        /// method which takes file paths as an argument, and can automatically detect schema and file type. 
        /// If are opening an *.xbim file you should also use the path-based overload because Esent database needs to operate 
        /// on the file and this function will have to create temporal file if it is not a file stream.
        /// If the input is a FileStream, be aware this method may call <see cref="Stream.Close"/> on it to keep exclusive access.
        /// </summary>
        /// <param name="stream">Stream of data</param>
        /// <param name="dataType">Type of data (*.ifc, *.ifcxml, *.ifczip)</param>
        /// <param name="modelType">Type of model to be used. You can choose between EsentModel and MemoryModel</param>
        /// <param name="editorDetails">Optional details. You should always pass these if you are going to change the data.</param>
        /// <param name="accessMode">Access mode to the stream. This is only important if you choose EsentModel. MemoryModel is completely in memory so this is not relevant</param>
        /// <param name="progDelegate">Progress reporting delegate</param>
        /// <param name="codePageOverride">
        /// A CodePage that will be used to read implicitly encoded one-byte-char strings. If -1 is specified the default ISO8859-1
        /// encoding will be used according to the Ifc specification. </param>
        /// <param name="streamBufferSize">The size of buffer to use when reading the file header (default: 8KB)</param>
        /// <returns></returns>
        public static IfcStore Open(Stream stream, StorageType dataType, XbimModelType modelType, XbimEditorCredentials editorDetails = null,
            XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1, int streamBufferSize = 4096 * 2)
        {
            if (stream.CanSeek)
            {
                XbimSchemaVersion schema = InferPayloadFromSchema(stream);
                return OpenModelStream(stream, dataType, modelType, editorDetails, accessMode, progDelegate, codePageOverride, schema);
            }
            else
            {
                // On a non-seekable stream we need to buffer the stream so we can reset the position after reading the header
                using var seekableStream = new ReadSeekableStream(stream, streamBufferSize);
                XbimSchemaVersion schema;
                try
                {
                    schema = InferPayloadFromSchema(seekableStream);
                }
                catch(NotSupportedException ex)
                {
                    throw new XbimException($"Cannot infer Schema for this model since the header size ({seekableStream.Position} bytes) exceeds the size of the buffer size ({streamBufferSize} bytes)", ex);
                }
                return OpenModelStream(seekableStream, dataType, modelType, editorDetails, accessMode, progDelegate, codePageOverride, schema);
            }

        }

        private static XbimSchemaVersion InferPayloadFromSchema(Stream stream)
        {
            // TODO: Could also sniff Xml Format. (STP,Zip,Xml) from header
            XbimSchemaVersion schema = MemoryModel.GetStepFileXbimSchemaVersion(stream);
            stream.Seek(0, SeekOrigin.Begin);
            if(stream is ReadSeekableStream buffered)
            {
                // Optimisation: No further buffering required. Forward only from here
                buffered.DisableBuffering();
            }
            return schema;
        }

        private static IfcStore OpenModelStream(Stream stream, StorageType dataType, XbimModelType modelType, XbimEditorCredentials editorDetails, XbimDBAccess accessMode, ReportProgressDelegate progDelegate, int codePageOverride, XbimSchemaVersion schema)
        {
            var newStore = new IfcStore();
            var model = newStore.ModelProvider.Open(stream, dataType, schema, modelType, accessMode, progDelegate, codePageOverride);

            newStore.AssignModel(model, editorDetails, schema);
            return newStore;
        }

        /// <summary>
        /// Opens an IFC file, Ifcxml, IfcZip, xbim from a file path
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
        /// encoding will be used according to the Ifc specification. </param>
        public static IfcStore Open(string path, XbimEditorCredentials editorDetails = null, double? ifcDatabaseSizeThreshHold = null, 
            ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
        {

            path = Path.GetFullPath(path);

            if (!Directory.Exists(Path.GetDirectoryName(path) ?? ""))
                throw new DirectoryNotFoundException(Path.GetDirectoryName(path) + " directory was not found");
            if (!File.Exists(path))
                throw new FileNotFoundException(path + " file was not found");
            
            var newStore = new IfcStore(); // we need an instance
            var ifcVersion = newStore.ModelProvider.GetXbimSchemaVersion(path);
            if (ifcVersion == XbimSchemaVersion.Unsupported)
            {
                throw new FileLoadException(path + " is not a valid IFC file format, ifc, ifcxml, ifczip and xBIM are supported.");
            }

            var model = newStore.ModelProvider.Open(path, ifcVersion, ifcDatabaseSizeThreshHold, progDelegate, accessMode, codePageOverride);
            if (string.IsNullOrEmpty(model.Header.FileName.Name))
            {
                // if we are looking at a memory model loaded from a file it might be safe to fix the file name in the 
                // header with the actual file loaded
                // historically setting this Filename to local IFC Path isd what enabled 
                // Federations to find their relative child models. 
                FileInfo f = new FileInfo(path);
                model.Header.FileName.Name = f.FullName;
            }
            newStore.AssignModel(model, editorDetails, ifcVersion, path);
            return newStore;

        }

        #region IModel

        public object Tag { get => Model.Tag; set => Model.Tag = value; }
        protected ILogger Logger { get; private set; }
        public IInverseCache InverseCache
        {
            get { return Model.InverseCache; }
        }

        public int UserDefinedId
        {
            get { return Model.UserDefinedId; }
            set { Model.UserDefinedId = value; }
        }

        public IGeometryStore GeometryStore
        {
            get { return Model.GeometryStore; }
        }

        public IStepFileHeader Header
        {
            get { return Model.Header; }
        }

        public bool IsTransactional
        {
            get { return Model.IsTransactional; }
        }

        public string Location
        {
            get => ModelProvider.GetLocation(Model);
        }

        public IEntityCollection Instances
        {
            get { return Model.Instances; }
        }

        /// <summary>
        /// Returns a list of the handles to only the entities in this model
        /// Note this do NOT include entities that are in any federated models
        /// </summary>
        public IList<XbimInstanceHandle> InstanceHandles
        {
            get { return Model.InstanceHandles.ToList(); }
        }

        bool IModel.Activate(IPersistEntity owningEntity)
        {
            return Model.Activate(owningEntity);
        }

        public void Delete(IPersistEntity entity)
        {
            Model.Delete(entity);
        }

        public ITransaction BeginTransaction(string name = null)
        {

            if(Model.IsTransactional)
            {
                return Model.BeginTransaction(name);
            }
            else
            {
                throw new XbimException("Native store does not support transactions");
            }
        }

        public ITransaction CurrentTransaction
        {
            get { return Model.CurrentTransaction; }
        }

        public ExpressMetaData Metadata
        {
            get { return Model.Metadata; }
        }

        public IModelFactors ModelFactors
        {
            get { return Model.ModelFactors; }
        }

        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
            bool keepLabels) where T : IPersistEntity
        {
            try
            {
                // don't handle new and modified instances for inserted objects as these should have their own
                // data or should be handles explicitely
                ManageOwnerHistory = false;
                return Model.InsertCopy(toCopy, mappings, propTransform, includeInverses, keepLabels);
            }
            finally
            {
                // switch back
                ManageOwnerHistory = true;
            }
        }

        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            Model.ForEach(source, body);
        }

        
        public IEntityCache EntityCache => Model.EntityCache;

        public IInverseCache BeginInverseCaching()
        {
            return Model.BeginInverseCaching();
        }

        public IEntityCache BeginEntityCaching()
        {
            return Model.BeginEntityCaching();
        }
        public XbimSchemaVersion SchemaVersion => Model.SchemaVersion;

        #endregion // IModel
        /// <summary>
        /// Closes the store and disposes of all resources. The store is invalid after this call
        /// </summary>
        public void Close()
        {
            foreach (var referencedModel in _referencedModels)
            {
                referencedModel.Close();
            }
            ModelProvider.Close(Model);

        }

        #region OwnerHistory Management


        private void Model_EntityDeleted(IPersistEntity entity)
        {
            if (EntityDeleted != null) EntityDeleted.Invoke(entity);
        }

        private void Model_EntityNew(IPersistEntity entity)
        {
            if (EntityNew != null) EntityNew.Invoke(entity);
        }

        private void Model_EntityModified(IPersistEntity entity, int property)
        {
            if (EntityModified != null) EntityModified.Invoke(entity, property);
        }

        internal bool ManageOwnerHistory = true;

        private void IfcRootModified(IPersistEntity entity, int property)
        {
            if (!ManageOwnerHistory)
                return;

            if (!(entity is IIfcRoot root) || root.OwnerHistory == _ownerHistoryAddObject)
                return;

            if (root.OwnerHistory != _ownerHistoryModifyObject)
            {
                var originalHistory = root.OwnerHistory;
                MergeOwnerHistory(originalHistory);

                root.OwnerHistory = OwnerHistoryModifyObject;
                OwnerHistoryModifyObject.LastModifiedDate = DateTime.UtcNow;
            }
        }

        private void MergeOwnerHistory(IIfcOwnerHistory originalHistory)
        {
            // This is naive. We have a store-wide OwnerHistory for modifications when really we 
            // should clone one from the original history, and apply to those items we edit.
            // OwnerHistory is not really fit for purpose
            OwnerHistoryModifyObject.CreationDate = originalHistory.CreationDate;
            OwnerHistoryModifyObject.OwningApplication = originalHistory.OwningApplication;
            OwnerHistoryModifyObject.OwningUser = originalHistory.OwningUser;
        }

        private void IfcRootInit(IPersistEntity entity)
        {
            if (!ManageOwnerHistory)
                return;

            if (entity is IIfcRoot root)
            {
                root.OwnerHistory = OwnerHistoryAddObject;
                root.GlobalId = Guid.NewGuid().ToPart21();
                OwnerHistoryAddObject.LastModifiedDate = DateTime.UtcNow;
                if(OwnerHistoryAddObject.CreationDate.Value == null)
                    OwnerHistoryAddObject.CreationDate = DateTime.UtcNow;
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
                if (EditorDetails == null)
                    return null;

                var factory = new EntityCreator(this);

                var person = Instances.OfType<IIfcPerson>()
                    .FirstOrDefault(p => // Look up by identifier and then Given/Family name
                        EditorDetails.EditorsIdentifier != null && p.Identification == EditorDetails.EditorsIdentifier || 
                        (p.GivenName == EditorDetails.EditorsGivenName && p.FamilyName == EditorDetails.EditorsFamilyName))
                    ?? factory.Person(p =>
                    {
                        p.Identification = EditorDetails.EditorsIdentifier;
                        p.GivenName = EditorDetails.EditorsGivenName;
                        p.FamilyName = EditorDetails.EditorsFamilyName;
                    });

                var organization = Instances.OfType<IIfcOrganization>()
                    .FirstOrDefault(o => // Look up by identifier and then Organisation name
                        EditorDetails.EditorsOrganisationIdentifier != null && o.Identification == EditorDetails.EditorsOrganisationIdentifier || 
                        (o.Name == EditorDetails.EditorsOrganisationName))
                    ?? factory.Organization(o =>
                    {
                        o.Name = EditorDetails.EditorsOrganisationName;
                        o.Identification = EditorDetails.EditorsOrganisationIdentifier;
                    });

                _defaultOwningUser = 
                    Instances.OfType<IIfcPersonAndOrganization>()
                    .FirstOrDefault(po => po.ThePerson == person && po.TheOrganization == organization)
                    ?? factory.PersonAndOrganization(po =>
                    {
                        po.TheOrganization = organization;
                        po.ThePerson = person;
                    });

                
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
                if (EditorDetails == null)
                    return null;

                var existingApplication = Instances.OfType<IIfcApplication>()
                    .Where(a => a.ApplicationFullName == EditorDetails.ApplicationFullName)
                    .Where(a => a.ApplicationIdentifier == EditorDetails.ApplicationIdentifier)
                    .Where(a => a.Version == EditorDetails.ApplicationVersion)
                    .FirstOrDefault();
                if(existingApplication != null)
                {
                    // use existing to avoid duplicate applications
                    _defaultOwningApplication = existingApplication;
                }
                else
                {
                    // Create new application
                    var factory = new EntityCreator(this);
                    _defaultOwningApplication = factory.Application(a =>
                    {
                        a.ApplicationDeveloper = Instances.OfType<IIfcOrganization>().FirstOrDefault(o => o.Name == EditorDetails.ApplicationDevelopersName)
                            ?? factory.Organization(o =>
                            {
                                o.Name = EditorDetails.ApplicationDevelopersName;
                                o.Roles.Add(factory.ActorRole(r =>
                                {
                                    r.Role = IfcRoleEnum.USERDEFINED;
                                    r.UserDefinedRole = "Software Provider";
                                }));
                            });
                        a.ApplicationFullName = EditorDetails.ApplicationFullName;
                        a.ApplicationIdentifier = EditorDetails.ApplicationIdentifier;
                        a.Version = EditorDetails.ApplicationVersion;
                    });
                }
                return _defaultOwningApplication;
            }
        }

        public IIfcOwnerHistory OwnerHistoryAddObject
        {
            get
            {
                if (_ownerHistoryAddObject != null)
                    return _ownerHistoryAddObject;
                var factory = new EntityCreator(this);
                _ownerHistoryAddObject = factory.OwnerHistory(owner =>
                {
                    owner.OwningUser = DefaultOwningUser;
                    owner.OwningApplication = DefaultOwningApplication;
                    owner.ChangeAction = IfcChangeActionEnum.ADDED;
                    owner.LastModifyingUser = DefaultOwningUser;
                    owner.LastModifyingApplication = DefaultOwningApplication;
                    
                });
                return _ownerHistoryAddObject;
            }
        }

        internal IIfcOwnerHistory OwnerHistoryModifyObject
        {
            get
            {
                if (_ownerHistoryModifyObject != null)
                    return _ownerHistoryModifyObject;

                var factory = new EntityCreator(this);
                _ownerHistoryModifyObject = factory.OwnerHistory(owner =>
                {
                    owner.OwningUser = DefaultOwningUser;
                    owner.OwningApplication = DefaultOwningApplication;

                    owner.ChangeAction = IfcChangeActionEnum.MODIFIED;
                    owner.LastModifyingUser = DefaultOwningUser;
                    owner.LastModifyingApplication = DefaultOwningApplication;
                });
                
                return _ownerHistoryModifyObject;
            }
        }

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
                    actualFormat = StorageType.Ifc; //the default
                else
                {
                    // we don't want to lose the original extension required by the user, but we need to add .ifc 
                    // and set StorageType.Ifc as default
                    extension = extension + ".ifc";
                    actualFormat = StorageType.Ifc; //the default
                }
            }
            var actualFileName = Path.ChangeExtension(fileName, extension);
            
            SaveAs(actualFileName, actualFormat, progDelegate);
        }

        /// <summary>
        /// Saves / Exports the model to a given file with the provided model format
        /// </summary>
        /// <param name="actualFileName"></param>
        /// <param name="actualFormat">this will be correctly set</param>
        /// <param name="progDelegate"></param>
        private void SaveAs(string actualFileName, StorageType actualFormat, ReportProgressDelegate progDelegate)
          {
            FileName = actualFileName;
            if (actualFormat.HasFlag(StorageType.Xbim)) //special case for xbim
            {
                ModelProvider.Persist(Model, actualFileName, progDelegate);
            }
            else
            {
                using (var fileStream = new FileStream(actualFileName, FileMode.Create, FileAccess.Write))
                {
                    if (actualFormat.HasFlag(StorageType.IfcZip))
                        //do zip first so that xml and ifc are not confused by the combination of flags
                        IfcStoreExportExtensions.SaveAsIfcZip(this, fileStream, Path.GetFileName(actualFileName), actualFormat, progDelegate);
                    else if (actualFormat.HasFlag(StorageType.Ifc))
                        IfcStoreExportExtensions.SaveAsIfc(this, fileStream, progDelegate);
                    else if (actualFormat.HasFlag(StorageType.IfcXml))
                        IfcStoreExportExtensions.SaveAsIfcXml(this, fileStream, progDelegate);

                }
            }
        }


        #region Referenced Models functions / Federation

     
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
                var factory = new EntityCreator(this);
                // TODO: should look up an existing matching role/organisatuon
                var role = factory.ActorRole(r =>
                {
                    r.Role = IfcRoleEnum.USERDEFINED;
                    r.UserDefinedRole = organisationRole;
                });
                // the string is converted appropriately by the IfcActorRoleClass
                
                var org = factory.Organization(org =>
                {
                    org.Name = organisationName;
                    org.Roles.Add(role);
                });
                retVal = AddModelReference(refModelPath, org);

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
        private XbimReferencedModel AddModelReference(string refModelPath, IIfcOrganization owner)
        {
            XbimReferencedModel retVal;
            var factory = new EntityCreator(this);
            if (CurrentTransaction == null)
            {
                using (var txn = BeginTransaction())
                {
                    var docInfo = factory.DocumentInformation(d =>
                    {
                        d.Identification = new IfcIdentifier(_referencedModels.NextIdentifer());
                        d.Name = refModelPath;
                        d.DocumentOwner = owner;
                        d.IntendedUse = RefDocument;
                    });
                    retVal = new XbimReferencedModel(docInfo);
                    AddModelReference(retVal);
                    txn.Commit();
                }
            }
            else
            {
                var docInfo = factory.DocumentInformation(d =>
                {
                    d.Identification = new IfcIdentifier(_referencedModels.NextIdentifer());
                    d.Name = refModelPath;
                    d.DocumentOwner = owner;
                    d.IntendedUse = RefDocument;
                });
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
        /// <param name="rootModelPath">Path to the root model</param>
        /// <param name="throwErrorOnReferenceModelExceptions">Set to true to enable your own error handling</param>
        private void LoadReferenceModels(string rootModelPath, bool throwErrorOnReferenceModelExceptions = false)
        {
            var docInfos = Instances.OfType<IIfcDocumentInformation>().Where(d => d.IntendedUse == RefDocument);
            foreach (var docInfo in docInfos)
            {
                try
                {
                    AddModelReference(new XbimReferencedModel(docInfo, rootModelPath));
                }
                catch (Exception ex)
                {
                    // do not throw exception on referenceModel Creation
                    if (throwErrorOnReferenceModelExceptions)
                        throw;
                    Logger?.LogError(
                        string.Format("Ignored exception on modelreference load for #{0}.", docInfo.EntityLabel),
                        ex);
                }
                
            }
        }

        public string FileName { get; set; }
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


        public IModel ReferencingModel
        {
            get { return Model; }
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
        #endregion

        #region Equality

        /// <summary>
        /// Returns true if it is another reference to this or if it is an embedded model
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IModel other)
        {
            return ReferenceEquals(this, other) || ReferenceEquals(other, Model);
        }

        /// <summary>
        /// Returns true if it is another reference to this or if it is an embedded model
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Model.Equals(obj) || ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public static bool operator == (IfcStore store, IModel model)
        {
            if (ReferenceEquals(store, model))
                return true;
            if (ReferenceEquals(store, null))
                return false;
            if (ReferenceEquals(model, null))
                return false;

            return store.Model.Equals(model);
        }

        public static bool operator !=(IfcStore store, IModel model)
        {
            return !(store == model);
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        Close();
                        //release event handlers
                        if (Model != null)
                        {
                            Model.EntityDeleted -= Model_EntityDeleted;
                            Model.EntityNew -= Model_EntityNew;
                            Model.EntityModified -= Model_EntityModified;
                            if (EditorDetails != null)
                            {
                                Model.EntityNew -= IfcRootInit;
                                Model.EntityModified -= IfcRootModified;
                            }
                        }

                        //managed resources
                        if (Model is IDisposable disposeInterface)
                            disposeInterface.Dispose();

                        Model = null;
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
        #endregion
    }
}
