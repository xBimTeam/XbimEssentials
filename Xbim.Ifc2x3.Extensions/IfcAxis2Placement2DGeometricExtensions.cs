using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcAxis2Placement2DGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcAxis2Placement2D ax2)
        {
            return ax2.Location.GetGeometryHashCode() ^ ax2.RefDirection.GetGeometryHashCode();
        }

         /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcAxis2Placement2D a, IfcAxis2Placement2D b)
        {
            if (a.Equals(b)) return true;
            double precision = b.Model.ModelFactors.Precision;
            return a.P[0].IsEqual(b.P[0],precision) && 
                a.P[1].IsEqual(b.P[1],precision) && 
                a.Location.GeometricEquals(b.Location);
        }
    }
}
