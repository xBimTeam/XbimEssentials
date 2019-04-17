using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xbim.Common;

namespace Xbim.Common.Model
{
    internal class EntityCollection : IEntityCollection, IDisposable
    {
        //simpler hasher as all entities will be in this model so no need to use its hash, small performance gain
        private struct EntityLabelComparer : IEqualityComparer<IPersistEntity>
        {
            public bool Equals(IPersistEntity x, IPersistEntity y)
            {
                return x.EntityLabel == y.EntityLabel;
            }

            public int GetHashCode(IPersistEntity obj)
            {
                return obj.EntityLabel;
            }
        }
        private readonly StepModel _model;
        private readonly XbimMultiValueDictionary<Type, IPersistEntity> _internal;
        private readonly Dictionary<int, IPersistEntity> _collection = new Dictionary<int, IPersistEntity>(0x77777);

        private readonly List<int> _naturalOrder = new List<int>(0x77777);
            //about a default of half a million stops too much growing, and 7 is lucky; 

        internal IEntityFactory Factory
        {
            get { return _model.EntityFactory; }
        }

        internal int CurrentLabel;

        public EntityCollection(StepModel model, int labelFrom = 0)
        {
            CurrentLabel = Math.Max(CurrentLabel, labelFrom);
            _model = model;
            _internal =
                XbimMultiValueDictionary<Type, IPersistEntity>.Create(
                    () => new HashSet<IPersistEntity>(new EntityLabelComparer()));
        }

        private IEnumerable<Type> GetQueryTypes(Type type)
        {
            var expType = _model.Metadata.ExpressType(type);
            if (expType != null)
                return expType.NonAbstractSubTypes.Select(t => t.Type);
            
            if(!type.GetTypeInfo().IsInterface) return new List<Type>();

            var implementations = _model.Metadata.ExpressTypesImplementing(type).Where(i => !i.Type.GetTypeInfo().IsAbstract);
            return implementations.Select(e => e.Type);
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument)
            where T : IPersistEntity

        {
            var cache = _model.InverseCache as MemoryInverseCache;
            if (cache == null)
                return Where(condition);

            if (cache.TryGet(inverseProperty, inverseArgument, out IEnumerable<T> result))
                return result.Where(condition);

            //build cache for this type
            lock (cache)
            {
                // check the condition again for case it was computed whilewaiting for access
                if (cache.TryGet(inverseProperty, inverseArgument, out result))
                    return result.Where(condition);

                var indexed = OfType<T>().OfType<IContainsIndexedReferences>().ToList();
                foreach (var item in indexed)
                {
                    foreach (var reference in item.IndexedReferences)
                    {
                        cache.Add(reference.EntityLabel, item);
                    }
                }
            }

            if (cache.TryGet(inverseProperty, inverseArgument, out result))
                return result.Where(condition);

            return Enumerable.Empty<T>();
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition) where T : IPersistEntity
        {
            var queryType = typeof(T);
            //get interface implementations and make sure it doesn't overlap
            var resultTypes = GetQueryTypes(queryType);

            if (condition != null)
            {
                foreach (var type in resultTypes)
                {
                    if (!_internal.TryGetValue(type, out ICollection<IPersistEntity> entities)) continue;
                    foreach (var candidate in entities.Where(c => condition((T) c)))
                        yield return (T) candidate;
                }
            }
            else
            {
                foreach (var type in resultTypes)
                {
                    if (!_internal.TryGetValue(type, out ICollection<IPersistEntity> entities)) continue;
                    foreach (var candidate in entities)
                        yield return (T) candidate;
                }
            }
        }


        public T FirstOrDefault<T>() where T : IPersistEntity
        {
            return OfType<T>().FirstOrDefault();
        }

        public T FirstOrDefault<T>(Func<T, bool> condition) where T : IPersistEntity
        {
            return Where(condition).FirstOrDefault();
        }

        public T FirstOrDefault<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument)
            where T : IPersistEntity
        {
            return Where(condition, inverseProperty, inverseArgument).FirstOrDefault();
        }

        public IEnumerable<T> OfType<T>() where T : IPersistEntity
        {
            var queryType = typeof(T);
            var resultTypes = GetQueryTypes(queryType);
            foreach (var resultType in resultTypes)
            {
                if (_internal.TryGetValue(resultType, out ICollection<IPersistEntity> entities))
                    foreach (var entity in entities)
                        yield return (T)entity;
            }
        }


