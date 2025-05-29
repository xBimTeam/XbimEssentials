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
        // A thread-safe cache between the type and a Setter for its PredefinedType property. By caching setters we elimiminate all Reflection
        static ConcurrentDictionary<Type, (Type, Action<object, object>)> _predefinedSetterDict = new ConcurrentDictionary<Type, (Type, Action<object, object>)>();

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
        /// Sets the PredefinedType property on the <see cref="IIfcObjectDefinition"/> instance, if the type has one.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <returns><c>true</c> if set was successful else <c>false</c></returns>
        public static bool SetPredefinedTypeValue(this IIfcObjectDefinition instance, string value)
        {
            if (instance is null)
            {
                return false;
            }

            //// Locate the setter for the PredefinedType property on this type from the cache, lazily creating one if not present
            var (type, setter) = _predefinedSetterDict.GetOrAdd(instance.GetType(), (_) => BuildPredefinedTypeSetter(instance));
            if(setter != null)
            {
                object enumValue = GetEnumValue(instance, value, type);
                if(enumValue != null)
                {
                    setter(instance, enumValue);
                    return true;
                }
            }
            // this type has no PredefinedType property, or the enum was not applicable
            return false;
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> is a known pre-defined enum on the <paramref name="instance"/>. 
        /// The test is case-insensitive.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <returns><c>true</c> if the value matches an enum; else <c>false</c></returns>
        public static bool IsPredefinedTypeEnum(this IIfcObjectDefinition instance, string value)
        {
            if (instance is null)
            {
                return false;
            }
            var (type, setter) = _predefinedSetterDict.GetOrAdd(instance.GetType(), (_) => BuildPredefinedTypeSetter(instance));
            if (setter != null)
            {
                return GetEnumValue(instance, value, type) != null;
            }
            return false;
        }

        private static object GetEnumValue(IIfcObjectDefinition instance, string value, Type enumType)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (enumType != null && !string.IsNullOrEmpty(value))
            {
                try
                {
                    if(IsNullable(enumType))
                    {
                        enumType = Nullable.GetUnderlyingType(enumType);
                    }
                    var pdt = Enum.Parse(enumType, value, true);
                    return pdt;
                }
                catch (System.ArgumentException) { }
                catch (System.OverflowException) { }

            }
            return null;
        }

        private static bool IsNullable(Type type)
        {
            if(type.IsValueType)
            {
                // value types are only nullable if they are Nullable<T>
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }
            else
            {
                return true;
            }
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

        /// <summary>
        /// Returns the Type and its setter method for the PredefinedType property on the concrete type of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static (Type, Action<object, object>) BuildPredefinedTypeSetter(IIfcObjectDefinition obj)
        {
            var predefinedMetadata = obj.ExpressType.Properties.FirstOrDefault(p => p.Value.Name == PredefinedType).Value;
            if (predefinedMetadata != null)
            {
                var type = predefinedMetadata.PropertyInfo.PropertyType;
                // return the Type and the setter function - Input: 1) the instance as param 2) enum object. Output=void
                return (type, predefinedMetadata.PropertyInfo.SetValue);
            }
            return (null, null);
        }

    }
}
