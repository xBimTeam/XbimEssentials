using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class IfcIShapeProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcIShapeProfileDef profile)
        {
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            int hash = f(profile.WebThickness) ^
                 f(profile.FlangeThickness) ^
                 f(profile.OverallDepth) ^
                 f(profile.OverallWidth) ^
                 profile.Position.GetGeometryHashCode();
            if (profile.FilletRadius.HasValue) hash ^= f(profile.FilletRadius ?? 0);
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcIShapeProfileDef a, IfcProfileDef b)
        {
            IfcIShapeProfileDef p = b as IfcIShapeProfileDef;
            if (p == null) return false; //different types are not the same
            return a.WebThickness == p.WebThickness &&
                a.FlangeThickness == p.FlangeThickness &&
                a.OverallDepth == p.OverallDepth &&
                a.OverallWidth == p.OverallWidth &&
                a.FilletRadius == p.FilletRadius &&
                a.Position.GeometricEquals(p.Position);
        }
    }
}
