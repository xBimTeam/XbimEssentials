using System;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Extensions
{
    /// <summary>
    /// Neds support for Grid Placements
    /// </summary>
    public static class IfcPlacementExtensions
    {
        public static double? Z(this IIfcObjectPlacement objPlacement)
        {
            var localPlacement = objPlacement as IIfcLocalPlacement;
            if (localPlacement == null) return null;
            var matrix = localPlacement.ToMatrix3D();
            var origin = new XbimPoint3D(0,0,0);
            origin = matrix.Transform(origin);
            return origin.Z;
        }

        public static XbimMatrix3D ToMatrix3D(this IIfcObjectPlacement objPlacement)
        {
            var lp = objPlacement as IIfcLocalPlacement;
            if (lp != null)
            {
                XbimMatrix3D local = lp.RelativePlacement.ToMatrix3D();
                if (lp.PlacementRelTo != null)
                    return local * lp.PlacementRelTo.ToMatrix3D();
                return local;
            }          
            else //
                throw new NotImplementedException($"Placement of type {objPlacement.GetType().Name} is not implemented");
        }

        /// <summary>
        /// Converts a placement to a Matrix3D
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement placement)
        {
            var ax3 = placement as IIfcAxis2Placement3D;
            var ax2 = placement as IIfcAxis2Placement2D;
            return ax3?.ToMatrix3D() ?? ax2?.ToMatrix3D() ?? XbimMatrix3D.Identity;
        }

        /// <summary>
        /// Converts a placement to a Matrix3D
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IIfcPlacement placement)
        {
            var ax3 = placement as IIfcAxis2Placement3D;
            var ax2 = placement as IIfcAxis2Placement2D;            
            return ax3?.ToMatrix3D()?? ax2?.ToMatrix3D() ?? XbimMatrix3D.Identity;
        }

        /// <summary>
        ///   Converts an Axis2Placement3D to a XbimMatrix3D
        /// </summary>
        /// <param name = "axis3"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement3D axis3)
        {
            if (axis3.RefDirection != null && axis3.Axis != null)
            {
                var za = new XbimVector3D(axis3.Axis.X,axis3.Axis.Y,axis3.Axis.Z);
                za = za.Normalized();
                var xa = new XbimVector3D(axis3.RefDirection.X, axis3.RefDirection.Y, axis3.RefDirection.Z);
                xa = xa.Normalized();
                var ya = XbimVector3D.CrossProduct(za, xa);
                ya = ya.Normalized();
                return new XbimMatrix3D(xa.X, xa.Y, xa.Z, 0, ya.X, ya.Y, ya.Z, 0, za.X, za.Y, za.Z, 0, axis3.Location.X,
                                    axis3.Location.Y, axis3.Location.Z, 1);
            }
            return new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, axis3.Location.X, axis3.Location.Y,
                axis3.Location.Z, 1);
        }

        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement2D axis2)
        {
            if (axis2.RefDirection != null)
            {
                var v = new XbimVector3D(axis2.RefDirection.X, axis2.RefDirection.Y, 0);
                v = v.Normalized();
                return new XbimMatrix3D(v.X, v.Y, 0, 0, v.Y, v.X, 0, 0, 0, 0, 1, 0, axis2.Location.X, axis2.Location.Y, 0, 1);
            }
            return new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, axis2.Location.X, axis2.Location.Y,
                axis2.Location.Z, 1);
        }
    }
}
