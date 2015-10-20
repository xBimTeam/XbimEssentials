namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcCompositeCurve
    {
        public override IfcDimensionCount Dim
        {
            get
            {
                var first = Segments.FirstOrDefault();
                return first != null ? first.Dim : 0;
            }
        }
    }
}
