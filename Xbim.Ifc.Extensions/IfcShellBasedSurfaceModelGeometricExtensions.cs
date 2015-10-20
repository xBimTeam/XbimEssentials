using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.TopologyResource;
using XbimGeometry.Interfaces;

namespace Xbim.Ifc2x3.Extensions
{

    public static class IfcShellBasedSurfaceModelGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcShellBasedSurfaceModel sbsm)
        {
            var pointCount = 0;
            foreach (var shell in sbsm.SbsmBoundary)
            {
                pointCount += shell.NumberOfPointsMax();
            }
            return pointCount;
        }

        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <returns></returns>
        public static int GetGeometryHashCode(this  IfcShellBasedSurfaceModel sbsm)
        {
            var hash = sbsm.SbsmBoundary.Count;
            if (hash > 30) return hash ^ sbsm.GetType().Name.GetHashCode(); //probably enough for a uniquish hash
            foreach (IXbimShell cfs in sbsm.SbsmBoundary)
            {
                foreach (IfcFace face in cfs.Faces)
                {
                    hash ^= face.GetGeometryHashCode();
                }
                
            }
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this  IfcShellBasedSurfaceModel a, IfcRepresentationItem b)
        {
            var p = b as IfcShellBasedSurfaceModel;
            if (p == null) return false; //different type
            var fsa = a.SbsmBoundary.ToList();
            var fsb = p.SbsmBoundary.ToList();
            if (fsa.Count != fsb.Count) return false;
            for (var i = 0; i < fsa.Count; i++)
            {
                if (!fsa[i].GeometricEquals(fsb[i])) return false;
            }
            return true;

        }

    }
}