        public IEnumerable<IPersistEntity> OfType(Type queryType)
        {
            var resultTypes = GetQueryTypes(queryType);
            foreach (var resultType in resultTypes)
            {
                if (_internal.TryGetValue(resultType, out ICollection<IPersistEntity> entities))
                    foreach (var entity in entities)
                        yield return entity;
            }
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            //activation doesn't exist because everything is activated by default
            return OfType<T>();
        }

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            var queryType = _model.Metadata.ExpressType(stringType.ToUpperInvariant());
            if (queryType == null)
                throw new ArgumentException("StringType must be a name of the existing persist entity type");
            foreach (var entity in OfType(queryType.Type))
            {
                yield return entity;
            }
        }

        public IPersistEntity New(Type t)
        {
            var entity = Factory.New(_model, t, Interlocked.Increment(ref CurrentLabel), true);
            AddReversible(entity);
            return entity;
        }

        internal IPersistEntity New(Type t, int label)
        {
            var entity = Factory.New(_model, t, label, true);
            Interlocked.Exchange(ref CurrentLabel, (label >= CurrentLabel) ? label : CurrentLabel);

            AddReversible(entity);
            return entity;
        }

        public T New<T>(Action<T> initPropertiesFunc) where T : IInstantiableEntity
        {
            var entity = Factory.New<T>(_model, Interlocked.Increment(ref CurrentLabel), true);
            AddReversible(entity);
            initPropertiesFunc?.Invoke(entity);
            return entity;
        }

        public T New<T>() where T : IInstantiableEntity
        {

            var entity = Factory.New<T>(_model, Interlocked.Increment(ref CurrentLabel), true);

            AddReversible(entity);
            return entity;
        }

        public IPersistEntity this[int label]
        {
            get
            {
                if (_collection.TryGetValue(label, out IPersistEntity result))
                    return result;
                return null;
            }
        }

        public long Count
        {
            get { return _collection.Count; }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            return OfType<T>().Count();
        }

        internal void InternalAdd(IPersistEntity entity)
        {
            if (entity == null)
                return;

            var key = entity.GetType();
            _internal.Add(key, entity);
            try
            {
                _collection.Add(entity.EntityLabel, entity);
                if (_naturalOrder != null) _naturalOrder.Add(entity.EntityLabel);
            }
            catch (Exception ex)
            {

                var exist = _collection[entity.EntityLabel];
                if (entity.ExpressType != exist.ExpressType)
                    _model.Logger?.LogError($"Duplicate entity #{entity.EntityLabel} with different data type ({exist.ExpressType.Name}/{entity.ExpressType.Name})", ex);
                else
                    _model.Logger?.LogWarning($"Duplicate entity #{entity.EntityLabel}", ex);
            }
        }

        private void AddReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();
            Action undo = () =>
            {
                _internal.Remove(key,entity);
                _collection.Remove(entity.EntityLabel);
                if (_naturalOrder != null) _naturalOrder.Remove(entity.EntityLabel);
            };
            Action doAction = () =>
            {
                _internal.Add(key, entity);
                _collection.Add(entity.EntityLabel, entity);
                if (_naturalOrder != null) _naturalOrder.Add(entity.EntityLabel);
            };

            if (!_model.IsTransactional)
            {
                doAction();
                return;
            }
            
            _model.CurrentTransaction.DoReversibleAction(doAction, undo, entity, ChangeType.New, 0);
        }

        public bool Contains(IPersistEntity entity)
        {
            return _collection.Keys.Contains(entity.EntityLabel);
        }

        internal bool RemoveReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();
            bool removed = false;
            Action doAction = () =>
            {
                _internal.Remove(key,entity);
                removed =_collection.Remove(entity.EntityLabel);
                _naturalOrder?.Remove(entity.EntityLabel);
            };
            Action undo = () =>
            {
                _internal.Add(key,entity);
                _collection.Add(entity.EntityLabel, entity);
                _naturalOrder?.Add(entity.EntityLabel);
            };

            if (!_model.IsTransactional)
            {
                doAction();
                return removed;
            }
            _model.CurrentTransaction.DoReversibleAction(doAction, undo, entity, ChangeType.Deleted, 0);
            return removed;
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            if(_naturalOrder!=null)
                return new NaturalOrderEnumerator(_naturalOrder,_collection);
            return _collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public void Dispose()
        {
            _internal.Clear();
            _collection.Clear();
            if (_naturalOrder != null)
            {
                _naturalOrder.Clear();
            }
        }
              

        private class NaturalOrderEnumerator : IEnumerator<IPersistEntity>
        {
            private readonly Dictionary<int, IPersistEntity> _entities;
            private readonly List<int> _naturalOrder;
            private int _current;
            private IPersistEntity _currentEntity;
            public NaturalOrderEnumerator(List<int> naturalOrder, Dictionary<int,IPersistEntity> entities)
            {
                _naturalOrder = naturalOrder;
                _entities = entities;
                Reset();
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {               
                while (++_current < _naturalOrder.Count )
                {
                    if (_entities.TryGetValue(_naturalOrder[_current], out _currentEntity))
                        return true;
                }
                _currentEntity = null;
                return false;
            }

            public void Reset()
            {
                _current = -1;
                _currentEntity = null;
            }

            public IPersistEntity Current
            {
                get { return _currentEntity; }
            } 

            object IEnumerator.Current
            {
                get { return _currentEntity; }
            }
        }
    }
}
