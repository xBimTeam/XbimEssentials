using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcCircleHollowProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCircleHollowProfileDef profile)
        {
            if (profile == null) return 0;
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            return f(profile.Radius) ^ f(profile.WallThickness) ^ profile.Position.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCircleHollowProfileDef a, IfcProfileDef b)
        {
            IfcCircleHollowProfileDef p = b as IfcCircleHollowProfileDef;
            if (p == null) return false; //different types are not the same
            return a.Radius == p.Radius && a.WallThickness == p.WallThickness && a.Position.GeometricEquals(p.Position);
        }
    }
}
