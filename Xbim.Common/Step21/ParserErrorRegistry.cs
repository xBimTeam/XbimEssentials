using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Common.Step21
{
    public class ParserErrorRegistry : IDisposable
    {
        private ConcurrentDictionary<string, int[]> _typesNotFound = new ConcurrentDictionary<string, int[]>();
        private ConcurrentDictionary<PropertyError, int[]> _propertyErrors = new ConcurrentDictionary<PropertyError, int[]>();

        public int TypesNotFound => _typesNotFound.Count;
        public int PropertyErrors => _propertyErrors.Count;

        public bool AddTypeNotCreated(string type)
        {
            type = type.ToUpperInvariant();
            if (_typesNotFound.TryGetValue(type, out int[] count))
            {
                count[0]++;
                return false;
            }
            return _typesNotFound.TryAdd(type, new[] { 1 });
        }

        public bool AddPropertyNotSet(IPersist entity, int propIndex, object value, Exception exception)
        {
            var err = new PropertyError(entity, propIndex, value, exception);
            if (_propertyErrors.TryGetValue(err, out int[] count))
            {
                count[0]++;
                return false;
            }

            return _propertyErrors.TryAdd(err, new[] { 1 });
        }

        public bool Any => TypesNotFound != 0 || PropertyErrors != 0;

        public string Summary
        {
            get
            {
                if (!Any)
                    return "";

                using (var w = new StringWriter())
                {
                    foreach (var kvp in _typesNotFound)
                        w.WriteLine($"Type '{kvp.Key}' not found {kvp.Value[0]} times.");
                    foreach (var kvp in _propertyErrors)
                    {
                        var err = kvp.Key;
                        w.WriteLine($"Property {err.Property} of {err.Entity.GetType().Name} failed to set value {err.Value} {kvp.Value[0]} times with exception {err.Exception.GetType().Name}: {err.Exception.Message}");
                    }
                    return w.ToString();
                }
            }
        }

        public void Dispose()
        {
            _typesNotFound.Clear();
        }

        private struct PropertyError
        {
            public readonly IPersist Entity;
            public readonly int Property;
            public readonly Exception Exception;
            public readonly object Value;

            public PropertyError(IPersist entity, int property, object value, Exception exception)
            {
                Entity = entity;
                Property = property;
                Exception = exception;
                Value = value;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is PropertyError err))
                    return false;
                return
                    err.Entity.GetType() == Entity.GetType() &&
                    err.Value?.GetType() == Value?.GetType() &&
                    err.Property == Property &&
                    err.Exception?.GetType() == Exception?.GetType();
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 31 + Entity.GetType().GetHashCode();
                hash = hash * 31 + Property.GetHashCode();
                if (Exception != null)
                    hash = hash * 31 + Exception.GetType().GetHashCode();
                if (Value != null)
                    hash = hash * 31 + Value.GetType().GetHashCode();
                return hash;
            }
        }
    }
}
