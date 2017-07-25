using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class IfcCircleProfileDefGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCircleProfileDef profile)
        {
            var model = profile.Model;
            return model.ModelFactors.GetGeometryDoubleHash(profile.Radius) ^ profile.Position.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCircleProfileDef a, IfcProfileDef b)
        {
            IfcCircleProfileDef p = b as IfcCircleProfileDef;
            if (p == null) return false; //different types are not the same
            return a.Radius == p.Radius && a.Position.GeometricEquals(p.Position);
        }
    }
}
