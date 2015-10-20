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
            za.Normalize();
            var xa = RefDirection.XbimVector3D();
            xa.Normalize();
            var ya = XbimVector3D.CrossProduct(za, xa);
            ya.Normalize();
            return new XbimMatrix3D(xa.X, xa.Y, xa.Z, 0, ya.X, ya.Y, ya.Z, 0, za.X, za.Y, za.Z, 0, Location.X,
                Location.Y, Location.Z, 1);
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

                var c = new IfcDirection(Model);
                c.DirectionRatios.InternalAdd(P[2].X);
                c.DirectionRatios.InternalAdd(P[2].Y);
                c.DirectionRatios.InternalAdd(P[2].Z);

                return new List<IfcDirection> { a, b, c };
            }
        }

        /// <summary>
        ///   Derived. The normalized directions of the placement X Axis (P[0]) and the placement Y Axis (P[1]) and the placement Z Axis (P[2]).
        /// </summary>
        public List<XbimVector3D> P
        {
            get
            {
                var p = new List<XbimVector3D>(3);
                if (RefDirection == null && Axis == null)
                {
                    p.Add(new XbimVector3D(1, 0, 0));
                    p.Add(new XbimVector3D(0, 1, 0));
                    p.Add(new XbimVector3D(0, 0, 1));
                }
                else if (RefDirection != null && Axis != null)
                {
                    var za = _axis.XbimVector3D();
                    za.Normalize();
                    var xa = _refDirection.XbimVector3D();
                    xa.Normalize();
                    var ya = XbimVector3D.CrossProduct(za, xa);
                    ya.Normalize();
                    p.Add(xa);
                    p.Add(ya);
                    p.Add(za);
                }
                else
                    throw new ArgumentException("RefDirection and Axis must be noth either null or both defined");
                return p;
            }
        }
    }
}
