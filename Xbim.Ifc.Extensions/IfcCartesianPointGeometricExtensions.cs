using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcCartesianPointGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCartesianPoint pt)
        {
            Func<double, int> f = pt.ModelOf.ModelFactors.GetGeometryDoubleHash;
            switch (pt.Dim)
            {
                case 1:
                    return f(pt.X);
                case 2:
                    return f(pt.X) ^ f(pt.Y);
                case 3:
                    return f(pt.X) ^ f(pt.Y) ^ f(pt.Z);
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCartesianPoint a, IfcCartesianPoint b)
        {
            if (a.Equals(b)) return true;
            double precision = a.ModelOf.ModelFactors.Precision;
            return a.IsEqual(b, precision);
        }
    }
}
