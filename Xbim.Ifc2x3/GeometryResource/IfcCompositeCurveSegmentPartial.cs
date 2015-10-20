namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcCompositeCurveSegment
    {
        /// <summary>
        ///   Derived. The space dimensionality of this class, defined by the dimensionality of the first ParentCurve.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return ParentCurve.Dim; }
        }
    }
}
