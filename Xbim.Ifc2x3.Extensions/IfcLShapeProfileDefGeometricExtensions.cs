using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class IfcLShapeProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcLShapeProfileDef profile)
        {
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            int hash = f(profile.Depth) ^ 
                 f(profile.Thickness) ^ 
                 profile.Position.GetGeometryHashCode();
            if (profile.Width.HasValue) hash ^= f(profile.Width ?? 0);
            return hash;
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcLShapeProfileDef a, IfcProfileDef b)
        {
            IfcLShapeProfileDef p = b as IfcLShapeProfileDef;
            if (p == null) return false; //different types are not the same
            return a.Depth == p.Depth && 
                a.Thickness == p.Thickness && 
                a.Width == p.Width && 
                a.Position.GeometricEquals(p.Position);
        }
    }
}
