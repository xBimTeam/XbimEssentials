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
        public IEntityFactory EntityFactory { get; private set; }
        private readonly EntityFactoryResolverDelegate _factoryResolver;


        public object Tag { get; set; }
        public int UserDefinedId { get; set; }

        public StepModel(IEntityFactory entityFactory, int labelFrom)
        {
            Logger = Logger ?? XbimLogging.CreateLogger<StepModel>();
            InitFromEntityFactory(entityFactory);

            _instances = new EntityCollection(this, labelFrom);

            IsTransactional = true;
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);

            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults, this);
            foreach (var schemasId in EntityFactory.SchemasIds)
                Header.FileSchema.Schemas.Add(schemasId);
        }

        public StepModel(IEntityFactory entityFactory) : this(entityFactory, 0)
        {
        }

        public StepModel(IEntityFactory entityFactory, ILogger logger = null, int labelFrom = 0) : this(entityFactory, labelFrom)
        {
            Logger = logger ?? XbimLogging.CreateLogger<StepModel>();
        }

        public StepModel(EntityFactoryResolverDelegate factoryResolver, ILogger logger = null, int labelFrom = 0)
        {
            Logger = logger ?? XbimLogging.CreateLogger<StepModel>();
            _factoryResolver = factoryResolver;
            _instances = new EntityCollection(this, labelFrom);
            IsTransactional = true;
            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);
        }

        private void InitFromEntityFactory(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory ?? throw new ArgumentNullException("entityFactory");
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
                    (_metadata = ExpressMetaData.GetMetadata(EntityFactory.GetType().GetTypeInfo().Module));
            }
        }

        

        public virtual void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            foreach (var entity in source)
            {
                body(entity);
            }
        }

        private List<NewEntityHandler> _newEntityHandlers = new List<NewEntityHandler>();
        private List<ModifiedEntityHandler> _modifiedEntityHandlers = new List<ModifiedEntityHandler>();
        private List<DeletedEntityHandler> _deletedEntityHandlers = new List<DeletedEntityHandler>();

        private event NewEntityHandler _entityNew;
        private event ModifiedEntityHandler _entityModified;
        private event DeletedEntityHandler _entityDeleted;

        /// <summary>
        /// This event is fired every time new entity is created.
        /// </summary>
        public event NewEntityHandler EntityNew
        {
            add
            {
                _entityNew += value;
                _newEntityHandlers.Add(value);
            }
            remove
            {
                _entityNew -= value;
                _newEntityHandlers.RemoveAll(v => v.Equals(value));
            }
        }

        /// <summary>
        /// This event is fired every time any entity is modified. If your model is not
        /// transactional it might not be called at all as the central point for all
        /// modifications is a transaction.
        /// </summary>
        public event ModifiedEntityHandler EntityModified
        {
            add
            {
                _entityModified += value;
                _modifiedEntityHandlers.Add(value);
            }
            remove
            {
                _entityModified -= value;
                _modifiedEntityHandlers.RemoveAll(v => v.Equals(value));
            }
        }

        /// <summary>
        /// This event is fired every time when entity gets deleted from model.
        /// </summary>
        public event DeletedEntityHandler EntityDeleted
        {
            add
            {
                _entityDeleted += value;
                _deletedEntityHandlers.Add(value);
            }
            remove
            {
                _entityDeleted -= value;
                _deletedEntityHandlers.RemoveAll(v => v.Equals(value));
            }
        }


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
                        _entityNew?.Invoke(entity);
                    break;
                case ChangeType.Deleted:
                        _entityDeleted?.Invoke(entity);
                    break;
                case ChangeType.Modified:
                        _entityModified?.Invoke(entity, propertyOrder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("changeType", changeType, null);
            }
        }

        public int LoadStep21Part(Stream data)
        {
            var parser = new XbimP21Scanner(data, -1);
            return LoadStep21(parser);
        }

        public int LoadStep21Part(string data)
        {
            var parser = new XbimP21Scanner(data);
            return LoadStep21(parser);
        }


        public static IStepFileHeader LoadStep21Header(Stream stream)
        {
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, null);
            var scanner = new XbimP21Scanner(stream, 1000);
            
            scanner.EntityCreate += (string name, long? label, bool inHeader) =>
            {
                if (inHeader)
                {
                    switch (name)
                    {
                        case "FILE_DESCRIPTION":
                            return header.FileDescription;
                        case "FILE_NAME":
                            return header.FileName;
                        case "FILE_SCHEMA":
                            if (header.FileSchema != null)
                                //set to new schema if it was set before from EntityFactory data
                                header.FileSchema = new StepFileSchema();
                            return header.FileSchema;
                        default:
                            return null;
                    }
                }
                else
                    return null;
            };
            try
            {
                scanner.Parse(true);
            }
            catch (Exception e)
            {
                var position = scanner.CurrentPosition;
                throw new XbimParserException(string.Format("Parser failed on line {0}, column {1}", position.EndLine, position.EndColumn), e);
            }
            return header;
        }

        protected virtual int LoadStep21(XbimP21Scanner parser)
        {
            if (Header == null)
                Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);

            if (EntityFactory == null && _factoryResolver == null)
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

                if (EntityFactory == null)
                {
                    EntityFactory = _factoryResolver(Header.FileSchema.Schemas);
                    if (EntityFactory == null)
                        throw new XbimParserException($"Entity factory resolver didn't resolve factory for schema '{string.Join(", ", Header.FileSchema.Schemas)}'");
                    InitFromEntityFactory(EntityFactory);
                }

                if (label == null)
                    return EntityFactory.New(name);

                var ent = EntityFactory.New(this, name, (int)label, true);

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
                    foreach (var s in EntityFactory.SchemasIds)
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

            // if the model is empty, having just a header, entity factory might still be empty
            if (EntityFactory == null)
            {
                EntityFactory = _factoryResolver(Header.FileSchema.Schemas);
                if (EntityFactory == null)
                    throw new XbimParserException($"Entity factory resolver didn't resolve factory for schema '{string.Join(", ", Header.FileSchema.Schemas)}'");
                InitFromEntityFactory(EntityFactory);
            }

            //fix case if necessary
            for (int i = 0; i < Header.FileSchema.Schemas.Count; i++)
            {
                var id = Header.FileSchema.Schemas[i];
                
                           
                var sid = EntityFactory.SchemasIds.FirstOrDefault(s => id.StartsWith(s, StringComparison.OrdinalIgnoreCase));
                if (sid == null)
                {
                    //add in a bit of flexibility for old Ifc models with weird schema names
                    var old2xSchemaNamesThatAreOK = new[] { "IFC2X2_FINAL", "IFC2X2" };
                    if(old2xSchemaNamesThatAreOK.FirstOrDefault(s => string.Equals(s, id, StringComparison.OrdinalIgnoreCase))==null)
                        throw new XbimParserException("Mismatch between schema defined in the file and schemas available in the data model.");
                    else
                        sid = EntityFactory.SchemasIds.FirstOrDefault(s => string.Equals(s, "IFC2X3", StringComparison.OrdinalIgnoreCase));
                }
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
        /// <param name="ignoreTypes">Types to ignore</param>
        /// <returns>Number of errors in parsing. Always check this to be null or the model might be incomplete.</returns>
        public virtual int LoadStep21(Stream stream, long streamSize, ReportProgressDelegate progDelegate = null, IEnumerable<string> ignoreTypes = null)
        {
            var parser = new XbimP21Scanner(stream, streamSize, ignoreTypes);
            if (progDelegate != null) parser.ProgressStatus += progDelegate;
            try
            {
                return LoadStep21(parser);
            }
            catch
            {
                throw;
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

        /// <summary>
        /// Extension point for inheriting classes
        /// </summary>
        protected virtual void Disposing() { }

        public virtual void Dispose()
        {
            _instances.Dispose();
            _transactionReference = null;
            _cacheReference = null;
            _entityCacheReference = null;

            // detach all listeners
            _newEntityHandlers.ToList().ForEach(h => EntityNew -= h);
            _modifiedEntityHandlers.ToList().ForEach(h => EntityModified -= h);
            _deletedEntityHandlers.ToList().ForEach(h => EntityDeleted -= h);
            _newEntityHandlers.Clear();
            _modifiedEntityHandlers.Clear();
            _deletedEntityHandlers.Clear();

            Disposing();
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
            catch
            {
                throw;
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
            get { return EntityFactory.SchemaVersion; }
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
