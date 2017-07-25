using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcAxis2Placement3DGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcAxis2Placement3D ax2)
        {
            return ax2.Location.GetGeometryHashCode() ^ ax2.Axis.GetGeometryHashCode() ^ ax2.RefDirection.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcAxis2Placement3D a, IfcAxis2Placement3D b)
        {
            if (a.Equals(b)) return true;
            double precision = a.Model.ModelFactors.Precision;
            return a.P[0].IsEqual(b.P[0], precision) && a.P[1].IsEqual(b.P[1], precision) && a.Location.GeometricEquals(b.Location);
        }
    }
}
