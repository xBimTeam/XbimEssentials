using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class IfcUShapeProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcUShapeProfileDef profile)
        {
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            int hash = f(profile.WebThickness) ^
                 f(profile.FlangeThickness) ^
                 f(profile.FlangeWidth) ^
                 f(profile.Depth) ^
                 profile.Position.GetGeometryHashCode();
            if (profile.FilletRadius.HasValue) hash ^= f(profile.FilletRadius ?? 0);
            if (profile.EdgeRadius.HasValue) hash ^= f(profile.EdgeRadius ?? 0);
            if (profile.FlangeSlope.HasValue) hash ^= f(profile.FlangeSlope ?? 0);
            if (profile.CentreOfGravityInX.HasValue) hash ^= f(profile.CentreOfGravityInX ?? 0);
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcUShapeProfileDef a, IfcProfileDef b)
        {
            IfcUShapeProfileDef p = b as Xbim.Ifc2x3.ProfileResource.IfcUShapeProfileDef;
            if (p == null) return false; //different types are not the same
            return a.WebThickness == p.WebThickness &&
                a.FlangeThickness == p.FlangeThickness &&
                a.FlangeWidth == p.FlangeWidth &&
                a.Depth == p.Depth &&
                a.FilletRadius == p.FilletRadius &&
                a.EdgeRadius == p.EdgeRadius &&
                a.FlangeSlope == p.FlangeSlope &&
                a.CentreOfGravityInX == p.CentreOfGravityInX &&
                a.Position.GeometricEquals(p.Position);
        }
    }
}
