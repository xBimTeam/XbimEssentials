namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcCartesianTransformationOperator3DnonUniform
    {
        public double Scl2
        {
            get { return Scale2 ?? Scl; }
        }

        public double Scl3
        {
            get { return Scale3 ?? Scl; }
        }
    }
}
