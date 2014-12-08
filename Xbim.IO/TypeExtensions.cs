using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.IO
{
    public static class TypeExtensions
    {
        internal static Type GetItemTypeFromGenericType(this Type genericType)
        {
            if (genericType == typeof(ICoordinateList))
                return typeof(IfcLengthMeasure); //special case for coordinates
            if (genericType.IsGenericType || genericType.IsInterface)
            {
                Type[] genericTypes = genericType.GetGenericArguments();
                if (genericTypes.GetUpperBound(0) >= 0)
                    return genericTypes[genericTypes.GetUpperBound(0)];
                return null;
            }
            if (genericType.BaseType != null)
                return genericType.BaseType.GetItemTypeFromGenericType();
            return null;
        }

        internal static short? IfcTypeId(this Type type)
        {
            return IfcMetaData.IfcTypeId(type);
        }
    }
}
