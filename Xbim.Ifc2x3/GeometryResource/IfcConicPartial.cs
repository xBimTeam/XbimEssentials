namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcConic
    {
        public override IfcDimensionCount Dim
        {
            get
            {
                if (Position is IfcAxis2Placement2D) return 2;
                if (Position is IfcAxis2Placement3D) return 3;
                return 0;
            }
        }
    }
}
