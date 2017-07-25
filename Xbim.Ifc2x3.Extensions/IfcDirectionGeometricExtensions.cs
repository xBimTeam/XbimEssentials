using System;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcDirectionGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcDirection dir)
        {
            if (dir == null) return 0;

            var model = dir.Model;
            Func<double, int> f = model.ModelFactors.GetGeometryDoubleHash;
            switch (dir.Dim)
            {
                case 1:
                    return f(dir.X);
                case 2:
                    return f(dir.X) ^ f(dir.Y);
                case 3:
                    return f(dir.X) ^ f(dir.Y) ^ f(dir.Z);
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
        public static bool GeometricEquals(this IfcDirection a, IfcDirection b)
        {
            if (a.Equals(b)) return true;
            XbimVector3D va = a.XbimVector3D();
            XbimVector3D vb = b.XbimVector3D();
            return va.IsEqual(vb,b.Model.ModelFactors.Precision);
        }
    }
}
