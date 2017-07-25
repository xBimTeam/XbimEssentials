using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    
    public static class IfcShellGeometricextensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcShell shell)
        {
            return ((IfcConnectedFaceSet)shell).NumberOfPointsMax();
            
        }


        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this  IfcShell shell)
        {
            if (shell is IfcOpenShell) return ((IfcOpenShell)shell).GetGeometryHashCode();
            if (shell is IfcClosedShell) return ((IfcClosedShell)shell).GetGeometryHashCode();
            else return shell.GetHashCode(); //use object hash for a uniqueish result
        }

        public static int GetGeometryHashCode(this  IfcOpenShell shell)
        {
            int hash = shell.CfsFaces.Count;
            if (hash > 30) return hash ^ shell.GetType().Name.GetHashCode(); //probably enough for a uniquish hash
            foreach (IfcFace face in shell.CfsFaces)
            {
                hash ^= face.GetGeometryHashCode();
            }
            return hash;
        }

        public static int GetGeometryHashCode(this  IfcClosedShell shell)
        {
            int hash = shell.CfsFaces.Count;
            if (hash > 30) return hash ^ shell.GetType().Name.GetHashCode(); //probably enough for a uniquish hash
            foreach (IfcFace face in shell.CfsFaces)
            {
                hash ^= face.GetGeometryHashCode();
            }
            return hash;
        }
        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this  IfcShell a, IfcShell b)
        {
            IfcOpenShell ob = b as IfcOpenShell;
            IfcOpenShell oa = a as IfcOpenShell;
            if (ob != null && oa != null) return oa.GeometricEquals(ob);

            IfcClosedShell cb = b as IfcClosedShell;
            IfcClosedShell ca = a as IfcClosedShell;
            if (cb != null && ca != null) return ca.GeometricEquals(cb);
            return false;

        }
        public static bool GeometricEquals(this  IfcOpenShell a, IfcOpenShell b)
        {
            if (a == null && b == null) return false; //null
            if (a == null || b == null) return false; //different type
            List<IfcFace> fsb = b.CfsFaces.ToList(); 
            List<IfcFace> fsa = a.CfsFaces.ToList(); 
            if (fsa.Count != fsb.Count) return false;
            for (int i = 0; i < fsa.Count; i++)
            {
                if (!fsa[i].GeometricEquals(fsb[i])) return false;
            }
            return true;
            
        }
        public static bool GeometricEquals(this  IfcClosedShell a, IfcClosedShell b)
        {
            if (a == null && b == null) return false; //null
            if (a == null || b == null) return false; //different type
            List<IfcFace> fsb = b.CfsFaces.ToList();
            List<IfcFace> fsa = a.CfsFaces.ToList();
            if (fsa.Count != fsb.Count) return false;
            for (int i = 0; i < fsa.Count; i++)
            {
                if (!fsa[i].GeometricEquals(fsb[i])) return false;
            }
            return true;

        }
    }
}
