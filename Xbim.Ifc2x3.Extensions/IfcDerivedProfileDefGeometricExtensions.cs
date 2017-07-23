using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcDerivedProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcDerivedProfileDef profile)
        {
            return profile.ParentProfile.GetGeometryHashCode() ^ profile.Operator.GetGeometryHashCode() ;     
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcDerivedProfileDef a, IfcProfileDef b)
        {
            IfcDerivedProfileDef p = b as IfcDerivedProfileDef;
            if (p == null) return false; //different types are not the same
            return a.Operator.GeometricEquals(p.Operator) && a.ParentProfile.GeometricEquals(p.ParentProfile);
        }
    }
}
