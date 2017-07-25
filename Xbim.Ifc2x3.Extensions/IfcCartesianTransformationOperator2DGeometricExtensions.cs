using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcCartesianTransformationOperator2DGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCartesianTransformationOperator2D transform)
        {

            int hash = transform.LocalOrigin.GetGeometryHashCode();
            if (transform.Axis1 != null) hash ^= transform.Axis1.GetGeometryHashCode();
            if (transform.Axis2 != null) hash ^= transform.Axis2.GetGeometryHashCode();
            if (transform.Scale.HasValue) hash ^= transform.Scale.Value.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCartesianTransformationOperator2D a, IfcCartesianTransformationOperator2D b)
        {
            bool originEqual = a.LocalOrigin.GeometricEquals(b.LocalOrigin);
            bool axis1Equal = (a.Axis1 != null && b.Axis1 != null && a.Axis1.GeometricEquals(b.Axis1)) || (a.Axis1 == null && b.Axis1 == null);
            bool axis2Equal = (a.Axis2 != null && b.Axis2 != null && a.Axis2.GeometricEquals(b.Axis2)) || (a.Axis2 == null && b.Axis2 == null);
            bool scaleEqual =  a.Scale.Value == b.Scale.Value;
            return originEqual && axis1Equal && axis2Equal && scaleEqual;

        }
    }
}
