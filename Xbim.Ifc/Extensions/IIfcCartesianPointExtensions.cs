using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcCartesianPointExtensions
    {
        public static void SetXY(this IIfcCartesianPoint obj, double x, double y)
        {
            obj.Coordinates.Clear();
            obj.Coordinates.Add(x);
            obj.Coordinates.Add(y);
        }

        public static void SetXYZ(this IIfcCartesianPoint obj, double x, double y, double z)
        {
            obj.Coordinates.Clear();
            obj.Coordinates.Add(x);
            obj.Coordinates.Add(y);
            obj.Coordinates.Add(z);
        }

        public static XbimPoint3D ToXbimPoint3D(this IIfcCartesianPoint obj)
        {
            return new XbimPoint3D(obj.X, obj.Y, obj.Z);
        }

        public static bool IsEqual(this IIfcCartesianPoint obj, IIfcCartesianPoint p, double tolerance)
        {
            return obj.DistanceSquared(p) <= (tolerance * tolerance);
        }

        public static double DistanceSquared(this IIfcCartesianPoint obj, IIfcCartesianPoint p)
        {
            double d = 0, dd;
            double x1, y1, z1, x2, y2, z2;
            obj.XYZ(out x1, out y1, out z1);
            p.XYZ(out x2, out y2, out z2);
            dd = x1; dd -= x2; dd *= dd; d += dd;
            dd = y1; dd -= y2; dd *= dd; d += dd;
            dd = z1; dd -= z2; dd *= dd; d += dd;
            return d;
        }

        public static void XYZ(this IIfcCartesianPoint obj, out double x, out double y, out double z)
        {
            if (obj.Dim == 3)
            {

                x = obj.Coordinates[0]; y = obj.Coordinates[1]; z = obj.Coordinates[2];
            }
            else if (obj.Dim == 2)
            {

                x = obj.Coordinates[0]; y = obj.Coordinates[1]; z = 0;
            }
            else
            {
                z = y = x = double.NaN;
            }
        }
    }
}
