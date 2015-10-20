namespace Xbim.Ifc2x3.GeometryResource
{
    public abstract partial class IfcCartesianTransformationOperator
    {
        /// <summary>
        ///   Derived. The derived scale S of the transformation, equal to scale if that exists, or 1.0 otherwise.
        /// </summary>
        public double Scl
        {
            get { return Scale ?? 1.0; }
        }
    }
}
