using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcCurveGeometricExtensions
    {
        /// <summary>
        /// returns a Hash for the geometric behaviour of this object
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcCurve curve)
        {
            if (curve is IfcPolyline )
                return ((IfcPolyline)curve).GetGeometryHashCode();
            return curve.GetHashCode(); //this will mostly give a hash, the geometric  hash functions for curves needs to be implemented, for now this avoids problems
        }

        /// <summary>
        /// Compares two objects for geometric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcCurve a, IfcCurve b)
        {
            if(a is IfcPolyline && b is IfcPolyline)
                return ((IfcPolyline)a).GeometricEquals((IfcPolyline)b);
            return a.Equals(b); //comparing two objects from different models hould always fails, this needs a proper implementation, but for now this ensure correctness but does find true duplicates
        }

    }
}
