using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;

namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcAxis2Placement3D
    {
        public XbimMatrix3D ToMatrix3D(ConcurrentDictionary<int, object> maps = null)
        {
            if (maps == null)
                return ConvertAxis3D();

            object transform;
            if (maps.TryGetValue(EntityLabel, out transform)) //already converted it just return cached
                return (XbimMatrix3D)transform;
            
            transform = ConvertAxis3D();
            maps.TryAdd(EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }

        private XbimMatrix3D ConvertAxis3D()
        {
            if (RefDirection == null || Axis == null)
                return new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, Location.X, Location.Y,
                    Location.Z, 1);

            var za = Axis.XbimVector3D();
            za.Normalized();
            var xa = RefDirection.XbimVector3D();
            xa.Normalized();
            var ya = XbimVector3D.CrossProduct(za, xa);
            ya.Normalized();
            return new XbimMatrix3D(xa.X, xa.Y, xa.Z, 0, ya.X, ya.Y, ya.Z, 0, za.X, za.Y, za.Z, 0, Location.X,
                Location.Y, Location.Z, 1);
        }

        public new IfcDimensionCount Dim
        {
            get { return (int)((long)(base.Dim)); }
        }

        List<XbimVector3D> IfcAxis2Placement.P
        {
            get
            {
                var a = new XbimVector3D(P[0].X,P[0].Y,P[0].Z);
                var b = new XbimVector3D(P[1].X,P[1].Y,P[1].Z);              
                var c = new XbimVector3D(P[2].X,P[2].Y,P[2].Z);
                return new List<XbimVector3D> { a, b, c };
            }
        }

    }
}
