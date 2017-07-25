using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.RepresentationResource;


namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcRepresentationGeometryExtensions
    {
      

        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcRepresentation rep)
        {
            int hash = 0;
            foreach (var item in rep.Items)
            {
                hash ^= item.GetGeometryHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcRepresentation a, IfcRepresentation b)
        {           
            if (a.Equals(b)) return true;
            List<IfcRepresentationItem> aRep = a.Items.ToList();
            List<IfcRepresentationItem> bRep = b.Items.ToList();
            for (int i = 0; i < aRep.Count; i++)
            {
                if(!aRep[i].GeometricEquals(bRep[i])) return false;
            }
            return true;
        }
    }
}
