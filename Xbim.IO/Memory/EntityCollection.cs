using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    public class EntityCollection : IEntityCollection, IDisposable
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
        private readonly MemoryModel _model;
        private readonly XbimMultiValueDictionary<Type, IPersistEntity> _internal;
        private readonly Dictionary<int,IPersistEntity> _collection = new Dictionary<int,IPersistEntity>(0x77777);
        private List<int> _naturalOrder = new List<int>(0x77777); //about a default of half a million stops too much growing, and 7 is lucky; 
        internal IEntityFactory Factory
        {
            get { return _model.EntityFactory; }
        }

        internal int NextLabel = 1;
        public EntityCollection(MemoryModel model)
        {
            _model = model;
            _internal = XbimMultiValueDictionary<Type, IPersistEntity>.Create(()=> new HashSet<IPersistEntity>(new EntityLabelComparer()));
        }

        private IEnumerable<Type> GetQueryTypes(Type type)
        {
            var expType = _model.Metadata.ExpressType(type);
            if (expType != null)
                return expType.NonAbstractSubTypes.Select(t => t.Type);
            
            if(!type.IsInterface) return new List<Type>();

            var implementations = _model.Metadata.ExpressTypesImplementing(type).Where(i => !i.Type.IsAbstract);
            return implementations.Select(e => e.Type);
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

            if (condition != null)
            {
                foreach (var type in resultTypes)
                {
                    ICollection<IPersistEntity> entities;
                    if (_internal.TryGetValue(type, out entities))
                        foreach (var candidate in entities.Where(c => condition((T)c)))
                            yield return (T)candidate;
                }
            }
            else
            {
                foreach (var type in resultTypes)
                {
                    ICollection<IPersistEntity> entities;
                    if (_internal.TryGetValue(type, out entities))
                        foreach (var candidate in entities)
                            yield return (T)candidate;
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
                ICollection<IPersistEntity> entities;
                if (_internal.TryGetValue(resultType, out entities))
                    foreach (var entity in entities)
                        yield return (T)entity;
            }
        }
    

    public IEnumerable<IPersistEntity> OfType(Type queryType) 
        {
            var resultTypes = GetQueryTypes(queryType);
            foreach (var resultType in resultTypes)
            {
                ICollection<IPersistEntity> entities;
                if (_internal.TryGetValue(resultType, out entities))
                    foreach (var entity in entities)
                        yield return entity;
            }
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistEntity
        {
            foreach (var entity in OfType<T>())
            {
                if (activate) _model.Activate(entity, true);
                yield return entity;
            }
        }

        public IEnumerable<IPersistEntity> OfType(string stringType, bool activate)
        {
            var queryType = _model.Metadata.ExpressType(stringType.ToUpperInvariant());
            if(queryType == null) 
                throw new ArgumentException("StringType must be a name of the existing persist entity type");
            foreach (var entity in OfType(queryType.Type))
            {
                if (activate) _model.Activate(entity, true);
                yield return entity;
            }
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
                IPersistEntity result;
                
                if (_collection.TryGetValue(label, out result))
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

            //Steve's code bellow wouldn't consider abstract types and interfaces.
            //var queryType = typeof(T);
            //ICollection<IPersistEntity> entities;           
            //if (_internal.TryGetValue(queryType, out entities))
            //     return entities.Count;
            //return 0;
        }

        internal void InternalAdd(IPersistEntity entity)
        {
            var key = entity.GetType();
            _internal.Add(key, entity);
            _collection.Add(entity.EntityLabel,entity);
            if (_naturalOrder != null) _naturalOrder.Add(entity.EntityLabel);
        }

        private void AddReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();
            Action undo = () =>
            {
                _internal.Remove(key,entity);
                _collection.Remove(entity.EntityLabel);
                if (_naturalOrder != null) _naturalOrder.RemoveAt(_naturalOrder.Count-1);
            };
            Action doAction = () =>
            {
                _internal.Add(key, entity);
                _collection.Add(entity.EntityLabel, entity);
                if (_naturalOrder != null) _naturalOrder.Add(entity.EntityLabel);
            };
            doAction();

            if (_model.IsTransactional)
                _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.New, 0);

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
            var oldOrder = _naturalOrder;
            Action doAction = () =>
            {
                _internal.Remove(key,entity);
                removed =_collection.Remove(entity.EntityLabel);
                _naturalOrder = null;
            };
            Action undo = () =>
            {
                _internal.Add(key,entity);
                _collection.Add(entity.EntityLabel, entity);
                _naturalOrder = oldOrder;
            };
            doAction();

            if (_model.IsTransactional) 
                _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.Deleted, 0);
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
            //return _internal.SelectMany(kv => kv.Value, (pair, entity) => entity).GetEnumerator();
            return _collection.GetEnumerator();
        }

        public void Dispose()
        {
            _internal.Clear();
            _collection.Clear();
            _naturalOrder.Clear();            
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
