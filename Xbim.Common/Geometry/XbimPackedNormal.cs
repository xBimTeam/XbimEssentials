using System;
using System.IO;

namespace Xbim.Common.Geometry
{
    [System.CLSCompliant(false)]
    public struct XbimPackedNormal
    {
        private ushort _packedData;
        const double PackSize = 252;
        public UInt16 ToUnit16()
        {
            return _packedData;  
        }

        public XbimPackedNormal(UInt16 packedData)
        {
            _packedData = packedData;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(U);
            bw.Write(V);
        }

        public void Read(BinaryReader br)
        {
           _packedData = br.ReadUInt16();
        }

  

        public XbimPackedNormal(byte u, byte v)
        {
            _packedData = (ushort)(u << 8 | v);
           
        }

        /// <summary>
        /// Normalized normal vector. If the vector is not normalized packing will result in wrong results.
        /// </summary>
        /// <param name="x">X direction of the normalized normal vector</param>
        /// <param name="y">Y direction of the normalized normal vector</param>
        /// <param name="z">Z direction of the normalized normal vector</param>
        public XbimPackedNormal(double x, double y, double z)
        {
            const double tolerance = 1e-4;

            //the most basic case when normal points in Y direction (singular point)
            if (Math.Abs(1 - y) < tolerance)
            {
                _packedData = 0; 
               return;
            }
            //the most basic case when normal points in -Y direction (second singular point)
            if (Math.Abs(y - 1) < tolerance)
            {
                _packedData = 0 << 8 | (byte)PackSize / 2;
                return;
            }
            
            double lat;
            double lon;
            //special cases when vector is aligned to one of the axis
            if (Math.Abs(z - 1) < tolerance)
            {
                lon = 0;
                lat = Math.PI/2;
            }
            else if (Math.Abs(z + 1) < tolerance)
            {
                lon = Math.PI;
                lat = Math.PI / 2;
            }
            else if (Math.Abs(x - 1) < tolerance)
            {
                lon = Math.PI/2;
                lat = Math.PI / 2;
            }
            else if (Math.Abs(x + 1) < tolerance)
            {
                lon = Math.PI + Math.PI / 2;
                lat = Math.PI / 2;
            }
            else
            {
                //Atan2 takes care for quadrants (goes from positive Z to positive X and around)
                lon = Math.Atan2(x,z);
                //latitude from 0 to PI starting at positive Y ending at negative Y
                lat = Math.Acos(y);
            }

            //normalize values
            lon = lon / (2 * Math.PI);
            lat = lat / Math.PI;

            //stretch to pack size so that round directions are aligned to axes.
            var u = (int)(lon * PackSize);
            var v = (byte)(lat * PackSize);
            _packedData = (ushort)(u << 8 | v); 
        }

        public XbimPackedNormal(XbimVector3D vec)
            : this(vec.X, vec.Y, vec.Z)
        {    
        }

        public byte U
        {
            get
            { 
                return (byte)(_packedData >> 8);
                
            }
        }
        public byte V
        {
            get
            {
               return (byte)(_packedData & 0xff);
            }
        }

        public XbimVector3D Normal
        {
            get
            {
                var lon = U / PackSize * Math.PI * 2; 
                var lat = V / PackSize * Math.PI; 

                var y = Math.Cos(lat);
                var x = Math.Sin(lon) * Math.Sin(lat);
                var z = Math.Cos(lon) * Math.Sin(lat);

                var v3D = new XbimVector3D(x, y, z);
                v3D.Normalize();
                return v3D;
            }
        }

        public XbimPackedNormal Transform(XbimQuaternion q)
        {
            XbimVector3D v1 = Normal;
            XbimVector3D v2;
            XbimQuaternion.Transform(ref v1, ref q, out v2);
            return new XbimPackedNormal(v2);
        }
    
    }
}
