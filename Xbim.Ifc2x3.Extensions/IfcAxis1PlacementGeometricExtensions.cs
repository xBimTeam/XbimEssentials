using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcAxis1PlacementGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcAxis1Placement ax1)
        {
            return ax1.Location.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcAxis1Placement a, IfcAxis1Placement b)
        {
            if (a.Equals(b)) return true;
            return 
                a.Location.GeometricEquals(b.Location);
        }
    }
}
