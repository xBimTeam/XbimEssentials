using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xbim.Common;

namespace Xbim.IO.Esent
{
    public class XbimFederatedModelInstances : IEntityCollection
    {
        readonly EsentModel _model;

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            
            foreach (var instance in _model.InstancesLocal.OfType(stringType, activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType(stringType, activate))
                    yield return instance;
        }

        public XbimFederatedModelInstances(EsentModel model)
        {
            _model = model;
        }
        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            foreach (var instance in _model.InstancesLocal.Where(expr))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.Where(expr))
                    yield return instance;
        }

        public T FirstOrDefault<T>() where T : IPersistEntity
        {
            return OfType<T>().FirstOrDefault();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            return Where(expr).FirstOrDefault();
        }

        public IEnumerable<T> OfType<T>() where T : IPersistEntity
        {
            foreach (var instance in _model.InstancesLocal.OfType<T>())
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>())
                    yield return instance;
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            foreach (var instance in _model.InstancesLocal.OfType<T>(activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>(activate))
                    yield return instance;
        }

        public IPersistEntity New(Type t)
        {
            return _model.InstancesLocal.New(t);
        }

        public T New<T>(Action<T> initPropertiesFunc) where T : IInstantiableEntity
        {
            return _model.InstancesLocal.New(initPropertiesFunc);
        }

        public T New<T>() where T : IInstantiableEntity
        {
            return _model.InstancesLocal.New<T>();
        }

       

        /// <summary>
        /// returns the local instance with the given label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public IPersistEntity this[int label]
        {
            get { return _model.InstancesLocal[label]; }
        }
        /// <summary>
        /// Returns the instance that corresponds to this handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IPersistEntity this[XbimInstanceHandle handle]
        {
            get { return handle.GetEntity(); }
        }

        public long Count
        {
            get
            {
                return _model.InstancesLocal.Count + _model.ReferencedModels.Sum(refModel => refModel.Model.Instances.Count);
            }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            return _model.InstancesLocal.CountOf<T>() + _model.ReferencedModels.Sum(refModel => refModel.Model.Instances.CountOf<T>());
        }

        /// <summary>
        /// returns the geometry from the local instances
        /// Does not access federated model geometry
        /// </summary>
        /// <param name="geometryLabel"></param>
        /// <returns></returns>
        public IPersistEntity GetFromGeometryLabel(int geometryLabel)
        {
            var filledGeomData = _model.Cache.GetGeometryHandle(geometryLabel);
            return _model.Cache.GetInstance(filledGeomData.ProductLabel, true, true);
        }

       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            return _model.InstancesLocal.Concat(_model.ReferencedModels.SelectMany(rm => rm.Model.Instances)).GetEnumerator();
        }
    }
}
