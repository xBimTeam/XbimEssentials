using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcCsgSolidGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCsgSolid solid)
        {
            if(solid.TreeRootExpression is IfcBooleanResult)
                return ((IfcBooleanResult)(solid.TreeRootExpression)).GetGeometryHashCode();
            else if (solid.TreeRootExpression is IfcCsgPrimitive3D)
            {
                throw new XbimGeometryException("Hashing of IfcCsgPrimitive3D is not implmeneted yet");
            }
            else
                throw new XbimGeometryException("Hashing of unknown Csg Solid is not implmeneted yet");
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCsgSolid a, IfcRepresentationItem b)
        {
            IfcCsgSolid csg = b as IfcCsgSolid;
            if (csg == null) return false; //different types are not the same

            if (a.TreeRootExpression is IfcBooleanResult && csg.TreeRootExpression is IfcBooleanResult)
                return ((IfcBooleanResult)(a.TreeRootExpression)).GeometricEquals((IfcBooleanResult)(csg.TreeRootExpression));
            else if (a.TreeRootExpression is IfcCsgPrimitive3D)
            {
                throw new XbimGeometryException("Equality of IfcCsgPrimitive3D is not implmeneted yet");
            }
            else
                throw new XbimGeometryException("Equality of unknown Csg Solid is not implmeneted yet");

        }
    }
}
