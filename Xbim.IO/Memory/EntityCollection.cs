using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    public class EntityCollection : IEntityCollection, IDisposable
    {
        private readonly MemoryModel _model;
        private readonly Dictionary<Type, List<IPersistEntity>> _internal = new Dictionary<Type, List<IPersistEntity>>();
        private readonly KeyedEntityCollection _collection = new KeyedEntityCollection(); 
        internal IEntityFactory Factory{get { return _model.EntityFactory; }}
        internal int NextLabel = 1;
        public EntityCollection(MemoryModel model)
        {
            _model = model;
        }

        private List<Type> GetQueryTypes(Type type)
        {
            var expType = _model.Metadata.ExpressType(type);
            if (expType != null)
                return expType.NonAbstractSubTypes.ToList();
            
            if(!type.IsInterface) return new List<Type>();

            var implementations = _model.Metadata.ExpressTypesImplementing(type).Where(i => !i.Type.IsAbstract);
            return implementations.Select(e => e.Type).ToList();
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument)
            where T : IPersistEntity

        {
            return Where(condition);
        }

        public IEnumerable<T> Where<T>(Func<T, bool> condition) where T : IPersistEntity
        {
            var queryType = typeof(T);
            //get interface implementations and make sure it doesn't overlap
            var resultTypes = GetQueryTypes(queryType);

            foreach (var type in resultTypes)
            {
                List<IPersistEntity> candidtes;
                if (!_internal.TryGetValue(type, out candidtes)) continue;

                if(condition == null)
                    foreach (var candidte in candidtes)
                    {
                        yield return (T)candidte;
                    }
                else
                {
                    foreach (var candidte in candidtes.Where(c => condition((T)c)))
                    {
                        yield return (T)candidte;
                    }
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

        public T FirstOrDefault<T>(Func<T, bool> condition, string inverseProperty, IPersistEntity inverseArgument) where T : IPersistEntity
        {
            return FirstOrDefault(condition);
        }

        public IEnumerable<T> OfType<T>() where T : IPersistEntity
        {
            var queryType = typeof(T);
            var resultTypes = GetQueryTypes(queryType);
            foreach (var resultType in resultTypes)
            {
                List<IPersistEntity> subresult;
                if (!_internal.TryGetValue(resultType, out subresult)) continue;
                foreach (var entity in subresult)
                {
                    yield return (T)entity;
                }
            }
        }

        public IEnumerable<IPersistEntity> OfType(Type queryType) 
        {
            var resultTypes = GetQueryTypes(queryType);
            foreach (var resultType in resultTypes)
            {
                List<IPersistEntity> subresult;
                if (!_internal.TryGetValue(resultType, out subresult)) continue;
                foreach (var entity in subresult)
                {
                    yield return entity;
                }
            }
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            foreach (var entity in OfType<T>())
            {
                _model.Activate(entity, true);
                yield return entity;
            }
        }

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            var queryType = _model.Metadata.ExpressType(stringType.ToUpperInvariant());
            if(queryType == null) 
                throw new ArgumentException("StringType must be a name of the existing persist entity type");
            return OfType(queryType.Type);
        }

        public IPersistEntity New(Type t)
        {
            var entity = Factory.New(_model, t, NextLabel++, true);
            AddReversible(entity);
            return entity;
        }

        internal IPersistEntity New(Type t, int label)
        {
            var entity = Factory.New(_model, t, label, true);
            if (label >= NextLabel)
                NextLabel = label + 1;

            AddReversible(entity);
            return entity;
        }

        public T New<T>(Action<T> initPropertiesFunc) where T : IInstantiableEntity
        {
            var entity = Factory.New(_model, initPropertiesFunc, NextLabel++, true);
            AddReversible(entity);
            return entity;
        }

        public T New<T>() where T : IInstantiableEntity
        {
            var entity = Factory.New<T>(_model, NextLabel++, true);
            AddReversible(entity);
            return entity;
        }

        public IPersistEntity this[int label]
        {
            get
            {
                return _collection[label];
            }
        }

        public long Count
        {
            get { return _internal.Values.Aggregate(0, (c, l)=> c + l.Count); }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            var queryType = typeof(T);
            return OfType(queryType).Count();
        }

        internal void InternalAdd(IPersistEntity entity)
        {
            var key = entity.GetType();
            List<IPersistEntity> list;
            if (_internal.TryGetValue(key, out list))
            {
                list.Add(entity);
            }
            else
            {
                _internal.Add(key, new List<IPersistEntity> { entity });
            }
            _collection.Add(entity);
        }

        private void AddReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();

            List<IPersistEntity> list;
            if (_internal.TryGetValue(key, out list))
            {
                Action undo = () =>
                {
                    list.Remove(entity);
                    _collection.Remove(entity);
                };
                Action doAction = () =>
                {
                    list.Add(entity);
                    _collection.Add(entity);
                };
                doAction();

                if (!_model.IsTransactional) return;
                _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.New);
            }
            else
            {
                Action doAction = () =>
                {
                    _internal.Add(key, new List<IPersistEntity> {entity});
                    _collection.Add(entity);
                };
                Action undo = () =>
                {
                    _internal.Remove(key);
                    _collection.Remove(entity);
                };
                doAction();

                if (!_model.IsTransactional) return;
                _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.New);
            }

        }

        internal bool RemoveReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();

            if (!_internal.ContainsKey(key) || !_internal[key].Contains(entity))
                return false;

            Action doAction = () =>
            {
                _internal[key].Remove(entity);
                _collection.Remove(entity);
            };
            Action undo = () =>
            {
                _internal[key].Add(entity);
                _collection.Add(entity);
            };
            doAction();

            if (!_model.IsTransactional) return true;
            _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.Deleted);
            return true;
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            //return _internal.SelectMany(kv => kv.Value, (pair, entity) => entity).GetEnumerator();
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            //return _internal.SelectMany(kv => kv.Value, (pair, entity) => entity).GetEnumerator();
            return _collection.GetEnumerator();
        }

        public void Dispose()
        {
            _internal.Clear();
            _collection.Clear();
        }

        private class KeyedEntityCollection : KeyedCollection<int, IPersistEntity>
        {
            protected override int GetKeyForItem(IPersistEntity item)
            {
                return item.EntityLabel;
            }
        }
    }
}
