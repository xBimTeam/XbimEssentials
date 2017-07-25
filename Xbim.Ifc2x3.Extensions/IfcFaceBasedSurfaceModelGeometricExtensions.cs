using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{

    public static class IfcFaceBasedSurfaceModelGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcFaceBasedSurfaceModel fbsm)
        {
            int pointCount = 0;
            foreach (IfcConnectedFaceSet cfs in fbsm.FbsmFaces)
            {
                pointCount += cfs.NumberOfPointsMax();
            }
            return pointCount;
        }


        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this  IfcFaceBasedSurfaceModel fbsm)
        {
            int hash = fbsm.FbsmFaces.Count;
            if (hash > 30) return hash ^ fbsm.GetType().Name.GetHashCode(); //probably enough for a uniquish hash
            foreach (var cfs in fbsm.FbsmFaces)
            {
                hash ^= cfs.GetGeometryHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this  IfcFaceBasedSurfaceModel a, IfcRepresentationItem b)
        {
            IfcFaceBasedSurfaceModel p = b as IfcFaceBasedSurfaceModel;
            if (p == null) return false; //different type
            List<IfcConnectedFaceSet> fsa = a.FbsmFaces.ToList();
            List<IfcConnectedFaceSet> fsb = p.FbsmFaces.ToList();
            if (fsa.Count != fsb.Count) return false;
            for (int i = 0; i < fsa.Count; i++)
            {
                if (!fsa[i].GeometricEquals(fsb[i])) return false;
            }
            return true;

        }
    }
}
