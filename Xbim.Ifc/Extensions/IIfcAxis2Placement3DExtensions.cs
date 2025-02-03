using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcAxis2Placement3DExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement3D obj, ConcurrentDictionary<int, object> maps = null)
        {
            if (maps == null)
                return obj.ConvertAxis3D();

            object transform;
            if (maps.TryGetValue(obj.EntityLabel, out transform)) //already converted it just return cached
                return (XbimMatrix3D)transform;

            transform = obj.ConvertAxis3D();
            maps.TryAdd(obj.EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }

        private static XbimMatrix3D ConvertAxis3D(this IIfcAxis2Placement3D obj)
        {
            if (obj.RefDirection == null || obj.Axis == null)
                return new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, obj.Location.X, obj.Location.Y,
                    obj.Location.Z, 1);

            var za = obj.Axis.XbimVector3D();
            za.Normalized();
            var xa = obj.RefDirection.XbimVector3D();
            xa.Normalized();
            var ya = XbimVector3D.CrossProduct(za, xa);
            ya.Normalized();
            return new XbimMatrix3D(xa.X, xa.Y, xa.Z, 0, ya.X, ya.Y, ya.Z, 0, za.X, za.Y, za.Z, 0, obj.Location.X,
                obj.Location.Y, obj.Location.Z, 1);
        }

    }
}
