using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    public class EntityCollection<TFactory> : IEntityCollection, IDisposable where TFactory : IEntityFactory, new()
    {
        private readonly IModel _model;
        private readonly Dictionary<Type, List<IPersistEntity>> _internal = new Dictionary<Type, List<IPersistEntity>>();
        private readonly Type[] _types;
        internal readonly TFactory Factory;
        internal int NextLabel = 1;
        public EntityCollection(IModel model)
        {
            _model = model;
            var mainType = typeof (IPersistEntity);
            _types = mainType.Assembly.GetTypes().Where(t => mainType.IsAssignableFrom(t)).ToArray();
            Factory = new TFactory();
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistEntity
        {
            var queryType = typeof(T);
            var condition = expr != null ? expr.Compile() : null;
            var resultTypes = _internal.Keys.Where(t => queryType.IsAssignableFrom(t));
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => (T)entity)
                    .Where(result => condition == null || condition(result));
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
            var queryType = typeof(T);
            var resultTypes = _internal.Keys.Where(t => queryType.IsAssignableFrom(t));
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => (T) entity);
        }

        public IEnumerable<IPersistEntity> OfType(Type queryType) 
        {
            if (!(typeof(IPersistEntity).IsAssignableFrom(queryType)))
                throw new Exception("Type must be assignable from IPersistEntity");
            var resultTypes = _internal.Keys.Where(queryType.IsAssignableFrom);
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => entity);
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
            var queryType = _types.FirstOrDefault(t => t.Name.ToLower() == stringType.ToLower());
            if(queryType == null) 
                throw new ArgumentException("StringType must be a name of the existing model type");
            
            var resultTypes = _internal.Keys.Where(t => queryType.IsAssignableFrom(t));
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => entity);
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
                return _internal.Values
                    .Select(list => list.FirstOrDefault(e => e.EntityLabel == label))
                    .FirstOrDefault(entity => entity != null);
            }
        }

        public long Count
        {
            get { return _internal.Values.Aggregate(0, (c, l)=> c + l.Count); }
        }

        public long CountOf<T>() where T : IPersistEntity
        {
            var queryType = typeof(T);
            var resultTypes = _internal.Keys.Where(t => queryType.IsAssignableFrom(t));
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => (T)entity).Count();
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
        }

        private void AddReversible(IPersistEntity entity)
        {
            if (_model.IsTransactional && _model.CurrentTransaction == null) throw new Exception("Operation out of transaction");
            var key = entity.GetType();

            List<IPersistEntity> list;
            if (_internal.TryGetValue(key, out list))
            {
                Action undo = () => list.Remove(entity);
                Action doAction = () => list.Add(entity);
                doAction();

                if (!_model.IsTransactional) return;
                _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.New);
            }
            else
            {
                Action doAction = () => _internal.Add(key, new List<IPersistEntity> { entity });
                Action undo = () => _internal.Remove(key);
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

            Action doAction = () => _internal[key].Remove(entity);
            Action undo = () => _internal[key].Add(entity);
            doAction();

            if (!_model.IsTransactional) return true;
            _model.CurrentTransaction.AddReversibleAction(doAction, undo, entity, ChangeType.Deleted);
            return true;
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            return _internal.SelectMany(kv => kv.Value, (pair, entity) => entity).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internal.SelectMany(kv => kv.Value, (pair, entity) => entity).GetEnumerator();
        }

        public void Dispose()
        {
            _internal.Clear();
        }
    }
}
