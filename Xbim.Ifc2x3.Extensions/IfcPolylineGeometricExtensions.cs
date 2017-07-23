using System.Linq;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcPolylineGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcPolyline pLine)
        {
            int hash = pLine.Points.Count;
            if (hash > 20 || hash < 3) return hash; //probably good enough
            int midIdx = pLine.Points.Count/2;
            return hash ^ pLine.Points.First().GetGeometryHashCode() ^ pLine.Points[midIdx].GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcPolyline a, IfcPolyline b)
        {
            if (a.Equals(b)) return true;
            if (a.Points.Count != b.Points.Count) return false;
            for (int i = 0; i < a.Points.Count; i++)
                if (!a.Points[i].GeometricEquals(b.Points[i])) return false; //dioes not deal with plines that have the same points but with a different start point
            return true;
        }
    }
}
