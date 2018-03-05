using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO;
using Xbim.IO.Parser;
using Xbim.IO.Step21;


namespace Xbim.Common.Model
{
    public class StepModel : IModel, IDisposable
    {
        public ILogger Logger { get; set; }

        public static List<string> GetStepFileSchemaVersion(Stream stream)
        {
            var scanner = new Scanner(stream);
            int tok = scanner.yylex();
            int dataToken = (int)Tokens.DATA;
            int eof = (int)Tokens.EOF;
            int typeToken = (int)Tokens.TYPE;
            int stringToken = (int)Tokens.STRING;

            //looking for: FILE_SCHEMA(('IFC2X3'));
            var schemas = new List<string>();

            while (tok != dataToken && tok != eof)
            {
                if (tok != typeToken)
                {
                    tok = scanner.yylex();
                    continue;
                }

                if (!string.Equals(scanner.yylval.strVal, "FILE_SCHEMA", StringComparison.OrdinalIgnoreCase))
                {
                    tok = scanner.yylex();
                    continue;
                }

                tok = scanner.yylex();
                //go until closing bracket
                while (tok != ')')
                {
                    if (tok != stringToken)
                    {
                        tok = scanner.yylex();
                        continue;
                    }

                    schemas.Add(scanner.yylval.strVal.Trim('\''));
                    tok = scanner.yylex();
                }
                break;
            }
            return schemas;
        }

        private readonly EntityCollection _instances;

        private IEntityFactory _entityFactory;
        public IEntityFactory EntityFactory { get { return _entityFactory; } }
        private readonly EntityFactoryResolverDelegate _factoryResolver;


        public object Tag { get; set; }
        public int UserDefinedId { get; set; }

