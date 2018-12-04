using System;

namespace Xbim.Common.Geometry
{
    public struct XbimQuaternion
    {
        #region members

        private static readonly XbimQuaternion _identity;
        private double _x;
        private double _y;
        private double _z;
        private double _w;
        private bool _isNotDefaultInitialised;

        public double X
        {
            get
            {
                if (!_isNotDefaultInitialised) this = _identity;
                return _x;
            }
            set
            {
                if (!_isNotDefaultInitialised) this = _identity;
                this._x = value;
            }
        }
        public double Y
        {
            get
            {
                if (!_isNotDefaultInitialised) this = _identity;
                return _y;
            }
            set
            {
                if (!_isNotDefaultInitialised) this = _identity;
                this._y = value;
            }
        }
        public double Z
        {
            get
            {
                if (!_isNotDefaultInitialised) this = _identity;
                return _z;
            }
            set
            {
                if (!_isNotDefaultInitialised) this = _identity;
                this._z = value;
            }
        }
        public double W
        {
            get
            {
                if (!_isNotDefaultInitialised) this = _identity;
                return _w;
            }
            set
            {
                if (!_isNotDefaultInitialised) this = _identity;
                this._w = value;
            }
        }
        #endregion

        static XbimQuaternion()
        {
            _identity = new XbimQuaternion(0.0, 0.0, 0.0, 1.0);
            _identity._isNotDefaultInitialised = true;
        }

       
        public XbimQuaternion(double x, double y, double z, double w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
            this._isNotDefaultInitialised = false;
        }


        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref XbimMatrix3D matrix, out XbimQuaternion result)
        {
            double sqrt;
            double half;
            double scale = matrix.M11 + matrix.M22 + matrix.M33;
            result = new XbimQuaternion();

            if (scale > 0.0f)
            {
                sqrt = Math.Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        public bool IsIdentity()
        {
            if (!_isNotDefaultInitialised) 
                return true;
            return Math.Abs(X - _identity.X) < double.Epsilon 
                   && Math.Abs(Y - _identity.Y) < double.Epsilon
                   && Math.Abs(Z - _identity.Z) < double.Epsilon
                   && Math.Abs(W - _identity.W) < double.Epsilon;
        }

        /// <summary>
        /// Transforms a 3D vector by the given rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The  rotation to apply.</param>
        /// <param name="result">When the method completes, contains the transformed .</param>
        public static void Transform(ref XbimVector3D vector, ref XbimQuaternion rotation, out XbimVector3D result)
        {
            double x = rotation.X + rotation.X;
            double y = rotation.Y + rotation.Y;
            double z = rotation.Z + rotation.Z;
            double wx = rotation.W * x;
            double wy = rotation.W * y;
            double wz = rotation.W * z;
            double xx = rotation.X * x;
            double xy = rotation.X * y;
            double xz = rotation.X * z;
            double yy = rotation.Y * y;
            double yz = rotation.Y * z;
            double zz = rotation.Z * z;

            result = new XbimVector3D(
                ((vector.X * ((1.0f - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy)),
                ((vector.X * (xy + wz)) + (vector.Y * ((1.0f - xx) - zz))) + (vector.Z * (yz - wx)),
                ((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1.0f - xx) - yy))
                );
        }

    }
}
