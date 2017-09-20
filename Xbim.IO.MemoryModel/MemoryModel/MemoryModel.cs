using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO.MemoryModel;
using Xbim.IO.Parser;
using Xbim.IO.Step21;
using Xbim.IO.Xml;
using Xbim.IO.Xml.BsConf;


namespace Xbim.IO.Memory
{
    public class MemoryModel : IModel, IDisposable
    {
        private ILogger _logger;
        public ILogger Logger { get { return _logger; } set { _logger = value; } }
        private static ZipArchiveEntry GetZipEntry(Stream fileStream)
        {
            using (var zipStream = new ZipArchive(fileStream))
            {
                return zipStream.Entries.FirstOrDefault(z => z.Name.IsStepTextFile()); //ignores xbim and zip files
            }
        }

        public static XbimSchemaVersion GetSchemaVersion(string fileName)
        {
            // need to get the header for each step file storage type
            //if it is a zip, xml or ifc text
            if (fileName.IsStepTextFile())
            {
                using (var fileStream = File.OpenRead(fileName))
                {
                    return GetStepFileXbimSchemaVersion(fileStream);
                }
            }
            else if (fileName.IsStepZipFile())
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(fileName))
                    {
                        var entry = archive.Entries.FirstOrDefault(z => z.Name.IsStepTextFile());
                        if (entry == null) throw new FileLoadException($"File does not contain a valid model: {fileName}");
                        using (var reader = entry.Open())
                        {
                            if (entry.Name.IsStepTextFile())
                                return GetStepFileXbimSchemaVersion(reader);
                            if (entry.Name.IsStepXmlFile())
                            {
                                XmlSchemaVersion schema;
                                using (var xml = XmlReader.Create(reader))
                                {
                                    schema = XbimXmlReader4.ReadSchemaVersion(xml);
                                }

                                switch (schema)
                                {
                                    case XmlSchemaVersion.Ifc2x3:
                                        return XbimSchemaVersion.Ifc2X3;
                                    case XmlSchemaVersion.Ifc4Add1:
                                    case XmlSchemaVersion.Ifc4:
                                        return XbimSchemaVersion.Ifc4;
                                    case XmlSchemaVersion.Unknown:
                                        return XbimSchemaVersion.Unsupported;
                                }
                            }
                            else
                                throw new FileLoadException($"File does not contain a valid model: {fileName}");
                        }
                    }
                }
                catch (Exception)
                {
                    throw new FileLoadException($"File is an invalid zip format: {fileName}");
                }
            }

            else if (fileName.IsStepXmlFile())
            {
                using (var fileStream = File.OpenRead(fileName))
                {
                    XmlSchemaVersion schema;
                    using (var reader = XmlReader.Create(fileStream))
                    {
                        schema = XbimXmlReader4.ReadSchemaVersion(reader);
                    }

                    switch (schema)
                    {
                        case XmlSchemaVersion.Ifc2x3:
                            return XbimSchemaVersion.Ifc2X3;
                        case XmlSchemaVersion.Ifc4Add1:
                        case XmlSchemaVersion.Ifc4:
                            return XbimSchemaVersion.Ifc4;
                        case XmlSchemaVersion.Unknown:
                            return XbimSchemaVersion.Unsupported;
                    }

                }
            }

            throw new FileLoadException($"File is an invalid model format: {fileName}");
        }

        //public static IStepFileHeader GetStepFileHeader(Stream stream, IModel model)
        //{
        //    var parser = new XbimP21Parser(stream, null, -1);

        //    var stepHeader = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, model);
        //    parser.EntityCreate += (string name, long? label, bool header, out int[] ints) =>
        //    {
        //        //allow all attributes to be parsed
        //        ints = null;
        //        if (header)
        //        {
        //            switch (name)
        //            {
        //                case "FILE_DESCRIPTION":
        //                    return stepHeader.FileDescription;
        //                case "FILE_NAME":
        //                    return stepHeader.FileName;
        //                case "FILE_SCHEMA":
        //                    return stepHeader.FileSchema;
        //                default:
        //                    return null;
        //            }
        //        }
        //        parser.Cancel = true; //done enough
        //        return null;
        //    };
        //    parser.Parse();
        //    return stepHeader;
        //}

        public static XbimSchemaVersion GetStepFileXbimSchemaVersion(IEnumerable<string> schemas)
        {
            foreach (var schema in schemas)
            {
                if (string.Compare(schema, "Ifc4", StringComparison.OrdinalIgnoreCase) == 0)
                    return XbimSchemaVersion.Ifc4;
                if (schema.StartsWith("Ifc2x", StringComparison.OrdinalIgnoreCase)) //return this as 2x3
                    return XbimSchemaVersion.Ifc2X3;
                if (schema.StartsWith("Cobie2X4", StringComparison.OrdinalIgnoreCase)) //return this as Cobie
                    return XbimSchemaVersion.Cobie2X4;
            }
            return XbimSchemaVersion.Unsupported;
        }

        public static XbimSchemaVersion GetStepFileXbimSchemaVersion(Stream stream)
        {
            return GetStepFileXbimSchemaVersion(GetStepFileSchemaVersion(stream));
        }

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
        private readonly EntityFactoryResolverDelegate _factoryResolver;

        internal IEntityFactory EntityFactory { get { return _entityFactory; } }

        public object Tag { get; set; }
        public int UserDefinedId { get; set; }

        public MemoryModel(IEntityFactory entityFactory, int labelFrom)
        {
            InitFromEntityFactory(entityFactory);

            _instances = new EntityCollection(this, labelFrom);

            IsTransactional = true;
            ModelFactors = new XbimModelFactors(Math.PI / 180, 1e-3, 1e-5);

            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults, this);
            foreach (var schemasId in _entityFactory.SchemasIds)
                Header.FileSchema.Schemas.Add(schemasId);
        }

        public MemoryModel(IEntityFactory entityFactory) : this(entityFactory, 0)
        {
        }

        public MemoryModel(IEntityFactory entityFactory, ILogger logger = null, int labelFrom = 0) : this(entityFactory, labelFrom)
        {
            Logger = logger;
        }

        private MemoryModel(EntityFactoryResolverDelegate factoryResolver, ILogger logger = null, int labelFrom = 0)
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
            Metadata = ExpressMetaData.GetMetadata(entityFactory.GetType().GetTypeInfo().Module);
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

        public IStepFileHeader Header { get; private set; }

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
                var reader3 = new XbimXmlReader3(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) reader3.ProgressStatus += progDelegate;
                Header = reader3.Read(stream, this);
                if (progDelegate != null) reader3.ProgressStatus -= progDelegate;
            }
            else
            {
                var xmlReader = new XbimXmlReader4(GetOrCreateXMLEntity, entity => { }, Metadata);
                if (progDelegate != null) xmlReader.ProgressStatus += progDelegate;
                Header = xmlReader.Read(stream, this);
                if (progDelegate != null) xmlReader.ProgressStatus -= progDelegate;
            }

            if (Header.FileSchema.Schemas == null)
                Header.FileSchema.Schemas = new List<string>();
            if (!Header.FileSchema.Schemas.Any())
                Header.FileSchema.Schemas.Add(schema);

            //purge
            _read.Clear();
        }

        private readonly Dictionary<int, IPersistEntity> _read = new Dictionary<int, IPersistEntity>();
        private IPersistEntity GetOrCreateXMLEntity(int label, Type type)
        {
            IPersistEntity exist;
            if (_read.TryGetValue(label, out exist))
                return exist;

            var ent = _entityFactory.New(this, type, label, true);
            _instances.InternalAdd(ent);
            _read.Add(label, ent);
            return ent;
        }

        public virtual void LoadZip(string file, ReportProgressDelegate progDelegate = null)
        {
            using (var stream = File.OpenRead(file))
            {
                LoadZip(stream, progDelegate);
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
            using (var zipStream = new ZipArchive(stream))
            {
                var zipContent = zipStream.Entries.FirstOrDefault(z => z.Name.IsStepTextFile()); //ignores xbim and zip files
                if (zipContent.Name.IsStepTextFile())
                {
                    using (var reader = zipContent.Open())
                    {
                        LoadStep21(reader, zipContent.Length, progDelegate);
                    }
                }
                else if (zipContent.Name.IsStepXmlFile())
                {
                    using (var reader = zipContent.Open())
                    {
                        LoadXml(reader, zipContent.Length, progDelegate);
                    }
                }
            }
        }

        public int LoadStep21Part(Stream data)
        {
            var parser = new PartialParser(data, Metadata)
            {
                Logger = Logger
            };
            return LoadStep21Part(parser);
        }

        public int LoadStep21Part(string data)
        {
            var parser = new PartialParser(data, Metadata)
            {
                Logger = Logger
            };
            return LoadStep21Part(parser);
        }


        private int LoadStep21Part(PartialParser parser)
        {
            if (Header == null)
                Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);

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
                            if (Header.FileSchema != null)
                                //set to new schema if it was set before from EntityFactory data
                                Header.FileSchema = new StepFileSchema();
                            return Header.FileSchema;
                        default:
                            return null;
                    }
                }

                if (label == null)
                    return _entityFactory.New(name);

                var typeId = Metadata.ExpressTypeId(name);
                var ent = _entityFactory.New(this, typeId, (int)label, true);

                // if entity is null do not add so that the file load operation can survive an illegal entity
                // e.g. an abstract class instantiation.
                if (ent != null)
                    _instances.InternalAdd(ent);
                else
                {
                    var msg = $"Error in file at label {label} for type {name}.";
                    if (Metadata.ExpressType(typeId).Type.GetTypeInfo().IsAbstract)
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
                        Header.FileSchema.Schemas.Add(s.ToUpperInvariant());
                    }
                }
            }
            catch (Exception e)
            {
                var position = parser.CurrentPosition;
                throw new XbimParserException(string.Format("Parser failed on line {0}, column {1}", position.EndLine, position.EndColumn), e);
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
        public virtual int LoadStep21(Stream stream, long streamSize, ReportProgressDelegate progDelegate = null)
        {
            var parser = new XbimP21Parser(stream, Metadata, streamSize)
            {
                Logger = Logger
            };
            if (progDelegate != null) parser.ProgressStatus += progDelegate;
            var first = true;
            Header = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, this);
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

                if (_entityFactory == null)
                {
                    if (_factoryResolver == null)
                        throw new XbimParserException("EntityFactory is not defined and no resolver is specified to create one. Data can't be created.");
                    _entityFactory = _factoryResolver(Header.FileSchema.Schemas);
                    if (_entityFactory == null)
                        throw new XbimParserException($"Entity factory resolver didn't resolve factory for schema '{string.Join(", ", Header.FileSchema.Schemas)}'");
                    InitFromEntityFactory(_entityFactory);
                }

                if (label == null)
                    return _entityFactory.New(name);
                //if this is a first non-header entity header is read completely by now. 
                //So we should check if the schema declared in the file is the one declared in EntityFactory
                if (first)
                {
                    first = false;
                    //fix case if necessary
                    for (int i = 0; i < Header.FileSchema.Schemas.Count; i++)
                    {
                        var id = Header.FileSchema.Schemas[i];
                        var sid = _entityFactory.SchemasIds.FirstOrDefault(s => string.Equals(s, id, StringComparison.OrdinalIgnoreCase));
                        if (sid == null)
                            throw new Exception("Mismatch between schema defined in the file and schemas available in the data model.");

                        //if the case is different set it to the one from entity factory
                        if (id != sid)
                            Header.FileSchema.Schemas[i] = sid;
                    }
                }

                var typeId = Metadata.ExpressTypeId(name);
                var ent = _entityFactory.New(this, typeId, (int)label, true);

                // if entity is null do not add so that the file load operation can survive an illegal entity
                // e.g. an abstract class instantiation.
                if (ent != null)
                    _instances.InternalAdd(ent);
                else
                {
                    var msg = $"Error in file at label {label} for type {name}.";
                    if (Metadata.ExpressType(typeId).Type.GetTypeInfo().IsAbstract)
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
            }
            catch (Exception e)
            {
                var position = parser.CurrentPosition;
                throw new XbimParserException(string.Format("Parser failed on line {0}, column {1}", position.EndLine, position.EndColumn), e);
            }

            if (progDelegate != null) parser.ProgressStatus -= progDelegate;
            return parser.ErrorCount;
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
        public static MemoryModel OpenRead(string fileName, ReportProgressDelegate progressDel)
        {
            return OpenRead(fileName, null, progressDel);
        }
        public static MemoryModel OpenRead(string fileName)
        {
            return OpenRead(fileName, null, null);
        }
        public static MemoryModel OpenRead(string fileName, ILogger logger, ReportProgressDelegate progressDel = null)
        {
            //step21 text file can resolve version in parser
            if (fileName.IsStepTextFile())
            {
                using (var file = File.OpenRead(fileName))
                {
                    return OpenReadStep21(file, logger, progressDel);
                }
            }


            var version = GetSchemaVersion(fileName); //an exception is thrown if this fails

            switch (version)
            {

                case XbimSchemaVersion.Ifc4:
                    var mm4 = new MemoryModel(new Ifc4.EntityFactory(), logger);

                    if (fileName.IsStepZipFile())
                        mm4.LoadZip(fileName, progressDel);
                    else if (fileName.IsStepXmlFile())
                        mm4.LoadXml(fileName, progressDel);
                    else
                        throw new FileLoadException($"Unsupported file type extension: {Path.GetExtension(fileName)}");

                    return mm4;

                case XbimSchemaVersion.Ifc2X3:
                    var mm2x3 = new MemoryModel(new Xbim.Ifc2x3.EntityFactory(), logger);
                    if (fileName.IsStepZipFile())
                        mm2x3.LoadZip(fileName, progressDel);
                    else if (fileName.IsStepXmlFile())
                        mm2x3.LoadXml(fileName, progressDel);
                    else
                        throw new FileLoadException($"Unsupported file type extension: {Path.GetExtension(fileName)}");

                    return mm2x3;

                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new FileLoadException($"Unsupported file format: {fileName}");
            }
        }

        /// <summary>
        /// Reads schema version fron the file on the fly inside the parser so it doesn't need to
        /// access the file twice.
        /// </summary>
        /// <param name="file">Input step21 text file</param>
        /// <param name="logger">Logger</param>
        /// <param name="progressDel">Progress delegate</param>
        /// <returns>New memory model</returns>
        public static MemoryModel OpenReadStep21(string file, ILogger logger = null, ReportProgressDelegate progressDel = null)
        {
            using (var stream = File.OpenRead(file))
            {
                return OpenReadStep21(stream, logger, progressDel);
            }
        }

        /// <summary>
        /// Reads schema version fron the stream on the fly inside the parser so it doesn't need to
        /// access the file twice.
        /// </summary>
        /// <param name="stream">Input stream for step21 text file</param>
        /// <param name="logger">Logger</param>
        /// <param name="progressDel">Progress delegate</param>
        /// <returns>New memory model</returns>
        public static MemoryModel OpenReadStep21(Stream stream, ILogger logger = null, ReportProgressDelegate progressDel = null)
        {
            var model = new MemoryModel((IEnumerable<string> schemas) => {
                var schema = GetStepFileXbimSchemaVersion(schemas);
                switch (schema)
                {
                    case XbimSchemaVersion.Ifc4:
                        return new Ifc4.EntityFactory();
                    case XbimSchemaVersion.Ifc2X3:
                        return new Ifc2x3.EntityFactory();
                    case XbimSchemaVersion.Cobie2X4:
                    case XbimSchemaVersion.Unsupported:
                    default:
                        return null;
                }
            }, logger);
            model.LoadStep21(stream, stream.Length, progressDel);
            return model;
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

            }
        }

        private IEnumerable<IPersistEntity> GetXmlOrderedEntities(string schema)
        {
            if (schema != null && schema.ToUpper().Contains("COBIE"))
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
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var schema = _entityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifcxml" : ".xml";
                var newEntry = zipStream.CreateEntry($"data{ext}");
                using (var writer = newEntry.Open())
                {
                    SaveAsXml(writer, xmlSettings, xbimSettings, configuration, progress);
                }

            }
        }

        public virtual void SaveAsStep21Zip(Stream stream, ReportProgressDelegate progress = null)
        {
            using (var zipStream = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var schema = _entityFactory.SchemasIds.FirstOrDefault();
                var ext = schema != null && schema.StartsWith("IFC") ? ".ifc" : ".stp";
                var newEntry = zipStream.CreateEntry($"data{ext}");
                using (var writer = newEntry.Open())
                {
                    SaveAsStep21(writer, progress);
                }

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

        public void Dispose()
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
