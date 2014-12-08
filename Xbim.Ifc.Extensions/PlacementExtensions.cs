using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.Extensions
{
    public static class PlacementExtensions
    {
        /// <summary>
        /// Converts a placement to a Matrix3D
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IfcPlacement placement)
        {
            IfcAxis2Placement3D ax3 = placement as IfcAxis2Placement3D;
            IfcAxis2Placement2D ax2 = placement as IfcAxis2Placement2D;
            if (ax3 != null) 
                return ax3.ToMatrix3D();
            else if (ax2 != null) 
                return ax2.ToMatrix3D();
            else
                return XbimMatrix3D.Identity;
        }

        public static XbimMatrix3D ToMatrix3D(this IfcAxis2Placement placement)
        {
            IfcAxis2Placement3D ax3 = placement as IfcAxis2Placement3D;
            IfcAxis2Placement2D ax2 = placement as IfcAxis2Placement2D;
            if (ax3 != null)
                return ax3.ToMatrix3D();
            else if (ax2 != null)
                return ax2.ToMatrix3D();
            else
                return XbimMatrix3D.Identity;
        }
    }
}
