using System;
using System.Runtime.InteropServices;

namespace Xbim.Common.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct XbimPoint3D 
    {
        const double Tolerance = 1e-9;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

       
        public readonly static XbimPoint3D Zero = new XbimPoint3D(0, 0, 0);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public XbimPoint3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }



        public override bool Equals(object ob)
        {
            if (ob is XbimPoint3D)
            {
                XbimPoint3D v = (XbimPoint3D)ob;
                return (Math.Abs(X - v.X) < Tolerance && Math.Abs(Y - v.Y) < Tolerance && Math.Abs(Z - v.Z) < Tolerance);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }


        public static XbimPoint3D operator +(XbimPoint3D p, XbimVector3D v)
        {
            return Add(p, v);
        }
        /// <summary>
        /// Adds a XbimPoint3D structure to a XbimVector3D and returns the result as a XbimPoint3D structure.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static XbimPoint3D Add(XbimPoint3D p, XbimVector3D v)
        {
            return new XbimPoint3D (p.X + v.X,
                                    p.Y + v.Y,
                                    p.Z + v.Z
                                    );
        }

        public static XbimPoint3D Add(XbimPoint3D p, XbimPoint3D v)
        {
            return new XbimPoint3D(p.X + v.X,
                                    p.Y + v.Y,
                                    p.Z + v.Z
                                    );
        }

        public static XbimPoint3D operator *(XbimPoint3D p, XbimMatrix3D m)
        {
            return Multiply(p, m);
        }

       

        public static XbimPoint3D Multiply(XbimPoint3D p, XbimMatrix3D m)
        {
            var x = p.X;
            var y = p.Y; 
            var z = p.Z;
            

            
            XbimPoint3D pRet = new XbimPoint3D(m.M11 * x + m.M21 * y + m.M31 * z + m.OffsetX,
                                   m.M12 * x + m.M22 * y + m.M32 * z + m.OffsetY,
                                   m.M13 * x + m.M23 * y + m.M33 * z + m.OffsetZ
                                  );
            if (m.IsAffine) return pRet;
            double affineRatio = x * m.M14 + y * m.M24 + z * m.M34 + m.M44;
            x = pRet.X / affineRatio;
            y = pRet.Y / affineRatio;
            z = pRet.Z / affineRatio;
            return new XbimPoint3D(x, y, z);
        }
        public static XbimVector3D operator -(XbimPoint3D a, XbimPoint3D b)
        {
            return Subtract(a, b);
        }
        public static XbimPoint3D operator -(XbimPoint3D a, XbimVector3D b)
        {
            return new XbimPoint3D(a.X - b.X,
                                    a.Y - b.Y,
                                    a.Z - b.Z);
        }
        public static XbimVector3D Subtract(XbimPoint3D a, XbimPoint3D b)
        {
            return new XbimVector3D(a.X - b.X,
                                    a.Y - b.Y,
                                    a.Z - b.Z);
        }

        public static bool operator ==(XbimPoint3D left, XbimPoint3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(XbimPoint3D left, XbimPoint3D right)
        {
            return !(left == right);
        }
    }
}
