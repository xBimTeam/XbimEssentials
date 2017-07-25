using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcManifoldSolidBrepGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcManifoldSolidBrep solid)
        {
            return solid.Outer.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcManifoldSolidBrep a, IfcRepresentationItem b)
        {
            IfcFacetedBrep fb = b as IfcFacetedBrep;
            if (fb == null) return false; //different types are not the same
            if(a.Equals(fb)) return true;
            return a.Outer.GeometricEquals(fb.Outer);
        }
    }
}
