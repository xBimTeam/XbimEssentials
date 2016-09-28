using System.Collections.Generic;
using Xbim.Common.Geometry;

namespace Xbim.Ifc4.GeometryResource
{
    // ReSharper disable once InconsistentNaming
    public partial interface IfcAxis2Placement
    {
        IfcDimensionCount Dim { get; }
        List<XbimVector3D> P { get; }
    }

}
