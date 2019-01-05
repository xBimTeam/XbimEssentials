using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Xbim.Common.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct XbimRect3D
    {
        private static readonly XbimRect3D _empty;

        public static XbimRect3D Empty
        {
            get { return XbimRect3D._empty; }
        } 

        #region Underlying Coordinate properties
        private double _x;
        private double _y;
        private double _z;
        private double _sizeX;
        private double _sizeY;
        private double _sizeZ;

        public double SizeX
        {
            get { return _sizeX; }
            set { _sizeX = value; }
        }


        public double SizeY
        {
            get { return _sizeY; }
            set { _sizeY = value; }
        }

        public double SizeZ
        {
            get { return _sizeZ; }
            set { _sizeZ = value; }
        }

       
        public XbimPoint3D Location
        {
            get
            {
                return new XbimPoint3D(_x, _y, _z);
            }
            set
            {
                this._x = value.X;
                this._y = value.Y;
                this._z = value.Z;
            }
        }


        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

       
        public bool IsEmpty
        {
            get
            {
                return SizeX < 0.0;
            }
        }
        #endregion

        #region Constructors
        public XbimRect3D(double x, double y, double z, double sizeX, double sizeY, double sizeZ)
        {
            _x = x;
            _y = y;
            _z = z;
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
        }

        public XbimRect3D(XbimPoint3D Position, XbimVector3D Size)
        {
            this._x = Position.X;
            this._y = Position.Y;
            this._z = Position.Z;
            this._sizeX = Size.X;
            this._sizeY = Size.Y;
            this._sizeZ = Size.Z;
        }

        public XbimRect3D(XbimPoint3D p1, XbimPoint3D p2)
        {
            this._x = Math.Min(p1.X, p2.X);
            this._y = Math.Min(p1.Y, p2.Y);
            this._z = Math.Min(p1.Z, p2.Z);
            this._sizeX = Math.Max(p1.X, p2.X) - this._x;
            this._sizeY = Math.Max(p1.Y, p2.Y) - this._y;
            this._sizeZ = Math.Max(p1.Z, p2.Z) - this._z;
        }

        static  XbimRect3D()
        {
            _empty = new XbimRect3D { _x = double.PositiveInfinity, _y = double.PositiveInfinity, _z = double.PositiveInfinity, _sizeX = double.NegativeInfinity, _sizeY = double.NegativeInfinity, _sizeZ = double.NegativeInfinity };
        }

        public XbimRect3D(XbimPoint3D highpt)
        {
            _x = highpt.X;
            _y = highpt.Y;
            _z = highpt.Z;
            _sizeX = 0.0;
            _sizeY = 0.0;
            _sizeZ = 0.0;
        }

        public XbimRect3D(XbimVector3D vMin, XbimVector3D vMax)
        {
            this._x = Math.Min(vMin.X, vMax.X);
            this._y = Math.Min(vMin.Y, vMax.Y);
            this._z = Math.Min(vMin.Z, vMax.Z);
            this._sizeX = Math.Max(vMin.X, vMax.X) - this._x;
            this._sizeY = Math.Max(vMin.Y, vMax.Y) - this._y;
            this._sizeZ = Math.Max(vMin.Z, vMax.Z) - this._z;
        }
        
        #endregion

        /// <summary>
        /// Minimum vertex
        /// </summary>
        
        public XbimPoint3D Min //This was returning maximum instead of minimum. Fixed by Martin Cerny 6/1/2014
        {
            get
            {
                return this.Location;
            }
        }
        /// <summary>
        /// Maximum vertex
        /// </summary>
       
        public XbimPoint3D Max  //This was returning minimum instead of maximum. Fixed by Martin Cerny 6/1/2014
        {
            get
            {
                return new XbimPoint3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ);
            }
        }

        
        public double Volume
        {
            get
            {
                return _sizeX * _sizeY * _sizeZ;
            }
        }

        #region Serialisation

        /// <summary>
        /// Reinitialises the rectangle 3D from the byte array
        /// </summary>
        /// <param name="array">6 doubles, definine, min and max values of the boudning box</param>
        public static XbimRect3D FromArray(byte[] array)
        {
            MemoryStream ms = new MemoryStream(array);
            BinaryReader bw = new BinaryReader(ms);
            XbimRect3D rect = new XbimRect3D();
            if (array.Length == 6 * sizeof(float)) //it is in floats
            {
                double srXmin = bw.ReadSingle();
                double srYmin = bw.ReadSingle();
                double srZmin = bw.ReadSingle();
                rect.Location = new XbimPoint3D(srXmin, srYmin, srZmin);

                double srXSz = bw.ReadSingle(); // all ToArray functions store position and size (bugfix: it was previously reading data as max)
                double srYSz = bw.ReadSingle();
                double srZSz = bw.ReadSingle();
                rect.SizeX = srXSz;
                rect.SizeY = srYSz;
                rect.SizeZ = srZSz;
            }
            else //go for doubles
            {
                double srXmin = bw.ReadDouble();
                double srYmin = bw.ReadDouble();
                double srZmin = bw.ReadDouble();
                rect.Location = new XbimPoint3D(srXmin, srYmin, srZmin);

                double srXSz = bw.ReadDouble(); // all ToArray functions store position and size (bugfix: it was previously reading data as max)
                double srYSz = bw.ReadDouble();
                double srZSz = bw.ReadDouble();
                rect.SizeX = srXSz;
                rect.SizeY = srYSz;
                rect.SizeZ = srZSz;
            }
           

            return rect;
        }

        /// <summary>
        /// Writes the Bounding Box as 6 doubles.
        /// </summary>
        /// <returns>An array of doubles (Position followed by Size).</returns>
        public byte[] ToDoublesArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(_x);
                    bw.Write(_y);
                    bw.Write(_z);
                    bw.Write(_sizeX);
                    bw.Write(_sizeY);
                    bw.Write(_sizeZ);
                    return ms.ToArray();
                }                    
            }
        }

        /// <summary>
        /// Writes the Bounding Box as 6 floats.
        /// </summary>
        /// <returns>An array of floats (Position followed by Size).</returns>
        public byte[] ToFloatArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw =new BinaryWriter(ms))
                {
                    bw.Write((float)_x);
                    bw.Write((float)_y);
                    bw.Write((float)_z);
                    bw.Write((float)_sizeX);
                    bw.Write((float)_sizeY);
                    bw.Write((float)_sizeZ);
                    return ms.ToArray();
                }
            }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4} {5}",
                _x, _y, _z, _sizeX, _sizeY, _sizeZ);
        }

        /// <summary>
        /// Imports values from a string
        /// </summary>
        /// <param name="Value">A space-separated string of 6 invariant-culture-formatted floats (x,y,z,sizeX,sizeY,sizeZ)</param>
        /// <returns>True if successful.</returns>
        public bool FromString(string Value)
        {
            string[] itms = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(itms.Length == 6);

            _x = Convert.ToSingle(itms[0], System.Globalization.CultureInfo.InvariantCulture);
            _y = Convert.ToSingle(itms[1], System.Globalization.CultureInfo.InvariantCulture);
            _z = Convert.ToSingle(itms[2], System.Globalization.CultureInfo.InvariantCulture);

            _sizeX = Convert.ToSingle(itms[3], System.Globalization.CultureInfo.InvariantCulture);
            _sizeY = Convert.ToSingle(itms[4], System.Globalization.CultureInfo.InvariantCulture);
            _sizeZ = Convert.ToSingle(itms[5], System.Globalization.CultureInfo.InvariantCulture);

            return true;
        }

        static public XbimRect3D Parse(string Value)
        {
            string[] itms = Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(itms.Length == 6);
            return new XbimRect3D(Convert.ToDouble(itms[0], System.Globalization.CultureInfo.InvariantCulture),
                                  Convert.ToDouble(itms[1], System.Globalization.CultureInfo.InvariantCulture),
                                  Convert.ToDouble(itms[2], System.Globalization.CultureInfo.InvariantCulture),
                                  Convert.ToDouble(itms[3], System.Globalization.CultureInfo.InvariantCulture),
                                  Convert.ToDouble(itms[4], System.Globalization.CultureInfo.InvariantCulture),
                                  Convert.ToDouble(itms[5], System.Globalization.CultureInfo.InvariantCulture));

        }

        #endregion

        public XbimRect3D Inflate( double x, double y, double z)
        {
            return Inflate(this, x,y,z);
        }

        static public XbimRect3D Inflate(XbimRect3D original, double x, double y, double z)
        {           
            XbimPoint3D p = new XbimPoint3D(original.X - x, original.Y - y, original.Z - z);
            XbimVector3D v = new XbimVector3D(original.SizeX + (x * 2), original.SizeY + (y * 2), original.SizeZ +(z * 2));
            return new XbimRect3D(p,v);
        }

        static public XbimRect3D Inflate(XbimRect3D original, double inflate)
        {
            XbimPoint3D p = new XbimPoint3D(original.X - inflate, original.Y - inflate, original.Z - inflate);
            XbimVector3D v = new XbimVector3D(original.SizeX + (inflate* 2), original.SizeY + (inflate * 2), original.SizeZ + (inflate * 2));
            return new XbimRect3D(p, v);
        }

        public XbimRect3D Inflate(double d)
        {
            return Inflate(this, d);
        }

        /// <summary>
        /// Calculates the centre of the 3D rect
        /// </summary>
        public XbimPoint3D Centroid()
        {
            if (IsEmpty) 
                return new XbimPoint3D(0, 0, 0);
            else
                return new XbimPoint3D((X + SizeX / 2), (Y + SizeY / 2), (Z + SizeZ / 2));
        }

        /// <summary>
        /// Transforms a bounding rect so that it is still axis aligned
        /// </summary>
        /// <param name="rect3d"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        static public XbimRect3D TransformBy(XbimRect3D rect3d, XbimMatrix3D m)
        {
            XbimPoint3D min = rect3d.Min;
            XbimPoint3D max = rect3d.Max;
            XbimVector3D up = m.Up;
            XbimVector3D right = m.Right;
            XbimVector3D backward = m.Backward;
            var xa = right * min.X;
            var xb = right * max.X;

            var ya = up * min.Y;
            var yb = up * max.Y;

            var za = backward * min.Z;
            var zb = backward * max.Z;

            return new XbimRect3D(
                XbimVector3D.Min(xa, xb) + XbimVector3D.Min(ya, yb) + XbimVector3D.Min(za, zb) + m.Translation,
                XbimVector3D.Max(xa, xb) + XbimVector3D.Max(ya, yb) + XbimVector3D.Max(za, zb) + m.Translation
            );
            
        }

        public void Union(XbimRect3D bb)
        {
            if (IsEmpty)
            {
                X = bb.X;
                Y = bb.Y;
                Z = bb.Z;
                SizeX = bb.SizeX;
                SizeY = bb.SizeY;
                SizeZ = bb.SizeZ;
            }
            else if (!bb.IsEmpty)
            {
                double numX = Math.Min(X, bb.X);
                double numY = Math.Min(Y, bb.Y);
                double numZ = Math.Min(Z, bb.Z);
                _sizeX = Math.Max((X + _sizeX), (bb.X + bb._sizeX)) - numX;
                _sizeY = Math.Max((Y + _sizeY), (bb.Y + bb._sizeY)) - numY;
                _sizeZ = Math.Max((Z + _sizeZ), (bb.Z + bb._sizeZ)) - numZ;
                X = numX;
                Y = numY;
                Z = numZ;
            }
        }

        public void Union(XbimPoint3D highpt)
        {
            Union(new XbimRect3D(highpt, highpt));
        }

        public bool Contains(double x, double y, double z)
        {
            if (this.IsEmpty)
            {
                return false;
            }
            return this.ContainsCoords(x, y, z);
        }

        public bool Contains(XbimPoint3D pt)
        {
            if (this.IsEmpty)
            {
                return false;
            }
            return this.ContainsCoords(pt.X, pt.Y, pt.Z);
        }

        private bool ContainsCoords(double x, double y, double z)
        {
            return (((((x >= this._x) && (x <= (this._x + this._sizeX))) && ((y >= this._y) && (y <= (this._y + this._sizeY)))) && (z >= this._z)) && (z <= (this._z + this._sizeZ)));
  
        }

        public bool Intersects(XbimRect3D rect)
        {
            //Martin Cerny: I don't think this is correct as this will find only one specific intersection case but it is not general.
            //I propose variant based on size and envelope bounding box which would be invariant

            //XbimRect3D envelope = XbimRect3D.Empty;
            //envelope.Union(rect);
            //envelope.Union(this);

            //var xSize = rect.SizeX + this.SizeX;
            //var ySize = rect.SizeY + this.SizeY;
            //var zSize = rect.SizeZ + this.SizeZ;

            //return
            //    xSize >= envelope.SizeX &&
            //    ySize >= envelope.SizeY &&
            //    zSize >= envelope.SizeZ
            //    ;

            if (this.IsEmpty || rect.IsEmpty)
            {
                return false;
            }
            return (((((rect._x <= (this._x + this._sizeX)) && ((rect._x + rect._sizeX) >= this._x)) && ((rect._y <= (this._y + this._sizeY)) && ((rect._y + rect._sizeY) >= this._y))) && (rect._z <= (this._z + this._sizeZ))) && ((rect._z + rect._sizeZ) >= this._z));
        }
       

        public bool Contains(XbimRect3D rect)
        {
            if (this.IsEmpty)
                return false;
            return 
                this.Contains(rect.Min) 
                && 
                this.Contains(rect.Max);
        }

       /// <summary>
       /// Returns the radius of the sphere that contains this bounding box rectangle 3D
       /// </summary>
       /// <returns></returns>
        public double Radius()
        {
            XbimVector3D max = new XbimVector3D(SizeX, SizeY, SizeZ);
            double len = max.Length;
            if (len != 0)
                return  len / 2;
            else
                return 0;
        }

        /// <summary>
        /// Indicative size of the Box along all axis.
        /// </summary>
        /// <returns>Returns the length of the diagonal</returns>
        public double Length()
        {
            XbimVector3D max = new XbimVector3D(SizeX, SizeY, SizeZ);
            return max.Length;
        }

        /// <summary>
        /// Warning: This function assumes no rotation is used for the tranform.
        /// </summary>
        /// <param name="composed">The NON-ROTATING transform to apply</param>
        /// <returns>the transformed bounding box.</returns>
        public XbimRect3D Transform(XbimMatrix3D composed)
        {
            var min = this.Min * composed;
            var max = this.Max * composed;

            return new XbimRect3D(min, max);
        }

        /// <summary>
        /// Rounds the values of the bounding box to the specified precision
        /// </summary>
        /// <param name="digits"></param>
        public void Round(int digits)
        {

            _x = Math.Round(_x, digits);
            _y = Math.Round(_y, digits);
            _z = Math.Round(_z, digits);
            _sizeX = Math.Round(_sizeX, digits);
            _sizeY = Math.Round(_sizeY, digits);
            _sizeZ = Math.Round(_sizeZ, digits);
        }

        /// <summary>
        /// Rounds the values of the bounding box to the specified precision and returns a copy
        /// </summary>
        /// <param name="r"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static XbimRect3D Round(XbimRect3D r, int digits)
        {
            return new XbimRect3D(Math.Round(r.X, digits),
                Math.Round(r.Y, digits),
                Math.Round(r.Z, digits),
                Math.Round(r.SizeX, digits),
                Math.Round(r.SizeY, digits),
                Math.Round(r.SizeZ, digits)
                );
        }


        /// <summary>
        /// true if the rect fits inside thsi rectangle when it is either inflated or defalted by the tolerance
        /// </summary>            
        /// <returns></returns>
        public bool IsSimilar(XbimRect3D rect, double tolerance)
        {
            double t = Math.Abs(tolerance);
            double t2 = 2 * t;
            return 
                Math.Abs(_x - rect.X) <= t &&
                Math.Abs(_y - rect.Y) <= t &&
                Math.Abs(_z - rect.Z) <= t &&
                Math.Abs(_sizeX - rect.SizeX) <= t2 &&
                Math.Abs(_sizeY - rect.SizeY) <= t2 &&
                Math.Abs(_sizeZ - rect.SizeZ) <= t2;

        }

       
    }
}
