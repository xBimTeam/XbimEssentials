using System.Collections.Concurrent;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcAxis2Placement2DExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement2D obj, ConcurrentDictionary<int, object> maps = null)
        {
            object transform;
            if (maps != null && maps.TryGetValue(obj.EntityLabel, out transform)) //already converted it just return cached
                return (XbimMatrix3D)transform;
            if (obj.RefDirection != null)
            {
                XbimVector3D v = obj.RefDirection.XbimVector3D();
                v.Normalized();
                transform = new XbimMatrix3D(v.X, v.Y, 0, 0, v.Y, v.X, 0, 0, 0, 0, 1, 0, obj.Location.X, obj.Location.Y, 0, 1);
            }
            else
                transform = new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, obj.Location.X, obj.Location.Y,
                                    obj.Location.Z, 1);
            if (maps != null) maps.TryAdd(obj.EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }

    }
}
