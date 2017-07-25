using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcFaceBoundGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this  IfcFaceBound faceBound)
        {
            return faceBound.Bound.NumberOfPointsMax();
        }

        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcFaceBound faceBound)
        {
            return faceBound.Bound.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcFaceBound a, IfcFaceBound b)
        {
            if (a.Equals(b)) return true;
            return a.Bound.GeometricEquals(b.Bound);
        }
    }
}
