using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xbim.Common.Model
{
    public class MemoryInverseCache : IInverseCache
    {
        private Dictionary<Type, Dictionary<int, HashSet<int>>> _index = new Dictionary<Type, Dictionary<int, HashSet<int>>>();
        private IEntityCollection _entities;

        public MemoryInverseCache(IEntityCollection entities)
        {
            _entities = entities;
        }

        private void Add(int key, IPersistEntity value)
        {
            var index = GetIndex(value.GetType());
            if (!index.TryGetValue(key, out HashSet<int> set))
            {
                set = new HashSet<int>();
                index.Add(key, set);
            }
            set.Add(value.EntityLabel);
        }

        private Dictionary<int, HashSet<int>> GetIndex(Type type)
        {
            if (_index.TryGetValue(type, out Dictionary<int, HashSet<int>> result))
                return result;

            result = new Dictionary<int, HashSet<int>>();
            _index.Add(type, result);
            return result;
        }

        private bool _disposed;

        public int Size {
            get
            {
                var count = 0;
                foreach (var item in _index)
                {
                    foreach (var kvp in item.Value)
                    {
                        count += kvp.Value.Count;
                    }
                }
                return count;
            }
        }

        public bool IsDisposed => _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _index.Clear();
            _index = null;
            _entities = null;
            _disposed = true;
        }

        public bool TryGet<T>(string inverseProperty, IPersistEntity inverseArgument, out IEnumerable<T> entities) where T : IPersistEntity
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            if (TryGetPrivate(inverseProperty, inverseArgument, out entities))
                return true;

            //build cache for this type
            lock (_index)
            {
                // check the condition again for case it was computed while waiting for access
                if (TryGetPrivate(inverseProperty, inverseArgument, out entities))
                    return true;

                var indexed = inverseArgument.Model.Instances.OfType<T>()
                    .OfType<IContainsIndexedReferences>().ToList();
                foreach (var item in indexed)
                {
                    foreach (var reference in item.IndexedReferences)
                    {
                        Add(reference.EntityLabel, item);
                    }
                }
            }

            if (TryGetPrivate(inverseProperty, inverseArgument, out entities))
                return true;

            entities = null;
            return false;
        }

        private bool TryGetPrivate<T>(string inverseProperty, IPersistEntity inverseArgument, out IEnumerable<T> entities) where T : IPersistEntity
        {
            var type = typeof(T);
            //get candidate types
            var keys = _index.Keys.Where(k => type.GetTypeInfo().IsAssignableFrom(k)).ToArray();
            if (keys.Length == 0)
            {
                entities = Enumerable.Empty<T>();
                return false;
            }
            entities = keys.SelectMany(k => Get<T>(k, inverseArgument.EntityLabel));
            return true;
        }

        private IEnumerable<T> Get<T>(Type type, int key) where T : IPersistEntity
        {
            var index = GetIndex(type);
            if (index.TryGetValue(key, out HashSet<int> labels))
            {
                foreach (var label in labels)
                {
                    yield return (T)_entities[label];
                }
            }
        }

        public void Clear()
        {
            _index.Clear();
        }
    }
}
