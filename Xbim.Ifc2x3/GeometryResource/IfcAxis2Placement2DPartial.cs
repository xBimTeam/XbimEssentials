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
                v.Normalize();
                transform = new XbimMatrix3D(v.X, v.Y, 0, 0, v.Y, v.X, 0, 0, 0, 0, 1, 0, Location.X, Location.Y, 0, 1);
            }
            else
                transform = new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, Location.X, Location.Y,
                                    Location.Z, 1);
            if (maps != null) maps.TryAdd(EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }

        public new int Dim
        {
            get { return (int)((long)(base.Dim)); }
        }

        List<IfcDirection> IfcAxis2Placement.P
        {
            get
            {
                var a = new IfcDirection(Model);
                a.DirectionRatios.InternalAdd(P[0].X);
                a.DirectionRatios.InternalAdd(P[0].Y);
                a.DirectionRatios.InternalAdd(P[0].Z);

                var b = new IfcDirection(Model);
                b.DirectionRatios.InternalAdd(P[1].X);
                b.DirectionRatios.InternalAdd(P[1].Y);
                b.DirectionRatios.InternalAdd(P[1].Z);

                return new List<IfcDirection>{a, b};
            }
        }

        /// <summary>
        ///   Optional.   P[1]: The normalized direction of the placement X Axis. This is (1.0,0.0,0.0) if RefDirection is omitted. P[2]: The normalized direction of the placement Y Axis. This is a derived attribute and is orthogonal to P[1].
        /// </summary>
        public List<XbimVector3D> P
        {
            get
            {
                var p = new List<XbimVector3D>(2);
                if (RefDirection == null)
                {
                    p.Add(new XbimVector3D(1, 0, 0));
                    p.Add(new XbimVector3D(0, 1, 0));
                }
                else
                {
                    p.Add(new XbimVector3D(RefDirection.DirectionRatios[0], RefDirection.DirectionRatios[1], 0));
                    p.Add(new XbimVector3D(-RefDirection.DirectionRatios[1], RefDirection.DirectionRatios[0], 0));
                }
                return p;
            }
        }
    }
}
