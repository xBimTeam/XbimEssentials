using System;

namespace Xbim.Common.Geometry
{
    public struct XbimVector3D : IVector3D
    {
        public static readonly XbimVector3D Zero;
        const double Tolerance = 1e-9;
        static XbimVector3D()
        {
            Zero = new XbimVector3D(0, 0, 0);
        }
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        
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

        public double Angle(XbimVector3D other)
        {
            var cosinus = DotProduct(other)/(Length*other.Length);
            if (cosinus > -0.70710678118655 && cosinus < 0.70710678118655)
                return Math.Acos(cosinus);
            var sinus = CrossProduct(other).Length / (Length * other.Length);
            if (cosinus < 0.0) return Math.PI - Math.Asin(sinus);
            return Math.Asin(sinus);
        }

        /// <summary>
        /// Returns true if the angle is less than tolerance
        /// </summary>
        /// <param name="other">other vector</param>
        /// <param name="angularTolerance">Tolerance in radians</param>
        /// <returns></returns>
        public bool IsOpposite(XbimVector3D other, double angularTolerance)
        {
            return Math.PI - Angle (other) <= angularTolerance;
        }

        /// <summary>
        /// Returns true if the vectors are parallel
        /// </summary>
        /// <param name="other">other vector</param>
        /// <param name="angularTolerance">Tolerance in radians</param>
        /// <returns></returns>
        public bool IsParallel(XbimVector3D other, double angularTolerance)
        {
            var ang = Angle(other);
            return ang <= angularTolerance || Math.PI - ang <= angularTolerance;
        }
            /// <summary>
        /// Returns true if the vectors are normal
        /// </summary>
        /// <param name="other">other vector</param>
        /// <param name="angularTolerance">Tolerance in radians</param>
        /// <returns></returns>
        public bool IsNormal(XbimVector3D other, double angularTolerance)
        {
            var ang = Math.PI / 2.0 - Angle(other);
            if (ang < 0) ang = -ang;
            return ang <= angularTolerance;
        }

        // ReSharper disable once InconsistentNaming
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
                return (Math.Abs(X - v.X) < Tolerance && Math.Abs(Y - v.Y) < Tolerance && Math.Abs(Z - v.Z) < Tolerance);
            }
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
            return Multiply(l, v1);
        }

        public static XbimVector3D operator *(XbimVector3D v1, double l)
        {
            return Multiply(l, v1);
        }
        
        public static XbimVector3D operator *(XbimVector3D v1, XbimMatrix3D m)
        {
            return Multiply(v1, m);
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

        public XbimVector3D Normalized()
        {
            var x = X;
            var y = Y;
            var z = Z;
            var len = Math.Sqrt(x * x + y * y + z * z);

            if (Math.Abs(len) < Tolerance)
            {               
                return new XbimVector3D(0,0,0);
            }

            len = 1 / len;
            x = x * len;
            y = y * len;
            z = z * len;

            if (Math.Abs(x - 1) < Tolerance)
                return new XbimVector3D(Math.Sign(x), 0, 0);
            if (Math.Abs(y - 1) < Tolerance)
                return new XbimVector3D(0, Math.Sign(y), 0);
            if (Math.Abs(z - 1) < Tolerance)
                return new XbimVector3D(0, 0, Math.Sign(z));
            return new XbimVector3D(x, y, z);
        }

        public XbimVector3D CrossProduct(XbimVector3D v2)
        {
            return CrossProduct(this, v2);
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
        public XbimVector3D Negated()
        {
            return new XbimVector3D(-X,-Y,-Z);
        }

        

        public static double DotProduct(XbimVector3D v1, XbimVector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        public double DotProduct(XbimVector3D v2)
        {
            return DotProduct(this, v2);
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
            return (Math.Abs(X) < Tolerance) && (Math.Abs(Y) < Tolerance) && (Math.Abs(Z) < Tolerance);
        }

        public bool IsEqual(XbimVector3D b, double precision = 1e-9)
        {
            double p = DotProduct(b);
            return Math.Abs(p - 1) <= precision;
        }


    }
}
