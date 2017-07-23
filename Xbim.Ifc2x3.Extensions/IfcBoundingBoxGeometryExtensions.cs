using System;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcBoundingBoxGeometryExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcBoundingBox solid)
        {
            Func<double, int> f = solid.Model.ModelFactors.GetGeometryDoubleHash;

            return solid.Corner.GetGeometryHashCode() ^
                   f(solid.XDim) ^
                   f(solid.YDim) ^
                   f(solid.ZDim);
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcBoundingBox a, IfcRepresentationItem b)
        {
            var bb = b as IfcBoundingBox;
            if (bb == null) return false; //different types are not the same
            double precision = a.Model.ModelFactors.Precision;
            return Math.Abs(a.XDim - bb.XDim) <= precision &&
                Math.Abs(a.YDim - bb.YDim) <= precision &&
                Math.Abs(a.ZDim - bb.ZDim) <= precision &&
                   a.Corner.GeometricEquals(bb.Corner);
        }
    }
}
