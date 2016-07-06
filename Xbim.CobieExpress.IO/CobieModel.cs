using System;
using System.Collections.Generic;
using System.IO;
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

        private CobieModel(IModel model)
        {
            _model = model;

            if(EsentModel == null && MemoryModel == null) throw new ArgumentException("Model has to be either MemoryModel or EsentModel");

            _esentDB = _model is EsentModel;
            InitEvents();
        }

        public CobieModel()
        {
            _model = new MemoryModel(new EntityFactory());
            InitEvents();
        }

        public CobieModel(bool esentDB)
        {
            _esentDB = esentDB;
            if(esentDB)
                _model = new EsentModel(new EntityFactory());
            else
                _model = new MemoryModel(new EntityFactory());
            
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
            var model = new MemoryModel(new EntityFactory(), labelFrom);
            model.LoadStep21(input, streamSize);
            return new CobieModel(model);
        }

        public static CobieModel OpenStep21(string input, bool esentDB = false)
        {
            if (esentDB)
            {
                var db = Path.ChangeExtension(input, ".xcobie");
                var esent = new EsentModel(new EntityFactory());
                esent.CreateFrom(input, db, null, true);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(new EntityFactory());
            model.LoadStep21(input);
            return new CobieModel(model);
        }

        public static CobieModel OpenStep21(Stream input, long streamSize,bool esentDB = false)
        {
            if (esentDB)
            {
                var esent = new EsentModel(new EntityFactory());
                esent.CreateFrom(input, streamSize, IfcStorageType.Stp, "temp.xbim", null, true);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(new EntityFactory());
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

        public void ExportToTable(string file, out string report, ModelMapping mapping = null)
        {
            var ext = (Path.GetExtension(file) ?? "").ToLower();
            if (ext != ".xls" && ext != ".xlsx")
                file = Path.ChangeExtension(file, ".xlsx");

            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(this, mapping);
            storage.Store(file);
            report = storage.Log.ToString();
        }

        public void ExportToTable(Stream file, ExcelTypeEnum typeEnum, out string report, ModelMapping mapping = null)
        {
            mapping = mapping ?? ModelMapping.Load(Properties.Resources.COBieUK2012);
            var storage = GetTableStore(this, mapping);
            storage.Store(file, typeEnum);
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
                var esent = new EsentModel(new EntityFactory());
                esent.CreateFrom(input, db, null, true);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(new EntityFactory());
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
        public bool Activate(IPersistEntity owningEntity, bool write)
        {
            return _model.Activate(owningEntity, write);
        }

        public void Activate(IPersistEntity entity, int depth)
        {
            _model.Activate(entity, depth);
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
