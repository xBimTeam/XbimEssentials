using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcArbitraryClosedProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcArbitraryClosedProfileDef profile)
        {
            return profile.OuterCurve.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcArbitraryClosedProfileDef a, IfcProfileDef b)
        {
            IfcArbitraryClosedProfileDef p = b as IfcArbitraryClosedProfileDef;
            if (p == null) return false; //different types are not the same
            return a.ProfileType==b.ProfileType && a.OuterCurve.GeometricEquals(p.OuterCurve);
        }
    }
}
