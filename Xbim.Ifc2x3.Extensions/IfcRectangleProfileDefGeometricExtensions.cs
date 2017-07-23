using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcRectangleProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcRectangleProfileDef profile)
        {
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            if (profile.YDim != 0) //dividing x/y makes profile hash unique
                return f(profile.XDim) ^ 
                    f(profile.XDim/profile.YDim) ^ 
                    profile.Position.GetGeometryHashCode();
            else
                return f(profile.XDim) ^
                        profile.Position.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcRectangleProfileDef a, IfcProfileDef b)
        {
            IfcRectangleProfileDef p = b as IfcRectangleProfileDef;
            if (p == null) return false; //different types are not the same
            return a.XDim == p.XDim && a.YDim == p.YDim && a.Position.GeometricEquals(p.Position);
        }
    }
}
