using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcFaceGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcFace face)
        {
            return face.Bounds.Sum(bound => bound.NumberOfPointsMax());
        }

        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcFace face)
        {
            int hash = face.Bounds.Count;
            if (hash > 2) return hash; //probably unique enough
            return face.Bounds.Aggregate(hash, (current, b) => current ^ b.GetGeometryHashCode());
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcFace a, IfcFace b)
        {
            if (a.Equals(b)) return true;
            if (a.Bounds.Count != b.Bounds.Count) return false;
            List<IfcFaceBound> aFaceBounds = a.Bounds.ToList();
            List<IfcFaceBound> bFaceBounds = b.Bounds.ToList();
            for (int i = 0; i < aFaceBounds.Count; i++)
            {
                if (!(aFaceBounds[i].GeometricEquals(bFaceBounds[i])))
                    return false;
            }
            return true;
        }
    }
}
