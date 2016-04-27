using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.Common.Federation
{
    public class FederatedModelInstances : IReadOnlyEntityCollection
    {
        readonly IFederatedModel _model;

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            
            foreach (var instance in _model.ReferencingModel.Instances.OfType(stringType, activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType(stringType, activate))
                    yield return instance;
        }

        public FederatedModelInstances(IFederatedModel model)
        {
            _model = model;
           
        }
        public IEnumerable<T> Where<T>(Func<T, bool> expr) where T : IPersistEntity
        {
            foreach (var instance in _model.ReferencingModel.Instances.Where(expr))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.Where(expr))
                    yield return instance;
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument) where T : IPersistEntity
        {
            foreach (var instance in _model.ReferencingModel.Instances.Where(condition, inverseProperty, inverseArgument))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.Where(condition, inverseProperty, inverseArgument))
                    yield return instance;
        }

        public T FirstOrDefault<T>() where T : IPersistEntity
        {
            return OfType<T>().FirstOrDefault();
        }

        public T FirstOrDefault<T>(Func<T, bool> expr) where T : IPersistEntity
        {
            return Where(expr).FirstOrDefault();
        }

        public T FirstOrDefault<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument) where T : IPersistEntity
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> OfType<T>() where T : IPersistEntity
        {
            foreach (var instance in _model.ReferencingModel.Instances.OfType<T>())
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>())
                    yield return instance;
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            foreach (var instance in _model.ReferencingModel.Instances.OfType<T>(activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>(activate))
                    yield return instance;
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
                return _model.ReferencingModel.Instances.Count + _model.ReferencedModels.Sum(refModel => refModel.Model.Instances.Count);
            }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            return _model.ReferencingModel.Instances.CountOf<T>() + _model.ReferencedModels.Sum(refModel => refModel.Model.Instances.CountOf<T>());
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            return _model.ReferencingModel.Instances.Concat(_model.ReferencedModels.SelectMany(rm => rm.Model.Instances)).GetEnumerator();
        }
    }
}
