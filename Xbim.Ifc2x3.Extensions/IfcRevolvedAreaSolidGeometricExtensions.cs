using System;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    
        public static class IfcRevolvedAreaSolidGeometricExtensions
        {
            /// <summary>
            /// returns a Hash for the geometric behaviour of this object
            /// </summary>
            /// <param name="solid"></param>
            /// <returns></returns>
            public static int GetGeometryHashCode(this IfcRevolvedAreaSolid solid)
            {
                Func<double, int> f = solid.Model.ModelFactors.GetGeometryDoubleHash;
                return solid.Axis.GetGeometryHashCode() ^
                       f(solid.Angle) ^
                       solid.Position.GetGeometryHashCode() ^
                       solid.SweptArea.GetGeometryHashCode();
            }

            /// <summary>
            /// Compares two objects for geomtric equality
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b">object to compare with</param>
            /// <returns></returns>
            public static bool GeometricEquals(this IfcRevolvedAreaSolid a, IfcRepresentationItem b)
            {
                IfcRevolvedAreaSolid eas = b as IfcRevolvedAreaSolid;
                if (eas == null) return false; //different types are not the same
                double precision = a.Model.ModelFactors.Precision;
                return Math.Abs(a.Angle - eas.Angle) <= precision &&
                       a.Axis.GeometricEquals(eas.Axis) &&
                       a.Position.GeometricEquals(eas.Position) &&
                       a.SweptArea.GeometricEquals(eas.SweptArea);
            }
        }
    }

