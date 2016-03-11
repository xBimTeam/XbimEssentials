using System;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.IO
{
    public static class TypeExtensions
    {
        internal static Type GetItemTypeFromGenericType(this Type genericType)
        {
            while (true)
            {
                //if (genericType == typeof (ICoordinateList))
                //    return typeof (IfcLengthMeasure); //special case for coordinates
                if (genericType.IsGenericType || genericType.IsInterface)
                {
                    var genericTypes = genericType.GetGenericArguments();
                    return genericTypes.GetUpperBound(0) >= 0 ? genericTypes[genericTypes.GetUpperBound(0)] : null;
                }
                if (genericType.BaseType == null) return null;
                genericType = genericType.BaseType;
            }
        }

        #region Extensions for .Net40 compatibility

        internal static bool GenericTypeArgumentIsAssignableFrom(this Type genericType, Type assignableType)
        {
            if (genericType.IsGenericType && !genericType.IsGenericTypeDefinition)
            {
                var args = genericType.GetGenericArguments();
                if (args.Length > 0) return args[0].IsAssignableFrom(assignableType);
            }

            return false;

        }

        #endregion
    }
}
