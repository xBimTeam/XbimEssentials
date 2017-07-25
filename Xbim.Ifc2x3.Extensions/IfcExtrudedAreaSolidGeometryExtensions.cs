using System;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcExtrudedAreaSolidGeometryExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcExtrudedAreaSolid solid)
        {
            Func<double, int> f = solid.Model.ModelFactors.GetGeometryDoubleHash;
            
            return f(solid.Depth) ^ 
                   solid.ExtrudedDirection.GetGeometryHashCode() ^
                   solid.Position.GetGeometryHashCode() ^ 
                   solid.SweptArea.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcExtrudedAreaSolid a, IfcRepresentationItem b)
        {
            IfcExtrudedAreaSolid eas = b as IfcExtrudedAreaSolid;
            if(eas == null) return false; //different types are not the same
            double precision = a.Model.ModelFactors.Precision;
            return Math.Abs(a.Depth - eas.Depth) <= precision &&
                   a.ExtrudedDirection.GeometricEquals(eas.ExtrudedDirection) &&
                   a.Position.GeometricEquals(eas.Position) &&
                   a.SweptArea.GeometricEquals(eas.SweptArea);
        }
    }
}
