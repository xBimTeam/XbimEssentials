using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.CobieExpress.IO.Resolvers;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;
using Xbim.IO.TableStore;

namespace Xbim.CobieExpress.IO
{
    public class CobieModel : IModel, IDisposable
    {
        private readonly bool _esentDB;
        private readonly IModel _model;
        private EsentModel EsentModel { get { return _model as EsentModel; } }
        private MemoryModel MemoryModel { get { return _model as MemoryModel; } }

        private static readonly IEntityFactory factory = new EntityFactoryCobieExpress();

        public CobieModel(IModel model)
        {
            _model = model;

            if(EsentModel == null && MemoryModel == null) throw new ArgumentException("Model has to be either MemoryModel or EsentModel");

            _esentDB = _model is EsentModel;
            InitEvents();
        }

        public CobieModel()
        {
            _esentDB = false;
            _model = new MemoryModel(factory);
            InitEvents();
        }

        public CobieModel(bool esentDB)
        {
            _esentDB = esentDB;
            if(esentDB)
                _model = EsentModel.CreateTemporaryModel(factory);
            else
                _model = new MemoryModel(factory);
            
            InitEvents();
        }

        public object Tag { get; set; }

        /// <summary>
        /// This constructor only opens an in memory model
        /// </summary>
        /// <param name="input"></param>
        /// <param name="labelFrom"></param>
        /// <returns></returns>
        public static CobieModel OpenStep21(Stream input, long streamSize, int labelFrom)
        {
            var model = new MemoryModel(factory, labelFrom);
            model.LoadStep21(input, streamSize);
            return new CobieModel(model);
        }

