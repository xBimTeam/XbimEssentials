using System.Linq;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcPolyLoopGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcPolyLoop pLoop)
        {
            return pLoop.Polygon.Count;
        }

        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcPolyLoop pLoop)
        {
            int hash = pLoop.Polygon.Count;
            if (hash > 10 || hash < 3) return hash; //probably good enough
            return hash ^ pLoop.Polygon.First().GetGeometryHashCode() ^ pLoop.Polygon.Last().GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcPolyLoop a, IfcPolyLoop b)
        {
            if (a.Equals(b)) return true;
            if (a.Polygon.Count != b.Polygon.Count) return false;
            for (int i = 0; i < a.Polygon.Count; i++)
                if (!a.Polygon[i].GeometricEquals(b.Polygon[i])) return false;
            return true;
        }
    }
}
