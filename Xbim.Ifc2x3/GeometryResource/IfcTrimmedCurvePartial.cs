using System;
namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcTrimmedCurve
    {
        public override IfcDimensionCount Dim
        {
            get { return BasisCurve.Dim; }
        }
    }
}
