using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Xbim.Common.Geometry
{
    public struct XbimPackedNormal
    {
        private ushort _packedData;
        const double PackSize = 252;
       

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

        public XbimPackedNormal(double x, double y, double z)
        {
            const double tolerance = 1e-4;

            if (Math.Abs(y) < tolerance || Math.Abs(y - 1) < tolerance)
            {
                byte half = (byte)PackSize/2;
                _packedData = (ushort)(half << 8 | (byte)y); 
               
            }

            var lon = Math.Abs(z) < tolerance ? Math.PI / 2 : (Math.Atan(x / z));

            // Need to correct Atan's value based on quadrant
            if ((x >= 0 && z >= 0) || (x < 0 && z >= 0))
            {
                lon = (Math.PI / 2) - lon;
            }
            else if (x < 0 && z < 0)
            {
                lon = Math.PI * 1.5 - Math.Abs(lon);
            }
            else if (x >= 0 && z < 0)
            {
                lon = Math.PI * 1.5 + Math.Abs(lon);
            }

            var lat = Math.Acos(y);


            lon = lon / (2 * Math.PI);
            lat =  1 - lat / Math.PI;


            //double longitude;
            //if (Math.Abs(x) < tolerance && Math.Abs(y) < tolerance)
            //{
            //    longitude = 0;
            //    // latitude = 1;
            //}
            //else
            //{
            //    longitude = Math.Acos(x / Math.Sqrt(x * x + y * y)) * (y < 0 ? -1 : 1);
            //}
            //double latitude = Math.Acos(z) * (z < 0 ? -1 : 1);
            ////long and lat are between -1 and 1
            //if (longitude < 0) longitude += 2*Math.PI;
            //if (latitude < 0) latitude += 2 * Math.PI;
            //longitude /= 2*Math.PI;
            //latitude /= 2 * Math.PI;
            ////longitude = Math.Abs((longitude + Math.PI) / (2 * Math.PI));
            ////latitude = Math.Abs((latitude + Math.PI) / (2 * Math.PI));

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
                //double u = ((U / PackSize) * (2 * Math.PI)) ;

                //double v = ((V / PackSize) * (2 * Math.PI)) ;

                //double x = (Math.Sin(v) * Math.Cos(u));
                //double y = (Math.Sin(v) * Math.Sin(u));
                //double z = Math.Cos(v);
                //var v3D = new XbimVector3D(x, y, z);
                //v3D.Normalize();
                //return v3D;
                var u = U / PackSize;
                var v = V / PackSize;
                var theta = (v-1) * Math.PI * -1;
                var sinTheta = Math.Sin(theta);
                var cosTheta = Math.Cos(theta);

                var phi = u * 2 * Math.PI;
                var sinPhi = Math.Sin(phi);
                var cosPhi = Math.Cos(phi);

                var nX = cosPhi * sinTheta;
                var nY = cosTheta;
                var nZ = sinPhi * sinTheta;

                return new XbimVector3D(nX, nY, nZ);
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
