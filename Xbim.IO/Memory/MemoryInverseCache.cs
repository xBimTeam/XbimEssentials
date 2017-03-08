using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    internal class MemoryInverseCache : IInverseCache
    {
        private Dictionary<Type, Dictionary<int, HashSet<int>>> _index = new Dictionary<Type, Dictionary<int, HashSet<int>>>();
        private EntityCollection _entities;

        public MemoryInverseCache(EntityCollection entities)
        {
            _entities = entities;
        }

        internal void Add(int key, IPersistEntity value)
        {
            var index = GetIndex(value.GetType());
            HashSet<int> set;
            if (!index.TryGetValue(key, out set))
            {
                set = new HashSet<int>();
                index.Add(key, set);
            }
            set.Add(value.EntityLabel);
        }

        private Dictionary<int, HashSet<int>> GetIndex(Type type)
        {
            Dictionary<int, HashSet<int>> result;
            if (_index.TryGetValue(type, out result))
                return result;

            result = new Dictionary<int, HashSet<int>>();
            _index.Add(type, result);
            return result;
        }

        public void Add<T>(string inverseProperty, IPersistEntity inverseArgument, IEnumerable<T> entities) where T : IPersistEntity
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _index.Clear();
        }

        public bool TryGet<T>(string inverseProperty, IPersistEntity inverseArgument, out IEnumerable<T> entities) where T : IPersistEntity
        {
            var type = typeof(T);
            //get candidate types
            var keys = _index.Keys.Where(k => type.IsAssignableFrom(k)).ToArray();
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
            HashSet<int> labels;
            if (index.TryGetValue(key, out labels))
            {
                foreach (var label in labels)
                {
                    yield return (T)_entities[label];
                }
            }
        }
    }
}
