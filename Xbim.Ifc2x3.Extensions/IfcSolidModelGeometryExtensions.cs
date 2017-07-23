using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcSolidModelGeometryExtensions
    {
      
        #region Geometry Hashing

        /// <summary>
        /// Returns a Hash Code for the geometric properties of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcSolidModel solid)
        {
            if (solid is IfcExtrudedAreaSolid)
                return ((IfcExtrudedAreaSolid)solid).GetGeometryHashCode();
            else if( solid is IfcFacetedBrep)
                return ((IfcFacetedBrep)solid).GetGeometryHashCode();
            else if (solid is IfcRevolvedAreaSolid)
                return ((IfcRevolvedAreaSolid)solid).GetGeometryHashCode();
            else if (solid is IfcCsgSolid)
                return ((IfcCsgSolid)solid).GetGeometryHashCode();
            else if (solid is IfcSurfaceCurveSweptAreaSolid)
                return ((IfcSurfaceCurveSweptAreaSolid)solid).GetGeometryHashCode();
            else
            {
                return 0;
                throw new XbimGeometryException("Unsupported solid geometry tpype " + solid.GetType().Name);
            }
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcSolidModel a, IfcRepresentationItem b)
        {
            if (a is IfcExtrudedAreaSolid)
                return ((IfcExtrudedAreaSolid)a).GeometricEquals(b);
            else if (a is IfcFacetedBrep)
                return ((IfcFacetedBrep)a).GeometricEquals(b);
            else if (a is IfcRevolvedAreaSolid)
                return ((IfcRevolvedAreaSolid)a).GeometricEquals(b);
            else if (a is IfcCsgSolid)
                return ((IfcCsgSolid)a).GeometricEquals(b);
            else if (a is IfcSurfaceCurveSweptAreaSolid)
                return ((IfcSurfaceCurveSweptAreaSolid)a).GeometricEquals(b);
            else
            {
                return false;
                throw new XbimGeometryException("Unsupported solid geometry tpype " + a.GetType().Name);
            }
        }
        #endregion

    }



}
