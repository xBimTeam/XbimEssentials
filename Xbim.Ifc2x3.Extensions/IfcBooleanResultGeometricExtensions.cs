using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcBooleanResultGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this  IfcBooleanResult bResult)
        {
            return (bResult.FirstOperand.EntityLabel ^ bResult.SecondOperand.EntityLabel).GetHashCode(); //good enough for most
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this  IfcBooleanResult a, IfcRepresentationItem b)
        {
            IfcBooleanResult p = b as IfcBooleanResult;
            if (p == null) return false; //different types are not the same
            if(a.Equals(p)) return true;
            return a.FirstOperand.EntityLabel == p.FirstOperand.EntityLabel &&
                 a.SecondOperand.EntityLabel == p.SecondOperand.EntityLabel &&
                 a.Operator == p.Operator;

        }
    }
}
