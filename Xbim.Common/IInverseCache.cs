using System;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.Common
{
    /// <summary>
    /// Inverse relations are one of very important concepts in EXPRESS modeling which allows to mimic 
    /// safe bi-directional relations between two objects. But because EXPRESS model is always flat list of entities
    /// some queries using inverse properties for navigation in model cause exponential cost if the data search.
    /// That is obviously not optimal. Inverse cache should help especially in cases where the IModel implementation
    /// doesn't have any secondary indexing mechanism for inverse relations. It can usually only be used outside
    /// of transaction. Starting transaction when caching is on might raise an exception. You should always keep
    /// inverse cache object inside of using statement. 
    /// </summary>
    public interface IInverseCache: IDisposable
    {
        
    }

    public class InverseCache : IInverseCache
    {
        private readonly Dictionary<CacheKey, IEnumerable<IPersistEntity>> _cache =
            new Dictionary<CacheKey, IEnumerable<IPersistEntity>>();

        public bool TryGet<T>(string inverseProperty, IPersistEntity inverseArgument, out IEnumerable<T> entities)
            where T : IPersistEntity
        {
            var key = new CacheKey(inverseProperty, inverseArgument, typeof(T));
            IEnumerable<IPersistEntity> result;
            if (_cache.TryGetValue(key, out result))
            {
                entities = result.Cast<T>();
                return true;
            }

            entities = Enumerable.Empty<T>();
            return false;
        }

        public void Add<T>(string inverseProperty, IPersistEntity inverseArgument, IEnumerable<T> entities)
            where T: IPersistEntity
        {
            var key = new CacheKey(inverseProperty, inverseArgument, typeof(T));
            _cache.Add(key, entities.Cast<IPersistEntity>());
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;
            _cache.Clear();
            _disposed = true;
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
