using Xbim.Common.Geometry;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3.GeometryResource
{
    public class XbimLine
    {
        public IfcCartesianPoint Pnt { get; internal set; }
        public XbimVector3D Orientation { get; internal set; }
    }
}
