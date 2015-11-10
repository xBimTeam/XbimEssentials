using System.Collections.Generic;
using Xbim.Common.Geometry;

namespace Xbim.Ifc2x3.GeometryResource
{
    // ReSharper disable once InconsistentNaming
    public partial interface IfcAxis2Placement
    {
        IfcDimensionCount Dim { get; }
        List<XbimVector3D> P { get; } 
    }

    public static class IfcAxis2PlacementExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IfcAxis2Placement placement)
        {
            var ax3 = placement as IfcAxis2Placement3D;
            var ax2 = placement as IfcAxis2Placement2D;
            if (ax3 != null)
                return ax3.ToMatrix3D();
            return ax2 != null ? ax2.ToMatrix3D() : XbimMatrix3D.Identity;
        }
    }
}
