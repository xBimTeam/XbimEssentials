using System;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcSurfaceCurveSweptAreaSolidGeometricExtension
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcSurfaceCurveSweptAreaSolid solid)
        {
            Func<double, int> f = solid.Model.ModelFactors.GetGeometryDoubleHash;
            return solid.Directrix.GetGeometryHashCode() ^
                   solid.ReferenceSurface.GetGeometryHashCode() ^
                   solid.Position.GetGeometryHashCode() ^
                   solid.SweptArea.GetGeometryHashCode();
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcSurfaceCurveSweptAreaSolid a, IfcRepresentationItem b)
        {
            IfcSurfaceCurveSweptAreaSolid scsa = b as IfcSurfaceCurveSweptAreaSolid;
            if (scsa == null) return false; //different types are not the same
            double precision = a.Model.ModelFactors.Precision;
            return a.Directrix.GeometricEquals(scsa.Directrix) &&
                   a.StartParam == scsa.EndParam &&
                   a.EndParam == scsa.EndParam &&
                   a.ReferenceSurface.GeometricEquals(scsa.ReferenceSurface) &&
                   a.Position.GeometricEquals(scsa.Position) &&
                   a.SweptArea.GeometricEquals(scsa.SweptArea);
        }
    }
}