        public static CobieModel OpenStep21(string input, bool esentDB = false)
        {
            if (esentDB)
            {
                var db = Path.ChangeExtension(input, ".xbim");
                var esent = new EsentModel(factory);
                esent.CreateFrom(input, db, null, true, true, IfcStorageType.Stp);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(factory);
            model.LoadStep21(input);
            return new CobieModel(model);
        }

        public static CobieModel OpenStep21(Stream input, long streamSize,bool esentDB = false)
        {
            if (esentDB)
            {
                var esent = new EsentModel(factory);
                esent.CreateFrom(input, streamSize, IfcStorageType.Stp, "temp.xbim", null, true);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(factory);
            model.LoadStep21(input, streamSize);
            return new CobieModel(model);
        }

        public void SaveAsStep21(string file)
        {
            if (_esentDB)
            {
                EsentModel.SaveAs(file, IfcStorageType.Stp);
                return;
            }

            using (var stream = File.Create(file))
            {
                MemoryModel.SaveAsStep21(stream);
                stream.Close();
            }
        }

        public static CobieModel OpenEsent(string esentDB)
        {
            var model = new EsentModel(factory);
            model.Open(esentDB, XbimDBAccess.ReadWrite);
            return new CobieModel(model);
        }

        public void SaveAsEsent(string dbName)
        {
            if (EsentModel != null && string.Equals(EsentModel.DatabaseName, dbName, StringComparison.OrdinalIgnoreCase))
            {
                //it is ESENT model already and all changes are persisted automatically.
            }
            else
            {
                using (var esent = new EsentModel(factory))
                {
                    esent.CreateFrom(_model, dbName);
                    esent.Close();
                }
            }
        }

        public void ExportToTable(string file, out string report, ModelMapping mapping = null, Stream template = null)
        {
            var ext = (Path.GetExtension(file) ?? "").ToLower();
            if (ext != ".xls" && ext != ".xlsx")
                file = Path.ChangeExtension(file, ".xlsx");

            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(this, mapping);
            storage.Store(file, template);
            report = storage.Log.ToString();
        }

        public void ExportToTable(Stream file, ExcelTypeEnum typeEnum, out string report, ModelMapping mapping = null, Stream template = null)
        {
            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(this, mapping);
            storage.Store(file, typeEnum, template);
            report = storage.Log.ToString();
        }


        public static CobieModel ImportFromTable(string file, out string report, ModelMapping mapping = null)
        {
            var loaded = new CobieModel();
            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(loaded, mapping);
            using (var txn = loaded.BeginTransaction("Loading XLSX"))
            {
                storage.LoadFrom(file);

                //assign all levels to facility because COBie XLS standard contains this implicitly
                var facility = loaded.Instances.FirstOrDefault<CobieFacility>();
                var floors = loaded.Instances.OfType<CobieFloor>().ToList();
                if (facility != null && floors.Any())
                    floors.ForEach(f => f.Facility = facility);

                txn.Commit();
            }

            report = storage.Log.ToString();
            return loaded;
        }

        public static CobieModel ImportFromTable(Stream file, ExcelTypeEnum typeEnum, out string report, ModelMapping mapping = null)
        {
            var loaded = new CobieModel();
            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(loaded, mapping);
            using (var txn = loaded.BeginTransaction("Loading XLSX"))
            {
                storage.LoadFrom(file, typeEnum);
                txn.Commit();
            }

            report = storage.Log.ToString();
            return loaded;
        }

        private static TableStore GetTableStore(IModel model, ModelMapping mapping)
        {
            var storage = new TableStore(model, mapping);
            storage.Resolvers.Add(new AttributeTypeResolver());
            return storage;
        }

        public void SaveAsStep21Zip(string file)
        {
            if (_esentDB)
            {
                EsentModel.SaveAs(file, IfcStorageType.StpZip);
                return;
            }

            using (var stream = File.Create(file))
            {
                MemoryModel.SaveAsStep21Zip(stream);
                stream.Close();
            }
        }

        public static CobieModel OpenStep21Zip(string input, bool esentDB = false)
        {
            if (esentDB)
            {
                var db = Path.ChangeExtension(input, ".xcobie");
                var esent = new EsentModel(factory);
                esent.CreateFrom(input, db, null, true, true, IfcStorageType.StpZip);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(factory);
            model.LoadZip(input);
            return new CobieModel(model);
        }

        public void InsertCopy(IEnumerable<CobieComponent> components, bool keepLabels, XbimInstanceHandleMap mappings)
        {
            foreach (var component in components)
            {
                InsertCopy(component, mappings, InsertCopyComponentFilter, true, keepLabels);
            }
        }

        private object InsertCopyComponentFilter(ExpressMetaProperty property, object parentObject)
        {
            if (!property.IsInverse) 
                return property.PropertyInfo.GetValue(parentObject, null);
            
            if (property.Name == "InSystems")
                return property.PropertyInfo.GetValue(parentObject, null);

            return null;
        }

        #region IModel implementation using inner model
        public int UserDefinedId 
        { 
            get { return _model.UserDefinedId; }
            set { _model.UserDefinedId = value; } 
        }

        public IGeometryStore GeometryStore
        {
            get { return _model.GeometryStore; }
        }
        public IStepFileHeader Header { get { return _model.Header; } }
        public bool IsTransactional { get { return _model.IsTransactional; } }
        public IList<XbimInstanceHandle> InstanceHandles { get { return _model.InstanceHandles; } }
        public IEntityCollection Instances {
            get { return _model.Instances; }
        }
        public bool Activate(IPersistEntity owningEntity)
        {
            return _model.Activate(owningEntity);
        }

        public void Delete(IPersistEntity entity)
        {
            _model.Delete(entity);
        }

        public ITransaction BeginTransaction(string name)
        {
            return _model.BeginTransaction(name);
        }

        public ITransaction CurrentTransaction {
            get { return _model.CurrentTransaction; }
        }
        public ExpressMetaData Metadata {
            get { return _model.Metadata; }
        }
        public IModelFactors ModelFactors {
            get { return _model.ModelFactors; }
        }

        public T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform, bool includeInverses,
            bool keepLabels) where T : IPersistEntity
        {
            return _model.InsertCopy(toCopy, mappings, propTransform, includeInverses, keepLabels);
        }

        public void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity
        {
            _model.ForEach(source, body);
        }

        public event NewEntityHandler EntityNew;
        public event ModifiedEntityHandler EntityModified;
        public event DeletedEntityHandler EntityDeleted;
        public IInverseCache BeginCaching()
        {
            return _model.BeginCaching();
        }

        public void StopCaching()
        {
            _model.StopCaching();
        }

        public IInverseCache InverseCache
        {
            get { return _model.InverseCache; }
        }

        public IfcSchemaVersion SchemaVersion
        {
            get { return _model.SchemaVersion; }
        }

        private void InitEvents()
        {
            _model.EntityNew += OnEntityNew;
            _model.EntityDeleted += OnEntityDeleted;
            _model.EntityModified += OnEntityModified;
        }

        protected virtual void OnEntityNew(IPersistEntity entity)
        {
            var handler = EntityNew;
            if (handler != null) handler(entity);
        }

        protected virtual void OnEntityModified(IPersistEntity entity, int property)
        {
            var handler = EntityModified;
            if (handler != null) handler(entity, property);
        }

        protected virtual void OnEntityDeleted(IPersistEntity entity)
        {
            var handler = EntityDeleted;
            if (handler != null) handler(entity);
        }
        #endregion

        #region Entity created and modified default CreatedInfo assignment
        private CobieCreatedInfo _entityInfo;
        private bool _ownChange;
        private IPersistEntity _lastEntity;

        protected virtual void SetEntityCreatedInfo(IPersistEntity entity, int property)
        {
            SetEntityCreatedInfo(entity);
        }

        protected virtual void SetEntityCreatedInfo(IPersistEntity entity)
        {
            if (_ownChange)
            {
                if (ReferenceEquals(_lastEntity, entity))
                    return;
                _ownChange = false;
            }
            var refObj = entity as CobieReferencedObject;
            if (refObj == null) return;

            _ownChange = true;
            _lastEntity = entity;
            refObj.Created = _entityInfo;
        }

        public CobieCreatedInfo SetDefaultEntityInfo(DateTime date, string email, string givenName, string familyName)
        {
            _entityInfo = Instances.New<CobieCreatedInfo>(ci =>
            {
                ci.CreatedOn = date;
                ci.CreatedBy =
                    Instances.FirstOrDefault<CobieContact>(
                        c => c.Email == email && c.GivenName == givenName && c.FamilyName == familyName) ??
                    Instances.New<CobieContact>(
                            c =>
                            {
                                c.Created = ci;
                                c.Email = email;
                                c.GivenName = givenName;
                                c.FamilyName = familyName;
                            });
            });
            EntityNew += SetEntityCreatedInfo;
            EntityModified += SetEntityCreatedInfo;
            return _entityInfo;
        }
        #endregion

        #region IDispose implementation
        public void Dispose()
        {
            //detach event handlers
            _model.EntityNew -= OnEntityNew;
            _model.EntityDeleted -= OnEntityDeleted;
            _model.EntityModified -= OnEntityModified;

            if (_entityInfo != null)
                EntityNew -= SetEntityCreatedInfo;

            //dispose model if it is disposable
            var dispModel = _model as IDisposable;
            if(dispModel != null) dispModel.Dispose();
        }
        #endregion
    }
}
