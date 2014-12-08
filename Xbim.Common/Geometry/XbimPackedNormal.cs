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
            
            double longitude;
            if (Math.Abs(x) < tolerance && Math.Abs(y) < tolerance)
            {
                longitude = 0;
                // latitude = 1;
            }
            else
            {
                longitude = Math.Acos(x / Math.Sqrt(x * x + y * y)) * (y < 0 ? -1 : 1);
            }
            double latitude = Math.Acos(z) * (z < 0 ? -1 : 1);
            //long and lat are between -1 and 1
            longitude = Math.Abs((longitude + Math.PI) / (2 * Math.PI));
            latitude = Math.Abs((latitude + Math.PI) / (2 * Math.PI));

            var u = (int)(longitude * PackSize);
            var v = (int)(latitude * PackSize);
            _packedData = (ushort)(u << 8 | v); 
        }

        public XbimPackedNormal(XbimVector3D vec)
        {
           
            const double tolerance = 1e-4;
            vec.Normalize(); //make sure it is normalised   
            double longitude;
            if (Math.Abs(vec.X) < tolerance && Math.Abs(vec.Y) < tolerance)
            {
                longitude = 0;
               // latitude = 1;
            }
            else
            {
                longitude = Math.Acos(vec.X / Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y)) * (vec.Y < 0 ? -1 : 1);        
            }
            double latitude = Math.Acos(vec.Z) * (vec.Z < 0 ? -1 : 1);
            //long and lat are between -1 and 1
            longitude = Math.Abs((longitude + Math.PI) / (2 * Math.PI));
            latitude = Math.Abs((latitude + Math.PI) / (2 * Math.PI));

            var u = (int)(longitude * PackSize);
            var v = (int)(latitude * PackSize);
            _packedData = (ushort)(u << 8 | v);      
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
                double u = ((U / PackSize) * (2 * Math.PI)) - Math.PI;

                double v = ((V / PackSize) * (2 * Math.PI)) - Math.PI;

                double x = (Math.Sin(v) * Math.Cos(u));
                double y = (Math.Sin(v) * Math.Sin(u));
                double z = Math.Cos(v);
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