        public StepModel(IEntityFactory entityFactory, int labelFrom)
        {
            InitFromEntityFactory(entityFactory);

            _instances = new EntityCollection(this, labelFrom);

            IsTransactional = true;
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);

            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults, this);
            foreach (var schemasId in _entityFactory.SchemasIds)
                Header.FileSchema.Schemas.Add(schemasId);
        }

        public StepModel(IEntityFactory entityFactory) : this(entityFactory, 0)
        {
        }

        public StepModel(IEntityFactory entityFactory, ILogger logger = null, int labelFrom = 0) : this(entityFactory, labelFrom)
        {
            Logger = logger;
        }

        public StepModel(EntityFactoryResolverDelegate factoryResolver, ILogger logger = null, int labelFrom = 0)
        {
            _factoryResolver = factoryResolver;
            _instances = new EntityCollection(this, labelFrom);
            IsTransactional = true;
            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);
        }

        private void InitFromEntityFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException("entityFactory");
        }

        /// <summary>
        /// Instance collection of all entities in the model. You can use this collection to create new
        /// entities or to query the model. This is the only way how to create new entities.
        /// </summary>
        public virtual IEntityCollection Instances
        {
            get { return _instances; }
        }

        bool IModel.Activate(IPersistEntity owningEntity)
        {
            //always return true because all entities are activated all the time in memory
            return true;
        }

        /// <summary>
        /// This function will try and release a persistent entity from the model, if the entity is referenced by another entity 
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
                    if (iPersist != null)
                        TryDrop(iPersist, dropped);
                    else if (propVal is IExpressEnumerable)
                    {
                        var propType = ifcProperty.PropertyInfo.PropertyType;
                        //only process lists that are real lists, see Cartesian point
                        var genType = propType.GetItemTypeFromGenericType();
                        if (genType != null && typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(genType))
                        {
                            foreach (var item in ((IExpressEnumerable)propVal).OfType<IPersistEntity>())
                                TryDrop(item, dropped);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This will delete the entity from model dictionary and also from any references in the model.
        /// Be careful as this might take a while to check for all occurrences of the object. Also make sure 
        /// you don't use this object anymore yourself because it won't get disposed until than. This operation
        /// doesn't guarantee that model is compliant with any kind of schema but it leaves it consistent. So if you
        /// serialize the model there won't be any references to the object which wouldn't be there.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual void Delete(IPersistEntity entity)
        {
            ModelHelper.Delete(this, entity, e => _instances.RemoveReversible(e));
        }


        public virtual ITransaction BeginTransaction(string name)
        {
            if (CurrentTransaction != null)
                throw new XbimException("Transaction is opened already.");
            if (InverseCache != null)
                throw new XbimException("Transaction can't be open when cache is in operation.");

            var txn = new Transaction(this);
            CurrentTransaction = txn;
            return txn;
        }

        public IStepFileHeader Header { get; protected set; }

        public virtual bool IsTransactional { get; private set; }

        /// <summary>
        /// Weak reference allows garbage collector to collect transaction once it goes out of the scope
        /// even if it is still referenced from model. This is important for the cases where the transaction
        /// is both not committed and not rolled back either.
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

        public virtual IModelFactors ModelFactors { get;  set; }

        private ExpressMetaData _metadata;

        public ExpressMetaData Metadata {
            get
            {
                return _metadata ?? 
                    (_metadata = ExpressMetaData.GetMetadata(_entityFactory.GetType().GetTypeInfo().Module));
            }
        }

        

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

        public IInverseCache BeginInverseCaching()
        {
            if (CurrentTransaction != null)
                throw new XbimException("Caching is not allowed within active transaction.");

            var c = InverseCache;
            if (c != null)
                return c;
            return InverseCache = new MemoryInverseCache(_instances);
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

        internal void HandleEntityChange(ChangeType changeType, IPersistEntity entity, int propertyOrder)
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

        public int LoadStep21Part(Stream data)
        {
            var parser = new XbimP21Scanner(data, -1)
            {
                Logger = Logger
            };
            return LoadStep21(parser);
        }

        public int LoadStep21Part(string data)
        {
            var parser = new XbimP21Scanner(data)
            {
                Logger = Logger
            };
            return LoadStep21(parser);
        }


        protected virtual int LoadStep21(XbimP21Scanner parser)
        {
            if (Header == null)
                Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);

            if (_entityFactory == null && _factoryResolver == null)
            {
                    throw new XbimParserException("EntityFactory is not defined and no resolver is specified to create one. Data can't be created.");
            }

            parser.EntityCreate += (string name, long? label, bool header) =>
            {
                if (header)
                {
                    switch (name)
                    {
                        case "FILE_DESCRIPTION":
                            return Header.FileDescription;
                        case "FILE_NAME":
                            return Header.FileName;
                        case "FILE_SCHEMA":
                            if (Header.FileSchema != null)
                                //set to new schema if it was set before from EntityFactory data
                                Header.FileSchema = new StepFileSchema();
                            return Header.FileSchema;
                        default:
                            return null;
                    }
                }

                if (_entityFactory == null)
                {
                    _entityFactory = _factoryResolver(Header.FileSchema.Schemas);
                    if (_entityFactory == null)
                        throw new XbimParserException($"Entity factory resolver didn't resolve factory for schema '{string.Join(", ", Header.FileSchema.Schemas)}'");
                    InitFromEntityFactory(_entityFactory);
                }

                if (label == null)
                    return _entityFactory.New(name);

                var ent = _entityFactory.New(this, name, (int)label, true);

                // if entity is null do not add so that the file load operation can survive an illegal entity
                // e.g. an abstract class instantiation.
                if (ent != null)
                    _instances.InternalAdd(ent);
                else
                {
                    var msg = $"Error in file at label {label} for type {name}.";
                    if (Metadata.ExpressType(name).Type.GetTypeInfo().IsAbstract)
                    {
                        msg = string.Format("Illegal element in file; cannot instantiate the abstract type {0} at label {1}.", name, label);
                    }
                    Logger?.LogError(msg);
                }

                //make sure that new added entities will have higher labels to avoid any clashes
                if (label >= _instances.CurrentLabel)
                    _instances.CurrentLabel = (int)label;
                return ent;
            };
            try
            {
                parser.Parse();

                //fix header with the schema if it was not a part of the data
                if (Header.FileSchema.Schemas.Count == 0)
                {
                    foreach (var s in _entityFactory.SchemasIds)
                    {
                        Header.FileSchema.Schemas.Add(s);
                    }
                }
            }
            catch (Exception e)
            {
                var position = parser.CurrentPosition;
                throw new XbimParserException(string.Format("Parser failed on line {0}, column {1}", position.EndLine, position.EndColumn), e);
            }

            //fix case if necessary
            for (int i = 0; i < Header.FileSchema.Schemas.Count; i++)
            {
                var id = Header.FileSchema.Schemas[i];
                var sid = _entityFactory.SchemasIds.FirstOrDefault(s => string.Equals(s, id, StringComparison.OrdinalIgnoreCase));
                if (sid == null)
                    throw new XbimParserException("Mismatch between schema defined in the file and schemas available in the data model.");

                //if the case is different set it to the one from entity factory
                if (id != sid)
                    Header.FileSchema.Schemas[i] = sid;
            }

            return parser.ErrorCount;
        }

        /// <summary>
        /// Opens the model from STEP21 file. 
        /// </summary>
        /// <param name="stream">Path to the file</param>
        /// <param name="streamSize"></param>
        /// <param name="progDelegate"></param>
        /// <returns>Number of errors in parsing. Always check this to be null or the model might be incomplete.</returns>
        public virtual int LoadStep21(Stream stream, long streamSize, ReportProgressDelegate progDelegate = null, IEnumerable<string> ignoreTypes = null)
        {
            var parser = new XbimP21Scanner(stream, streamSize)
            {
                Logger = Logger
            };
            if (progDelegate != null) parser.ProgressStatus += progDelegate;
            if (ignoreTypes != null) ignoreTypes.ToList().ForEach(t => parser.SkipTypes.Add(t));

            try
            {
                return LoadStep21(parser);
            }
            finally
            {
                if (progDelegate != null) parser.ProgressStatus -= progDelegate;
            }
        }

        /// <summary>
        /// Opens the model from STEP21 file. 
        /// </summary>
        /// <param name="file">Path to the file</param>
        /// <param name="progDelegate"></param>
        /// <returns>Number of errors in parsing. Always check this to be null or the model might be incomplete.</returns>
        public virtual int LoadStep21(string file, ReportProgressDelegate progDelegate = null)
        {
            using (var stream = File.OpenRead(file))
            {
                var result = LoadStep21(stream, stream.Length, progDelegate);
                return result;
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
                SaveAsStep21(writer, progress);
            }
        }

        /// <summary>
        /// Saves the model as PART21 file
        /// </summary>
        /// <param name="writer">Text writer to be used to write the file</param>
        /// <param name="progress"></param>
        public virtual void SaveAsStep21(TextWriter writer, ReportProgressDelegate progress = null)
        {
            Part21Writer.Write(this, writer, Metadata, new Dictionary<int, int>());
        }

        public virtual void Dispose()
        {
            _instances.Dispose();
            _transactionReference = null;
        }

        /// <summary>
        /// Inserts deep copy of an object into this model. The entity must originate from the same schema (the same EntityFactory). 
        /// This operation happens within a transaction which you have to handle yourself unless you set the parameter "noTransaction" to true.
        /// Insert will happen outside of transactional behaviour in that case. Resulting model is not guaranteed to be valid according to any
        /// model view definition. However, it is granted to be consistent. You can optionally bring in all inverse relationships. Be careful as it
        /// might easily bring in almost full model.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the copied entity</typeparam>
        /// <param name="toCopy">Entity to be copied</param>
        /// <param name="mappings">Mappings of previous inserts</param>
        /// <param name="includeInverses">Option if to bring in all inverse entities (enumerations in original entity)</param>
        /// <param name="keepLabels">Option if to keep entity labels the same</param>
        /// <param name="propTransform">Optional delegate which you can use to filter the content which will get copied over.</param>
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
        /// model view definition. However, it is granted to be consistent. You can optionally bring in all inverse relationships. Be careful as it
        /// might easily bring in almost full model.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the copied entity</typeparam>
        /// <param name="toCopy">Entity to be copied</param>
        /// <param name="mappings">Mappings of previous inserts</param>
        /// <param name="includeInverses">Option if to bring in all inverse entities (enumerations in original entity)</param>
        /// <param name="keepLabels">Option if to keep entity labels the same</param>
        /// <param name="propTransform">Optional delegate which you can use to filter the content which will get copied over.</param>
        /// <param name="noTransaction">If TRUE all operations inside this function will happen out of transaction. 
        /// Also no notifications will be fired from objects.</param>
        /// <returns>Copy from this model</returns>
        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
           bool keepLabels, bool noTransaction) where T : IPersistEntity
        {
            if (noTransaction)
                IsTransactional = false;
            T result;
            try
            {
                result = ModelHelper.InsertCopy(this, toCopy, mappings, propTransform, includeInverses, keepLabels,
                    (type, i) => _instances.New(type, i));
            }
            finally
            {
                //make sure model is transactional at the end again
                IsTransactional = true;
            }
            return result;
        }

        public IEntityCache BeginEntityCaching()
        {
            return EntityCacheReference = new MemoryEntityCache(this);
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

        public XbimSchemaVersion SchemaVersion
        {
            get { return _entityFactory.SchemaVersion; }
        }

        private WeakReference<MemoryEntityCache> _entityCacheReference;
        internal MemoryEntityCache EntityCacheReference
        {
            get
            {
                if (_entityCacheReference == null)
                    return null;
                if (_entityCacheReference.TryGetTarget(out MemoryEntityCache c))
                    return c;
                return null;
            }
            set
            {
                if (value == null)
                {
                    _entityCacheReference = null;
                    return;
                }

                _entityCacheReference = new WeakReference<MemoryEntityCache>(value);
            }
        }
        public IEntityCache EntityCache => EntityCacheReference;

        protected void AddEntityInternal(IPersistEntity entity)
        {
            _instances.InternalAdd(entity);
        }
    }

    /// <summary>
    /// This delegate can be used to implement customized logic in type mapping.
    /// </summary>
    /// <param name="entity">Original entity</param>
    /// <returns>Express type which maps to the type of the original entity</returns>
    public delegate ExpressType TypeResolverDelegate(IPersistEntity entity);

    /// <summary>
    /// Delegate to be used to set up metadata and entity factory from schema IDs
    /// </summary>
    /// <param name="schemas"></param>
    /// <returns></returns>
    public delegate IEntityFactory EntityFactoryResolverDelegate(IEnumerable<string> schemas);
}
