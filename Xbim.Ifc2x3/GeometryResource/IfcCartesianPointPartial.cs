using Xbim.Common.Geometry;

namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcCartesianPoint
    {
        public double X
        {
            get
            {
                return Coordinates.Count == 0 ? double.NaN : (double)Coordinates[0];
            }
            set
            {
                if (Coordinates.Count == 0)
                    Coordinates.Add(value);
                else
                    Coordinates[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return Coordinates.Count < 2 ? double.NaN : (double)Coordinates[1];
            }
            set
            {
                if (Coordinates.Count < 2)
                {
                    if (Coordinates.Count == 0) Coordinates.Add(double.NaN);
                    Coordinates.Add(value);
                }
                else
                    Coordinates[1] = value;
            }
        }

        public double Z
        {
            get
            {
                return Coordinates.Count < 3 ? double.NaN : (double)Coordinates[2];
            }
            set
            {
                if (Coordinates.Count < 3)
                {
                    if (Coordinates.Count == 0) Coordinates.Add(double.NaN);
                    if (Coordinates.Count == 1) Coordinates.Add(double.NaN);
                    Coordinates.Add(value);
                }
                else
                    Coordinates[2] = value;
            }
        }

        public void SetXY(double x, double y)
        {
            Coordinates.Clear();
            Coordinates.Add(x);
            Coordinates.Add(y);
        }

        public void SetXYZ(double x, double y, double z)
        {
            Coordinates.Clear();
            Coordinates.Add(x);
            Coordinates.Add(y);
            Coordinates.Add(z);
        }

        public XbimPoint3D XbimPoint3D()
        {
            return new XbimPoint3D(X, Y, Z);
        }

        public bool IsEqual(IfcCartesianPoint p, double tolerance)
        {
            return DistanceSquared(p) <= (tolerance * tolerance);
        }

        public double DistanceSquared(IfcCartesianPoint p)
        {
            double d = 0, dd;
            double x1, y1, z1, x2, y2, z2;
            XYZ(out x1, out y1, out z1);
            p.XYZ(out x2, out y2, out z2);
            dd = x1; dd -= x2; dd *= dd; d += dd;
            dd = y1; dd -= y2; dd *= dd; d += dd;
            dd = z1; dd -= z2; dd *= dd; d += dd;
            return d;
        }

        public void XYZ(out double x, out double y, out double z)
        {
            if (Dim == 3)
            {

                x = Coordinates[0]; y = Coordinates[1]; z = Coordinates[2];
            }
            else if (Dim == 2)
            {

                x = Coordinates[0]; y = Coordinates[1]; z = 0;
            }
            else
            {
                z = y = x = double.NaN;
            }
        }
    }
}
