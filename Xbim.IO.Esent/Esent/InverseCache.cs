using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;

namespace Xbim.IO.Esent
{
    public class InverseCache : IInverseCache
    {
        private readonly Dictionary<CacheKey, IEnumerable<IPersistEntity>> _cache =
            new Dictionary<CacheKey, IEnumerable<IPersistEntity>>();

        public bool TryGet<T>(string inverseProperty, IPersistEntity inverseArgument, out IEnumerable<T> entities)
            where T : IPersistEntity
        {
            var key = new CacheKey(inverseProperty, inverseArgument, typeof(T));
            if (_cache.TryGetValue(key, out IEnumerable<IPersistEntity> result))
            {
                entities = result.Cast<T>();
                return true;
            }

            entities = Enumerable.Empty<T>();
            return false;
        }

        public void Add<T>(string inverseProperty, IPersistEntity inverseArgument, IEnumerable<T> entities)
            where T : IPersistEntity
        {
            var key = new CacheKey(inverseProperty, inverseArgument, typeof(T));
            _cache.Add(key, entities.Cast<IPersistEntity>());
        }

        private bool _disposed;

        public int Size => _cache.Count;

        public void Dispose()
        {
            if (_disposed)
                return;
            _cache.Clear();
            _disposed = true;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        private class CacheKey
        {
            private readonly string _property;
            private readonly IPersistEntity _agument;
            private readonly Type _type;

            public CacheKey(string property, IPersistEntity agument, Type type)
            {
                _property = property;
                _agument = agument;
                _type = type;
            }

            public override int GetHashCode()
            {
                return _property.GetHashCode() + _agument.EntityLabel + _type.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var key = obj as CacheKey;
                if (key == null)
                    return false;
                return _agument.EntityLabel == key._agument.EntityLabel && _type == key._type &&
                       string.Equals(_property, key._property, StringComparison.Ordinal);
            }
        }
    }
}
