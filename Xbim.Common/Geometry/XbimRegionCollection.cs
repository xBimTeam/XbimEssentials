using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Xbim.Common.Geometry
{
    public class XbimRegionCollection : List<XbimRegion>, IXbimShapeGeometryData
    {
        /// <summary>
        /// The IFC label of the geometric represenation context this region represents
        /// </summary>
        public int ContextLabel;

        #region Serialisation
        new public byte[] ToArray()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(Count);
            
            foreach (var region in this)
            {
                bw.Write(region.Name);
                bw.Write(region.Population);
                bw.Write((float)region.Centre.X);
                bw.Write((float)region.Centre.Y);
                bw.Write((float)region.Centre.Z);
                bw.Write((float)region.Size.X);
                bw.Write((float)region.Size.Y);
                bw.Write((float)region.Size.Z);
            }
            bw.Close();
            return ms.ToArray();
        }

        public static XbimRegionCollection FromArray(byte[] bytes)
        {
            var coll = new XbimRegionCollection();
            var ms = new MemoryStream(bytes);
            var br = new BinaryReader(ms);
            int count = br.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var region = new XbimRegion
                {
                    Name = br.ReadString(),
                    Population = br.ReadInt32(),
                    Centre =
                    {
                        X = br.ReadSingle(),
                        Y = br.ReadSingle(),
                        Z = br.ReadSingle()
                    },
                    
                };   
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();
                region.Size = new XbimVector3D(x,y,z);
                coll.Add(region);
            }
            return coll;
        }

        private void FillFromArray(byte[] bytes)
        {
            Clear();
            var ms = new MemoryStream(bytes);
            var br = new BinaryReader(ms);
            var count = br.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var region = new XbimRegion
                {
                    Name = br.ReadString(),
                    Population = br.ReadInt32(),
                    Centre =
                    {
                        X = br.ReadSingle(),
                        Y = br.ReadSingle(),
                        Z = br.ReadSingle()
                    },
                   
                };
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();
                region.Size = new XbimVector3D(x, y, z);
                Add(region);
            }
            
        }
        #endregion

        public XbimRegion MostPopulated()
        {
            var max = -1;
            XbimRegion mostPopulated = null;
            foreach (var region in this)
            {
                if (region.Population == -1) //indicates everything
                    return region;
                if (region.Population > max)
                {
                    mostPopulated = region;
                    max = region.Population;
                }
            }
            return mostPopulated;
        }

        public XbimRegion Largest()
        {
            double max = 0;
            XbimRegion largest = null;
            foreach (var region in this)
            {
                if (region.Diagonal() > max)
                {
                    largest = region;
                    max = region.Diagonal();
                }
            }
            return largest;
        }

        int IXbimShapeGeometryData.ShapeLabel { get; set; }

        int IXbimShapeGeometryData.IfcShapeLabel
        {
            get { return ContextLabel; }
            set { ContextLabel = value; }
        }

        int IXbimShapeGeometryData.GeometryHash
        {
            get
            {
                return 0;
            }
            set
            {
                
            }
        }

        int IXbimShapeGeometryData.Cost
        {
            get { return -1; }
        }

        int IXbimShapeGeometryData.ReferenceCount
        {
            get
            {
                return -1;
            }
            set
            {
               
            }
        }

        byte IXbimShapeGeometryData.LOD
        {
            get { return (byte)XbimLOD.LOD_Unspecified; }
            set
            {
                
            }
        }

        byte IXbimShapeGeometryData.Format
        {
            get
            {
                return (byte)XbimGeometryType.Region;
            }
            set
            {
                
            }
        }

        byte[] IXbimShapeGeometryData.BoundingBox
        {
            get
            {
                return XbimRect3D.Empty.ToFloatArray();
            }
            set
            {
               
            }
        }

        byte[] IXbimShapeGeometryData.ShapeDataCompressed
        {
            get
            {
                using (var msi = new MemoryStream(ToArray()))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        msi.CopyTo(gs);
                    }
                    return mso.ToArray();
                }
            }
            set
            {
                using (var msi = new MemoryStream(value))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }                   
                    FillFromArray(mso.ToArray());
                }
            }
        }

        byte[] IXbimShapeGeometryData.ShapeData
        {
            get { return ToArray(); }
            set
            {
                FillFromArray(value);
            }
        }
    }
}
