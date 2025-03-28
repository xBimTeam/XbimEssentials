using System;
using System.Collections.Concurrent;


namespace Xbim.Ifc4.Interfaces
{
    public static class IfcObjectDefinitionExtensions
    {
        // map of an Enum Type to its USERDEFINED entry
        private static ConcurrentDictionary<Type, Enum> UserDefinedEnumCache = new ConcurrentDictionary<Type, Enum>();

        /// <summary>
        /// Returns the Enum with value of "USERDEFINED" for the <typeparamref name="TOut"/> Enum
        /// </summary>
        /// <remarks>A cache is used for efficiency</remarks>
        /// <typeparam name="TOut">The target Enum type</typeparam>
        /// <param name="_">The object - for future use</param>
        /// <returns></returns>
        public static TOut GetUserDefined<TOut>(this IIfcObjectDefinition _) where TOut : Enum
        {
            var enumType = typeof(TOut);
            
            var undefinedEnum = UserDefinedEnumCache.GetOrAdd(enumType, GetUserdefined(enumType));
            if (undefinedEnum == null)
            {
                return default; // Doesn't contain USERDEFINED
            }
            return (TOut)undefinedEnum;
        }

        private static Enum GetUserdefined(Type enumType)
        {
            try
            {
                return (Enum)Enum.Parse(enumType, "USERDEFINED");
            }
            catch { }
            return default;
        }
    }
}
