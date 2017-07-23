using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcConnectedFaceSetGeometricExtensions
    {

        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcConnectedFaceSet cfs)
        {
            int pointCount = 0;
            foreach (IfcFace face in cfs.CfsFaces)
            {
                pointCount += face.NumberOfPointsMax();
            }
            return pointCount;
        }


        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcConnectedFaceSet cfs)
        {
            int hash = cfs.CfsFaces.Count;
            if (hash > 30) return hash ^ cfs.GetType().Name.GetHashCode(); //probably enough for a uniquish hash
            foreach (var face in cfs.CfsFaces)
                hash ^= face.GetGeometryHashCode();
            return hash;
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcConnectedFaceSet a, IfcRepresentationItem b)
        {         

            if (a.Equals(b)) return true;
            IfcConnectedFaceSet p = b as IfcConnectedFaceSet;
            if (p == null) return false;
            if(a.CfsFaces.Count!=p.CfsFaces.Count) return false;
            List<IfcFace> aFaces = a.CfsFaces.ToList();
            List<IfcFace> bFaces = p.CfsFaces.ToList();
            for (int i = 0; i < aFaces.Count; i++)
			{
			 if(!(aFaces[i].GeometricEquals(bFaces[i])))
                 return false;
			}
            return true;
        }
    }
}
