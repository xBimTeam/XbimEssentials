using System;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class IfcRectangularHollowProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcRectangleHollowProfileDef profile)
        {
            Func<double, int> f = profile.Model.ModelFactors.GetGeometryDoubleHash;

            if (profile.YDim != 0) //dividing x/y makes profile hash unique
                return f(profile.XDim) ^
                    f(profile.XDim / profile.YDim) ^
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
        public static bool GeometricEquals(this IfcRectangleHollowProfileDef a, IfcProfileDef b)
        {
            IfcRectangleHollowProfileDef p = b as IfcRectangleHollowProfileDef;
            if (p == null) return false; //different types are not the same
            return a.XDim == p.XDim && a.YDim == p.YDim &&
                a.WallThickness == p.WallThickness &&
                a.InnerFilletRadius == p.InnerFilletRadius &&
                a.OuterFilletRadius == p.OuterFilletRadius && 
                a.Position.GeometricEquals(p.Position);
        }
    }
}
