﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.Step21;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;
using PropertyTranformDelegate = Xbim.Common.PropertyTranformDelegate;

namespace Xbim.IO.Memory
{
    public class MemoryModel : IModel, IDisposable
    {
        private static ZipEntry GetZipEntry(Stream fileStream)
        {
            // used because - The ZipInputStream has one major advantage over using ZipFile to read a zip: 
            // it can read from an unseekable input stream - such as a WebClient download
            using (var zipStream = new ZipInputStream(fileStream))
            {
                var entry = zipStream.GetNextEntry();
                while (entry != null)
                {
                    if (entry.IsFile && entry.Name.StorageType() != IfcStorageType.Invalid)
                    {
                        return entry;
                    }
                    entry = zipStream.GetNextEntry(); //get next entry
                }
            }
            return null;
        }

        public static IStepFileHeader GetFileHeader(string fileName)
        {
            // need to get the header for each step file storage type
            //if it is a zip file
            var storageType = fileName.StorageType();
            using (var stream = File.OpenRead(fileName))
            {

                if (storageType.HasFlag(IfcStorageType.IfcZip)|| storageType.HasFlag(IfcStorageType.Zip))
                {

                    var zipEntry = GetZipEntry(stream);
                    if (zipEntry == null) return null;

                    var zipStorageType = zipEntry.Name.StorageType();
                    using (var zipFile = new ZipFile(fileName))
                    {
                        using (var reader = zipFile.GetInputStream(zipEntry))
                        {
                            switch (zipStorageType)
                            {
                                case IfcStorageType.Ifc:
                                    return GetStepFileHeader(reader);
                                case IfcStorageType.IfcXml:
                                    return XbimXmlReader4.ReadHeader(reader);
                            }
                        }
                    }
                    return null;
                }
                if (storageType.HasFlag(IfcStorageType.IfcXml) )
                    return XbimXmlReader4.ReadHeader(stream);
                //go for default of Ifc
                return GetStepFileHeader(stream);
            }
        }

        public static IStepFileHeader GetStepFileHeader(Stream stream)
        {
                var parser = new XbimP21Parser(stream, null,-1);
                var stepHeader = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);
                parser.EntityCreate += (string name, long? label, bool header, out int[] ints) =>
                {
                    //allow all attributes to be parsed
                    ints = null;
                    if (header)
                    {
                        switch (name)
                        {
                            case "FILE_DESCRIPTION":
                                return stepHeader.FileDescription;
                            case "FILE_NAME":
                                return stepHeader.FileName;
                            case "FILE_SCHEMA":
                                return stepHeader.FileSchema;
                            default:
                                return null;
                        }
                    }
                    parser.Cancel = true; //done enough
                    return null;
                };
                parser.Parse();
                stream.Close();
                return stepHeader;
            }
        

   
        private readonly EntityCollection _instances;
        private readonly IEntityFactory _entityFactory;

        internal IEntityFactory EntityFactory { get { return _entityFactory; } }

