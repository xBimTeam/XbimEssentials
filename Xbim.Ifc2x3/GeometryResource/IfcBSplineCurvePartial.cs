
namespace Xbim.Ifc2x3.GeometryResource
{
    public abstract partial class IfcBSplineCurve
    {
        public override IfcDimensionCount Dim
        {
            get
            {
                var first = ControlPointsList.FirstOrDefault();
                return first != null ? first.Dim : 0;
            }
        }
    }
}
