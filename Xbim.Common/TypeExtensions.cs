using System;
using System.Reflection;

namespace Xbim.IO
{
    public static class TypeExtensions
    {
        public static Type GetItemTypeFromGenericType(this Type genericType)
        {
            while (true)
            {
                //if (genericType == typeof (ICoordinateList))
                //    return typeof (IfcLengthMeasure); //special case for coordinates
                if (genericType.GetTypeInfo().IsGenericType || genericType.GetTypeInfo().IsInterface)
                {
                    var genericTypes = genericType.GetTypeInfo().GetGenericArguments();
                    return genericTypes.GetUpperBound(0) >= 0 ? genericTypes[genericTypes.GetUpperBound(0)] : null;
                }
                if (genericType.GetTypeInfo().BaseType == null) return null;
                genericType = genericType.GetTypeInfo().BaseType;
            }
        }

        #region Extensions for .Net40 compatibility

        public static bool GenericTypeArgumentIsAssignableFrom(this Type genericType, Type assignableType)
        {
            if (genericType.GetTypeInfo().IsGenericType && !genericType.GetTypeInfo().IsGenericTypeDefinition)
            {
                var args = genericType.GetTypeInfo().GetGenericArguments();
                if (args.Length > 0) return args[0].GetTypeInfo().IsAssignableFrom(assignableType);
            }

            return false;

        }

        #endregion
    }
}
