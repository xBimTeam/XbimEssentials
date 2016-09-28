using System.Collections.Concurrent;
using System.Collections.Generic;
using Xbim.Common.Geometry;

namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcAxis2Placement2D
    {
        public XbimMatrix3D ToMatrix3D(ConcurrentDictionary<int, object> maps = null)
        {
            object transform;
            if (maps != null && maps.TryGetValue(EntityLabel, out transform)) //already converted it just return cached
                return (XbimMatrix3D)transform;
            if (RefDirection != null)
            {
                XbimVector3D v = RefDirection.XbimVector3D();
                v.Normalized();
                transform = new XbimMatrix3D(v.X, v.Y, 0, 0, v.Y, v.X, 0, 0, 0, 0, 1, 0, Location.X, Location.Y, 0, 1);
            }
            else
                transform = new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, Location.X, Location.Y,
                                    Location.Z, 1);
            if (maps != null) maps.TryAdd(EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }
    }
}
