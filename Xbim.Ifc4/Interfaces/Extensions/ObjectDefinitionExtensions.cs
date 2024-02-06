using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Xbim.Ifc4.Interfaces
{
    public static class ObjectDefinitionExtensions
    {
        private const string PredefinedType = nameof(IIfcPile.PredefinedType);

        // A thread-safe cache between the type and a Getter for its PredefinedType property. By caching getters we elimiminate all Reflection
        static ConcurrentDictionary<Type, Func<object, object>> _predefinedGetterDict = new ConcurrentDictionary<Type, Func<object, object>>();

        /// <summary>
        /// Gets the string value of any PredefinedType property on the <see cref="IIfcObjectDefinition"/> instance, if the type has one; else returns null
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string GetPredefinedTypeValue(this IIfcObjectDefinition instance)
        {
            if (instance is null)
            {
                return null;
            }

            // Locate the getter for the PredefinedType property on this type from the cache, lazily creating one if not present
            var getter = _predefinedGetterDict.GetOrAdd(instance.GetType(), (_) => BuildPredefinedTypeGetter(instance));
            if(getter != null)
            {
                return getter(instance)?.ToString();    // Ouitput is the enum, which can be null
            }
            // this type has no PredefinedType property
            return null;
        }

        /// <summary>
        /// Returns a getter method for the PredefinedType property on the concrete type of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Func<object, object> BuildPredefinedTypeGetter(IIfcObjectDefinition obj)
        {
            var predefinedMetadata = obj.ExpressType.Properties.FirstOrDefault(p => p.Value.Name == PredefinedType).Value;
            if (predefinedMetadata != null)
            {
                // return the getter function - Input: the instance as param. Output: the enum object.
                return predefinedMetadata.PropertyInfo.GetValue;
            }
            return null;
        }

    }
}
