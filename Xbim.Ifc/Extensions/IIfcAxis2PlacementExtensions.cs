using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcAxis2PlacementExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IIfcAxis2Placement placement)
        {
            return placement switch
            {
                IIfcAxis2Placement3D ax3 => ax3.ToMatrix3D(),
                IIfcAxis2Placement2D ax2 => ax2.ToMatrix3D(),
                _ => XbimMatrix3D.Identity
            };
        }

    }
}
