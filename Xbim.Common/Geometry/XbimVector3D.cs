using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Common.Geometry
{
    public struct XbimVector3D : IVector3D
    {
        public readonly static XbimVector3D Zero;

        static XbimVector3D()
        {
            Zero = new XbimVector3D(0, 0, 0);
        }
        public double X;
        public double Y;
        public double Z;
        
        public double Length 
        {
            get
            {
             return length();
            }
        }

        public double Modulus
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        double Angle(XbimVector3D other)
        {
            var cosinus = DotProduct(other);
            if (cosinus > -0.70710678118655 && cosinus < 0.70710678118655)
                return Math.Acos(cosinus);
            var sinus = CrossProduct(other).Length;
            if (cosinus < 0.0) return Math.PI - Math.Asin(sinus);
            return Math.Asin(sinus);
        }

        /// <summary>
        /// Returns true if the angle is less than tolerance
        /// </summary>
        /// <param name="other">other vector</param>
        /// <param name="angularTolerance">Tolerance in radians</param>
        /// <returns></returns>
        bool IsOpposite(XbimVector3D other, double angularTolerance)
        {
            return Math.PI - Angle (other) <= angularTolerance;
        }

        /// <summary>
        /// Returns true if the vectors are parallel
        /// </summary>
        /// <param name="other">other vector</param>
        /// <param name="angularTolerance">Tolerance in radians</param>
        /// <returns></returns>
        bool IsParallel(XbimVector3D other, double angularTolerance)
        {
            var ang = Angle(other);
            return ang <= angularTolerance || Math.PI - ang <= angularTolerance;
        }
           

        private double length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }


        public XbimVector3D(double vx, double vy, double vz)
        {
            X = vx;
            Y = vy;
            Z = vz;
        }

        public XbimVector3D(double v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        static public XbimVector3D Min(XbimVector3D a, XbimVector3D b)
        {
            return new XbimVector3D(
                (a.X < b.X) ? a.X : b.X,
                (a.Y < b.Y) ? a.Y : b.Y,
                (a.Z < b.Z) ? a.Z : b.Z);
        }
        static public XbimVector3D Max(XbimVector3D a, XbimVector3D b)
        {
            return new XbimVector3D(
                (a.X > b.X) ? a.X : b.X,
                (a.Y > b.Y) ? a.Y : b.Y,
                (a.Z > b.Z) ? a.Z : b.Z);
        }

        #region Operators

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override bool Equals(object ob)
        {
            if (ob is XbimVector3D)
            {
                XbimVector3D v = (XbimVector3D)ob;
                return (X == v.X && Y == v.Y && Z == v.Z);
            }
            else
                return false;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", X, Y, Z);
        }

        public static bool operator !=(XbimVector3D v1, XbimVector3D v2)
        {
            return !v1.Equals(v2);
        }

        public static bool operator ==(XbimVector3D v1, XbimVector3D v2)
        {
            return v1.Equals(v2);
        }

        public static XbimVector3D operator +(XbimVector3D vector1, XbimVector3D vector2)
        {
            return new XbimVector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        public static XbimVector3D Add(XbimVector3D vector1, XbimVector3D vector2)
        {
            return new XbimVector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        public static XbimVector3D operator -(XbimVector3D vector1, XbimVector3D vector2)
        {
            return new XbimVector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public static XbimVector3D Subtract(XbimVector3D vector1, XbimVector3D vector2)
        {
            return new XbimVector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public static XbimVector3D operator *(double l, XbimVector3D v1)
        {
            return XbimVector3D.Multiply(l, v1);
        }

        public static XbimVector3D operator *(XbimVector3D v1, double l)
        {
            return XbimVector3D.Multiply(l, v1);
        }
        
        public static XbimVector3D operator *(XbimVector3D v1, XbimMatrix3D m)
        {
            return XbimVector3D.Multiply(v1, m);
        }

        public static XbimVector3D Multiply(double val, XbimVector3D vec)
        {
            return new XbimVector3D( vec.X * val, vec.Y * val, vec.Z * val);
        }
        
        public static XbimVector3D Multiply(XbimVector3D vec, XbimMatrix3D m)
        {
            var x = vec.X;
            var y = vec.Y;
            var z = vec.Z;
            return new XbimVector3D (m.M11 * x + m.M21 * y + m.M31 * z ,
                                     m.M12 * x + m.M22 * y + m.M32 * z ,
                                     m.M13 * x + m.M23 * y + m.M33 * z 
                                    );
        }

        public void Normalize()
        {
            var x = X;
            var y = Y;
            var z = Z;
            var len = Math.Sqrt(x * x + y * y + z * z);

            if (len == 0)
            {
                X = 0; Y = 0; Z = 0;
                return;
            }

            len = 1 / len;
            X = x * len;
            Y = y * len;
            Z = z * len;

            if (Math.Abs(X) == 1)
                { Y = 0; Z = 0; }
            else if (Math.Abs(Y) == 1)
                { X = 0; Z = 0; }
            else if (Math.Abs(Z) == 1)
                { X = 0; Y = 0; }
        }

        public XbimVector3D CrossProduct(XbimVector3D v2)
        {
            return XbimVector3D.CrossProduct(this, v2);
        }

        public static XbimVector3D CrossProduct(XbimVector3D v1, XbimVector3D v2)
        {
            var x = v1.X;
            var y = v1.Y;
            var z = v1.Z;
            var x2 = v2.X;
            var y2 = v2.Y;
            var z2 = v2.Z;
            return new XbimVector3D(y * z2 - z * y2,
                                    z * x2 - x * z2,
                                    x * y2 - y * x2);
        }

        /// <summary>
        /// Makes the vector point in the opposite direction
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        

        public static double DotProduct(XbimVector3D v1, XbimVector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        public double DotProduct(XbimVector3D v2)
        {
            return XbimVector3D.DotProduct(this, v2);
        }
        #endregion





        double IVector3D.X
        {
            get { return X; }
        }

        double IVector3D.Y
        {
            get { return Y; }
        }

        double IVector3D.Z
        {
            get { return Z; }
        }

        public bool IsInvalid()
        {
            return (X == 0.0) && (Y == 0.0) && (Z == 0.0);
        }

        public bool IsEqual(XbimVector3D b, double precision = 1e-9)
        {
            double p = this.DotProduct(b);
            return Math.Abs(p - 1) <= precision;
        }


    }
}
