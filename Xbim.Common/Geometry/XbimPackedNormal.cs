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
                //double u = ((U / PackSize) * (2 * Math.PI)) ;

                //double v = ((V / PackSize) * (2 * Math.PI)) ;

                //double x = (Math.Sin(v) * Math.Cos(u));
                //double y = (Math.Sin(v) * Math.Sin(u));
                //double z = Math.Cos(v);
                //var v3D = new XbimVector3D(x, y, z);
                //v3D.Normalize();
                //return v3D;
                var lon = U / PackSize * Math.PI * 2; //lon
                var lat = V / PackSize * Math.PI; //lat

                var Y = Math.Cos(lat);
                var X = Math.Sin(lon) * Math.Sin(lat);
                var Z = Math.Cos(lon) * Math.Sin(lat);

                return new XbimVector3D(X, Y, Z);
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
