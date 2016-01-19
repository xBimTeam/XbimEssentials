using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

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

        public static CobieModel OpenStep21(Stream input, bool esentDB = false)
        {
            if (esentDB)
            {
                var esent = new EsentModel(new EntityFactory());
                esent.CreateFrom(input, IfcStorageType.Stp, "temp.xbim", null, true);
                return new CobieModel(esent);
            }

            var model = new MemoryModel(new EntityFactory());
            model.LoadStep21(input);
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

        protected virtual void OnEntityModified(IPersistEntity entity)
        {
            var handler = EntityModified;
            if (handler != null) handler(entity);
        }

        protected virtual void OnEntityDeleted(IPersistEntity entity)
        {
            var handler = EntityDeleted;
            if (handler != null) handler(entity);
        }
        #endregion

        #region Entity created and modified default CreatedInfo assignment
        private CobieCreatedInfo _newInfo;
        private CobieCreatedInfo _modifiedInfo;
        protected virtual void EntityNewCreatedInfo(IPersistEntity entity)
        {
            var refObj = entity as CobieReferencedObject;
            if (refObj != null)
                refObj.Created = _newInfo;
        }

        protected virtual void EntityModifiedCreatedInfo(IPersistEntity entity)
        {
            var refObj = entity as CobieReferencedObject;
            if (refObj != null)
                refObj.Created = _modifiedInfo;
        }

        public void SetDefaultNewEntityInfo(DateTime date, string email, string givenName, string familyName)
        {
            _newInfo = Instances.New<CobieCreatedInfo>(ci =>
            {
                ci.CreatedOn = date;
                ci.CreatedBy =
                    Instances.FirstOrDefault<CobieContact>(
                        c => c.Email == email && c.GivenName == givenName && c.FamilyName == familyName) ??
                    Instances.New<CobieContact>(
                            c =>
                            {
                                c.Email = email;
                                c.GivenName = givenName;
                                c.FamilyName = familyName;
                            });
            });
            EntityNew += EntityNewCreatedInfo;
        }

        public void SetDefaultModifiedEntityInfo(DateTime date, string email, string givenName, string familyName)
        {
            _modifiedInfo = Instances.New<CobieCreatedInfo>(ci =>
            {
                ci.CreatedOn = date;
                ci.CreatedBy =
                    Instances.FirstOrDefault<CobieContact>(
                        c => c.Email == email && c.GivenName == givenName && c.FamilyName == familyName) ??
                    Instances.New<CobieContact>(
                            c =>
                            {
                                c.Email = email;
                                c.GivenName = givenName;
                                c.FamilyName = familyName;
                            });
            });
            EntityModified += EntityModifiedCreatedInfo;
        }
        #endregion

        #region IDispose implementation
        public void Dispose()
        {
            //detach event handlers
            _model.EntityNew -= OnEntityNew;
            _model.EntityDeleted -= OnEntityDeleted;
            _model.EntityModified -= OnEntityModified;

            if (_newInfo != null)
                EntityNew -= EntityNewCreatedInfo;
            if (_modifiedInfo != null)
                EntityModified -= EntityModifiedCreatedInfo;

            //dispose model if it is disposable
            var dispModel = _model as IDisposable;
            if(dispModel != null) dispModel.Dispose();
        }
        #endregion
    }
}