        public int UserDefinedId { get; set; }
        public MemoryModel(IEntityFactory entityFactory)
        {
            if (entityFactory == null) throw new ArgumentNullException("entityFactory");

            _entityFactory = entityFactory;
            _instances = new EntityCollection(this);
            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults);
            foreach (var schemasId in _instances.Factory.SchemasIds)
                Header.FileSchema.Schemas.Add(schemasId);
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);
            Metadata = ExpressMetaData.GetMetadata(entityFactory.GetType().Module);
            IsTransactional = true;
        }

        /// <summary>
        /// Instance collection of all entities in the model. You can use this collection to create new
        /// entities or to query the model. This is the only way how to create new entities.
        /// </summary>
        public virtual IEntityCollection Instances
        {
            get { return _instances; }
        }

        public virtual bool Activate(IPersistEntity owningEntity, bool write)
        {
            return true;
        }

        /// <summary>
        /// This implementation of IModel keeps all data in memory
        /// so this doesn't have any effect.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="depth"></param>
        public void Activate(IPersistEntity entity, int depth)
        {
            
        }

        /// <summary>
        /// This function will try and release a persistant entity from the model, if the entity is referenced by another entity 
        /// it will stay in the model but can only be accessed via other entities,however if the model is saved and then reloaded 
        /// the entity will be restored to persisted status
        /// if the the entity is not referenced it will be garbage collected and removed and lost
        /// All entities that are directly referenced by this entity will also be made candidates to be dropped and dropped
        /// inverse references are not pursued
        /// Once dropped an entity cannot be accessed via the instances collection.
        /// Returns a collection of entities that have been dropped
        /// </summary>
        /// <param name="entity">the root entity to drop</param>
        public IEnumerable<IPersistEntity> TryDrop(IPersistEntity entity)
        {
            var dropped = new HashSet<IPersistEntity>();
            TryDrop(entity, dropped);
            return dropped;
        }

        
        private void TryDrop(IPersistEntity entity, HashSet<IPersistEntity> dropped)
        {
            
            if (!dropped.Add(entity)) return; //if the entity is in the map do not delete it again
            if (!_instances.Contains(entity)) return; //already gone
            _instances.RemoveReversible(entity);
            var expressType = Metadata.ExpressType(entity);
            foreach (var ifcProperty in expressType.Properties.Values)
            //only delete persistent attributes, ignore inverses
            {
                if (ifcProperty.EntityAttribute.State != EntityAttributeState.DerivedOverride)
                {
                    var propVal = ifcProperty.PropertyInfo.GetValue(entity, null);
                    var iPersist = propVal as IPersistEntity;
                    if(iPersist != null)
                        TryDrop(iPersist, dropped);
                    else if (propVal is IExpressEnumerable)
                    {
                        var propType = ifcProperty.PropertyInfo.PropertyType;
                        //only process lists that are real lists, see cartesianpoint
                        var genType = propType.GetItemTypeFromGenericType();
                        if (genType != null && typeof(IPersistEntity).IsAssignableFrom(genType))
                        {
                            foreach (var item in ((IExpressEnumerable)propVal).OfType<IPersistEntity>())
                                TryDrop(item,  dropped);
                        }
                    }  
                }
            }
        }
        /// <summary>
        /// This will delete the entity from model dictionary and also from any references in the model.
        /// Be carefull as this might take a while to check for all occurances of the object. Also make sure 
        /// you don't use this object anymore yourself because it won't get disposed until than. This operation
        /// doesn't guarantee that model is compliant with any kind of schema but it leaves it consistent. So if you
        /// serialize the model there won't be any references to the object which wouldn't be there.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual void Delete(IPersistEntity entity)
        {
            //remove from entity collection
            var removed = _instances.RemoveReversible(entity);
            if (!removed) return;

            var entityType = entity.GetType();
            List<DeleteCandidateType> candidateTypes;
            if (!_deteteCandidatesCache.TryGetValue(entityType, out candidateTypes))
            {
                candidateTypes = new List<DeleteCandidateType>();
                _deteteCandidatesCache.Add(entityType, candidateTypes);

                //find all potential references and delete from there
                var types = Metadata.Types().Where(t => typeof(IInstantiableEntity).IsAssignableFrom(t.Type));
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var type in types)
                {
                    var toNullify = type.Properties.Values.Where(p =>
                        p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                        p.PropertyInfo.PropertyType.IsAssignableFrom(entityType)).ToList();
                    var toRemove =
                        type.Properties.Values.Where(p =>
                            p.EntityAttribute != null && p.EntityAttribute.Order > 0 &&
                            p.PropertyInfo.PropertyType.IsGenericType &&
                            p.PropertyInfo.PropertyType.GenericTypeArgumentIsAssignableFrom(entityType)).ToList();
                    if (!toNullify.Any() && !toRemove.Any()) continue;

                    candidateTypes.Add(new DeleteCandidateType { Type = type, ToNullify = toNullify, ToRemove = toRemove });
                }
            }

            foreach (var candidateType in candidateTypes)
                DeleteReferences(entity, candidateType);
        }

        /// <summary>
        /// Deletes references to specified entity from all entities in the model where entity is
        /// a references as an object or as a member of a collection.
        /// </summary>
        /// <param name="entity">Entity to be removed from references</param>
        /// <param name="candidateType">Candidate type containing reference to the type of entity</param>
        protected virtual void DeleteReferences(IPersistEntity entity, DeleteCandidateType candidateType)
        {
            //get all instances of this type and nullify and remove the entity
            var entitiesToCheck = _instances.OfType(candidateType.Type.Type);
            foreach (var toCheck in entitiesToCheck)
            {
                //check properties
                foreach (var pInfo in candidateType.ToNullify.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;
                    //it is enough to compare references
                    if (!ReferenceEquals(pVal, entity)) continue;
                    pInfo.SetValue(toCheck, null);
                }

                foreach (var pInfo in candidateType.ToRemove.Select(p => p.PropertyInfo))
                {
                    var pVal = pInfo.GetValue(toCheck);
                    if (pVal == null) continue;

                    //it might be uninitialized optional item set
                    var optSet = pVal as IOptionalItemSet;
                    if (optSet != null && !optSet.Initialized) continue;

                    //or it is non-optional item set implementind IList
                    var itemSet = pVal as IList;
                    if (itemSet != null)
                    {
                        if (itemSet.Contains(entity))
                            itemSet.Remove(entity);
                        continue;
                    }

                    //fall back operating on common list functions using reflection (this is slow)
                    var contMethod = pInfo.PropertyType.GetMethod("Contains");
                    if (contMethod == null) continue;
                    var contains = (bool)contMethod.Invoke(pVal, new object[] { entity });
                    if (!contains) continue;
                    var removeMethod = pInfo.PropertyType.GetMethod("Remove");
                    if (removeMethod == null) continue;
                    removeMethod.Invoke(pVal, new object[] { entity });
                }
            }
        }

        /// <summary>
        /// Helper structure to hold information for reference removal. If multiple objects of the same type are to
        /// be removed this will cache the information about where to have a look for the references.
        /// </summary>
        protected struct DeleteCandidateType
        {
            public ExpressType Type;
            public List<ExpressMetaProperty> ToNullify;
            public List<ExpressMetaProperty> ToRemove;
        }

        private readonly Dictionary<Type, List<DeleteCandidateType>> _deteteCandidatesCache = new Dictionary<Type, List<DeleteCandidateType>>(); 

        public virtual ITransaction BeginTransaction(string name)
        {
            if (CurrentTransaction != null)
                throw new XbimException("Transaction is opened already.");
            if(InverseCache != null)
                throw new XbimException("Transaction can't be open when cache is in operation.");

            var txn = new Transaction(this);
            CurrentTransaction = txn;
            return txn;
        }

        public IStepFileHeader Header { get; private set; }

        public virtual bool IsTransactional { get; private set; }

        /// <summary>
        /// Weak reference allows garbage collector to collect transaction once it goes out of the scope
        /// even if it is still referenced from model. This is important for the cases where the transaction
        /// is both not commited and not rolled back either.
        /// </summary>
        private WeakReference _transactionReference;

        private InMemoryGeometryStore _geometryStore;

        public virtual ITransaction CurrentTransaction
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

        public virtual IModelFactors ModelFactors { get; private set; }
        public ExpressMetaData Metadata { get; private set; }

       
        public virtual void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            foreach (var entity in source)
            {
                body(entity);
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

        public IInverseCache BeginCaching()
        {
            if (CurrentTransaction != null)
                throw new XbimException("Caching is not allowed within active transaction.");

            var c = InverseCache;
            if (c != null)
                return c;
            return InverseCache = new InverseCache();
        }

        public void StopCaching()
        {
            var c = InverseCache;
            if (c == null)
                return;

            c.Dispose();
            InverseCache = null;
        }

        private WeakReference _cacheReference;
        public IInverseCache InverseCache
        {
            get
            {
                if (_cacheReference == null || !_cacheReference.IsAlive)
                    return null;
                return _cacheReference.Target as IInverseCache;
            }
            private set
            {
                if (value == null)
                {
                    _cacheReference = null;
                    return;
                }
                if (_cacheReference == null)
                    _cacheReference = new WeakReference(value);
                else
                    _cacheReference.Target = value;
            }
        }

        internal void HandleEntityChange(ChangeType changeType, IPersistEntity entity, byte propertyOrder)
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
                        EntityModified(entity, propertyOrder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("changeType", changeType, null);
            }
        }

        public virtual void LoadXml(string path, ReportProgressDelegate progDelegate = null)
        {
            using (var file = File.OpenRead(path))
            {
                LoadXml(file, file.Length, progDelegate);
            }
        }

        public virtual void LoadXml(Stream stream, long streamSize, ReportProgressDelegate progDelegate = null)
        {
            _read.Clear();
            var schema = _entityFactory.SchemasIds.First();
            if (string.Equals(schema, "IFC2X3", StringComparison.OrdinalIgnoreCase))
            {
                var reader3 = new IfcXmlReader(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) reader3.ProgressStatus += progDelegate;
                Header = reader3.Read(stream);
                if (progDelegate != null) reader3.ProgressStatus -= progDelegate;
            }
            else
            {
                var xmlReader = new XbimXmlReader4(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) xmlReader.ProgressStatus += progDelegate;
                Header = xmlReader.Read(stream);
                if (progDelegate != null) xmlReader.ProgressStatus -= progDelegate;
            }

            if(Header.FileSchema.Schemas == null)
                Header.FileSchema.Schemas = new List<string>();
            if (!Header.FileSchema.Schemas.Any())
                Header.FileSchema.Schemas.Add(schema);

            //purge
            _read.Clear();
        }

        private readonly Dictionary<int, IPersistEntity>  _read = new Dictionary<int, IPersistEntity>();
        private IPersistEntity GetOrCreateXMLEntity(int label, Type type)
                    {
                        IPersistEntity exist;
            if (_read.TryGetValue(label, out exist))
                            return exist;

                        var ent = _instances.Factory.New(this, type, label, true);
                        _instances.InternalAdd(ent);
            _read.Add(label, ent);
                        return ent;
        }

        public virtual void LoadZip(string file, ReportProgressDelegate progDelegate = null)
        {
            using (var stream = File.OpenRead(file))
            {
                LoadZip(stream, progDelegate);
                stream.Close();
            }
        }

        /// <summary>
        /// Loads the content of the model from ZIP archive. If the actual model file inside the archive is XML
        /// it is supposed to have an extension containing 'XML' like '.ifcxml', '.stpxml' or similar.
        /// </summary>
        /// <param name="stream">Input stream of the ZIP archive</param>
        /// <param name="progDelegate"></param>
        public virtual void LoadZip(Stream stream, ReportProgressDelegate progDelegate = null)
        {
            using (var zipStream = new ZipInputStream(stream))
            {
                var entry = zipStream.GetNextEntry();

                var extension = Path.GetExtension(entry.Name) ?? "";
                var xml = extension.ToLower().Contains("xml");
                using (var zipFile = new ZipFile(stream))
                {
                    while (entry != null)
                    {
                        if (!entry.IsFile) 
                        {
                            entry = zipStream.GetNextEntry();
                            continue; 
                        }
                        try
                        {
                            using (var reader = zipFile.GetInputStream(entry))
                            {
                                if (xml)
                                    LoadXml(reader, entry.Size, progDelegate);
                                else
                                    LoadStep21(reader, entry.Size, progDelegate);

                                reader.Close();
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            //if it crashed try next entry if available
                            entry = zipStream.GetNextEntry();
                        }
                    }
                    zipFile.Close();
                }

                zipStream.Close();
            }
        }

        /// <summary>
        /// Opens the model from STEP21 file. 
        /// </summary>
        /// <param name="stream">Path to the file</param>
        /// <param name="streamSize"></param>
        /// <param name="progDelegate"></param>
        /// <returns>Number of errors in parsing. Always check this to be null or the model might be incomplete.</returns>
        public virtual int LoadStep21(Stream stream, long streamSize, ReportProgressDelegate progDelegate=null)
        {
            var parser = new XbimP21Parser(stream, Metadata, streamSize);
            if (progDelegate != null) parser.ProgressStatus += progDelegate;
            var first = true;
            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty);
            parser.EntityCreate += (string name, long? label, bool header, out int[] ints) =>
            {
                //allow all attributes to be parsed
                ints = null;

                if (header)
                {
                    switch (name)
                    {
                        case "FILE_DESCRIPTION":
                            return Header.FileDescription;
                        case "FILE_NAME":
                            return Header.FileName;
                        case "FILE_SCHEMA":
                            return Header.FileSchema;
                        default:
                            return null;
                    }
                }
                
                if (label == null)
                    return _instances.Factory.New(name);
                //if this is a first non-header entity header is read completely by now. 
                //So we should check if the schema declared in the file is the one declared in EntityFactory
                if (first) 
                {
                    first = false;
                    if (!Header.FileSchema.Schemas.All(s => _instances.Factory.SchemasIds.Contains(s)))
                        throw new Exception("Mismatch between schema defined in the file and schemas available in the data model.");
                }

                var typeId = Metadata.ExpressTypeId(name);
                var ent = _instances.Factory.New(this, typeId, (int)label, true);
                _instances.InternalAdd(ent);

                //make sure that new added entities will have higher labels to avoid any clashes
                if (label >= _instances.NextLabel)
                    _instances.NextLabel = (int)label + 1;
                return ent;
            };
            parser.Parse();
            if (progDelegate != null) parser.ProgressStatus -= progDelegate;
            return parser.ErrorCount;
        }

        /// <summary>
        /// Opens the model from STEP21 file. 
        /// </summary>
        /// <param name="file">Path to the file</param>
        /// <param name="progDelegate"></param>
        /// <returns>Number of errors in parsing. Always check this to be null or the model might be incomplete.</returns>
        public virtual int LoadStep21(string file, ReportProgressDelegate progDelegate=null)
        {
            using (var stream = File.OpenRead(file))
            {
                var result = LoadStep21(stream, stream.Length, progDelegate);
                stream.Close();
                return result;
            }
        }



        public virtual void SaveAsXml(Stream stream, XmlWriterSettings xmlSettings, XbimXmlSettings xbimSettings = null, configuration configuration = null, ReportProgressDelegate progress = null)
        {
            using (var xmlWriter = XmlWriter.Create(stream, xmlSettings))
            {
                var schema = _entityFactory.SchemasIds.FirstOrDefault();
                switch (schema)
                {
                    case "IFC2X3":
                        var writer3 = new IfcXmlWriter3();
                        writer3.Write(this, xmlWriter, GetXmlOrderedEntities(schema));
                        break;
                    case "IFC4":
                        var writer4 = new XbimXmlWriter4(configuration.IFC4Add1, XbimXmlSettings.IFC4Add1);
                        writer4.Write(this, xmlWriter, GetXmlOrderedEntities(schema));
                        break;
                    case "COBIE_EXPRESS":
                        var writerCobie = new XbimXmlWriter4(configuration.COBieExpress, XbimXmlSettings.COBieExpress);
                        writerCobie.Write(this, xmlWriter, GetXmlOrderedEntities(schema));
                        break;
                    default:
                        var writer = new XbimXmlWriter4(configuration, xbimSettings);
                        writer.Write(this, xmlWriter);
                        break;
                }
                xmlWriter.Close();
            }
        }

        private IEnumerable<IPersistEntity> GetXmlOrderedEntities(string schema)
        {
            if (schema != null && schema.ToLower().Contains("cobie"))
            {
                return Instances.OfType("Facility", true)
                    .Concat(Instances);
            }

            if (schema == null || !schema.StartsWith("IFC"))
                return Instances;

            var project = Instances.OfType("IfcProject", true);
            var products = Instances.OfType("IfcObject", true);
            var relations = Instances.OfType("IfcRelationship", true);

            //create nice deep XML structure if possible
            var all =
                new IPersistEntity[] { }
                //start from root
                    .Concat(project)
                //add all products not referenced in the project tree
                    .Concat(products)
                //add all relations which are not inversed
                    .Concat(relations)
                //make sure all other objects will get written
                    .Concat(Instances);
            return all;
        }

        public virtual void SaveAsXMLZip(Stream stream, XmlWriterSettings xmlSettings, XbimXmlSettings xbimSettings = null, configuration configuration = null, ReportProgressDelegate progress = null)
        {
            using (var zipStream = new ZipOutputStream(stream))
            {
                var schema = _entityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifcxml" : ".xml";
                var newEntry = new ZipEntry("data" + ext) { DateTime = DateTime.Now };
                zipStream.PutNextEntry(newEntry);
                SaveAsXml(zipStream, xmlSettings, xbimSettings, configuration, progress);
            }
        }

        public virtual void SaveAsStep21Zip(Stream stream, ReportProgressDelegate progress = null)
        {
            using (var zipStream = new ZipOutputStream(stream))
            {
                var schema = _entityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifc" : ".stp";
                var newEntry = new ZipEntry("data" + ext) { DateTime = DateTime.Now };
                zipStream.PutNextEntry(newEntry);
                SaveAsStep21(zipStream, progress);
            }
        }

        /// <summary>
        /// Saves the model as PART21 file
        /// </summary>
        /// <param name="stream">Stream to be used to write the file</param>
        /// <param name="progress"></param>
        public virtual void SaveAsStep21(Stream stream, ReportProgressDelegate progress = null)
        {
            using (var writer = new StreamWriter(stream))
            {
                SaveAsStep21(writer);       
            }
        }

        /// <summary>
        /// Saves the model as PART21 file
        /// </summary>
        /// <param name="writer">Text writer to be used to write the file</param>
        /// <param name="progress"></param>
        public virtual void SaveAsStep21(TextWriter writer, ReportProgressDelegate progress = null)
        {
            var part21Writer = new Part21FileWriter();
            part21Writer.Write(this, writer, Metadata, new Dictionary<int, int>());
        }

        public void Dispose()
        {
            _instances.Dispose();
            _deteteCandidatesCache.Clear();
            _transactionReference = null;
        }

        /// <summary>
        /// Inserts deep copy of an object into this model. The entity must originate from the same schema (the same EntityFactory). 
        /// This operation happens within a transaction which you have to handle yourself unless you set the parameter "noTransaction" to true.
        /// Insert will happen outside of transactional behaviour in that case. Resulting model is not guaranteed to be valid according to any
        /// model view definition. However, it is granted to be consistent. You can optionaly bring in all inverse relationships. Be carefull as it
        /// might easily bring in almost full model.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the copied entity</typeparam>
        /// <param name="toCopy">Entity to be copied</param>
        /// <param name="mappings">Mappings of previous inserts</param>
        /// <param name="includeInverses">Option if to bring in all inverse entities (enumerations in original entity)</param>
        /// <param name="keepLabels">Option if to keep entity labels the same</param>
        /// <param name="propTransform">Optional delegate which you can use to filter the content which will get coppied over.</param>
        /// <returns>Copy from this model</returns>
        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
           bool keepLabels) where T : IPersistEntity
        {
            return InsertCopy(toCopy, mappings, propTransform, includeInverses, keepLabels, false);
        }

        /// <summary>
        /// Inserts deep copy of an object into this model. The entity must originate from the same schema (the same EntityFactory). 
        /// This operation happens within a transaction which you have to handle yourself unless you set the parameter "noTransaction" to true.
        /// Insert will happen outside of transactional behaviour in that case. Resulting model is not guaranteed to be valid according to any
        /// model view definition. However, it is granted to be consistent. You can optionaly bring in all inverse relationships. Be carefull as it
        /// might easily bring in almost full model.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the copied entity</typeparam>
        /// <param name="toCopy">Entity to be copied</param>
        /// <param name="mappings">Mappings of previous inserts</param>
        /// <param name="includeInverses">Option if to bring in all inverse entities (enumerations in original entity)</param>
        /// <param name="keepLabels">Option if to keep entity labels the same</param>
        /// <param name="propTransform">Optional delegate which you can use to filter the content which will get coppied over.</param>
        /// <param name="noTransaction">If TRUE all operations inside this function will happen out of transaction. 
        /// Also no notifications will be fired from objects.</param>
        /// <returns>Copy from this model</returns>
        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
           bool keepLabels, bool noTransaction) where T : IPersistEntity
        {
            var tName = toCopy.ExpressType.Name;
            if (noTransaction)
                IsTransactional = false;
            try
            {
                var toCopyLabel = toCopy.EntityLabel;
                XbimInstanceHandle copyHandle;
                var toCopyHandle = new XbimInstanceHandle(toCopy);
                //try to get the value if it was created before
                if (mappings.TryGetValue(toCopyHandle, out copyHandle))
                {
                    return (T) copyHandle.GetEntity();
                }

                var expressType = Metadata.ExpressType(toCopy);
                var copy = keepLabels ? _instances.New(toCopy.GetType(), toCopyLabel) : _instances.New(toCopy.GetType());
                copyHandle = new XbimInstanceHandle(copy);
                //key is the label in original model
                mappings.Add(toCopyHandle, copyHandle);

                var props = expressType.Properties.Values.Where(p => !p.EntityAttribute.IsDerived);
                if (includeInverses)
                    props = props.Union(expressType.Inverses);

                foreach (var prop in props)
                {
                    var value = propTransform != null
                        ? propTransform(prop, toCopy)
                        : prop.PropertyInfo.GetValue(toCopy, null);
                    if (value == null) continue;

                    var isInverse = (prop.EntityAttribute.Order == -1); //don't try and set the values for inverses
                    var theType = value.GetType();
                    //if it is an express type or a value type, set the value
                    if (theType.IsValueType || typeof (ExpressType).IsAssignableFrom(theType) || theType == typeof(string))
                    {
                        prop.PropertyInfo.SetValue(copy, value, null);
                    }
                    else if (!isInverse && typeof (IPersistEntity).IsAssignableFrom(theType))
                    {
                        prop.PropertyInfo.SetValue(copy,
                            InsertCopy((IPersistEntity) value, mappings, propTransform, includeInverses, keepLabels,
                                noTransaction), null);
                    }
                    else if (!isInverse && typeof (IList).IsAssignableFrom(theType))
                    {
                        var itemType = theType.GetItemTypeFromGenericType();

                        var copyColl = prop.PropertyInfo.GetValue(copy, null) as IList;
                        if (copyColl == null)
                            throw new Exception(string.Format("Unexpected collection type ({0}) found", itemType.Name));

                        foreach (var item in (IList) value)
                        {
                            var actualItemType = item.GetType();
                            if (actualItemType.IsValueType || typeof (ExpressType).IsAssignableFrom(actualItemType))
                                copyColl.Add(item);
                            else if (typeof (IPersistEntity).IsAssignableFrom(actualItemType))
                            {
                                var cpy = InsertCopy((IPersistEntity) item, mappings, propTransform, includeInverses,
                                    keepLabels, noTransaction);
                                copyColl.Add(cpy);
                            }
                            else
                                throw new Exception(string.Format("Unexpected collection item type ({0}) found",
                                    itemType.Name));
                        }
                    }
                    else if (isInverse && value is IEnumerable<IPersistEntity>) //just an enumeration of IPersistEntity
                    {
                        foreach (var ent in (IEnumerable<IPersistEntity>) value)
                        {
                            InsertCopy(ent, mappings, propTransform, includeInverses, keepLabels, noTransaction);
                        }
                    }
                    else if (isInverse && value is IPersistEntity) //it is an inverse and has a single value
                    {
                        InsertCopy((IPersistEntity) value, mappings, propTransform, includeInverses, keepLabels,
                            noTransaction);
                    }
                    else
                        throw new Exception(string.Format("Unexpected item type ({0})  found", theType.Name));
                }
                return (T) copy;
            }          
            finally
            {
                //make sure model is transactional at the end again
                IsTransactional = true;
            }
        }


        public IGeometryStore GeometryStore
        {
            get { return _geometryStore ?? (_geometryStore = new InMemoryGeometryStore()); }
        }


       


        /// <summary>
        /// Returns a list of the handles to only the entities in this model
        /// Note this do NOT include entities that are in any federated models
        /// </summary>

        public IList<XbimInstanceHandle> InstanceHandles
        {
            get { return _instances.Select(e => new XbimInstanceHandle(this, e.EntityLabel)).ToList(); }
        }

       
    }

    /// <summary>
    /// This delegate can be used to implement customized logic in type mapping.
    /// </summary>
    /// <param name="entity">Original entity</param>
    /// <returns>Express type which maps to the type of the original entity</returns>
    public delegate ExpressType TypeResolverDelegate(IPersistEntity entity);
}
