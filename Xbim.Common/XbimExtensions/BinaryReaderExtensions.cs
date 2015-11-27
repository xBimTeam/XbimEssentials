using System;
using System.Collections.Generic;
using System.IO;
using Xbim.Common.Geometry;

namespace Xbim.Common.XbimExtensions
{
    [CLSCompliant(false)]
    public static class BinaryReaderExtensions
    {
        public static XbimShapeTriangulation ReadShapeTriangulation(this BinaryReader br)
        {
            var version = br.ReadByte(); //stream format version
			var numVertices = br.ReadInt32();
            // ReSharper disable once UnusedVariable
            var numTriangles = br.ReadInt32();
            var vertices = new List<XbimPoint3D>(numVertices);

            for (var i = 0; i < numVertices; i++)
            {
                vertices.Add(br.ReadPointFloat3D());
            }
            var numFaces = br.ReadInt32();
            var faces = new List<XbimFaceTriangulation>(numFaces);
            for (var i = 0; i < numFaces; i++)
            {
                
                var numTrianglesInFace = br.ReadInt32();
                if (numTrianglesInFace == 0) continue;
                var isPlanar = numTrianglesInFace > 0;
                numTrianglesInFace = Math.Abs(numTrianglesInFace);
                if (isPlanar)
                {
                     var faceTriangulation = new XbimFaceTriangulation(numTrianglesInFace, 1);
                     faceTriangulation.AddNormal(br.ReadPackedNormal());
                     for (var j = 0; j < numTrianglesInFace; j++)
                     {
                         faceTriangulation.AddIndex(br.ReadIndex(numVertices));//a
                         faceTriangulation.AddIndex(br.ReadIndex(numVertices));//b
                         faceTriangulation.AddIndex(br.ReadIndex(numVertices));//c
                     }
                     faces.Add(faceTriangulation);
                }
                else
                {
                    var faceTriangulation = new XbimFaceTriangulation(numTrianglesInFace, numTrianglesInFace*3);
                    for (var j = 0; j < numTrianglesInFace; j++)
                    {
                        faceTriangulation.AddIndex(br.ReadIndex(numVertices));//a
                        faceTriangulation.AddNormal(br.ReadPackedNormal());
                        faceTriangulation.AddIndex(br.ReadIndex(numVertices));//b
                        faceTriangulation.AddNormal(br.ReadPackedNormal());
                        faceTriangulation.AddIndex(br.ReadIndex(numVertices));//c
                        faceTriangulation.AddNormal(br.ReadPackedNormal());
                    }
                    faces.Add(faceTriangulation);
                }
            }
            return new XbimShapeTriangulation(vertices, faces, version);
        }

        private static int ReadIndex(this BinaryReader br, int maxVertexCount)
        {
            if (maxVertexCount <= 0xFF)
                return br.ReadByte();
            else if(maxVertexCount <= 0xFFFF)
                return br.ReadUInt16();
            else 
                return (int)br.ReadUInt32(); //this should never go over int32

        }
        public static XbimPoint3D ReadPointFloat3D(this BinaryReader br)
        {
            double x = br.ReadSingle();
            double y = br.ReadSingle();
            double z = br.ReadSingle();
            return new XbimPoint3D(x,y,z);
        }

        public static XbimPackedNormal ReadPackedNormal(this BinaryReader br)
        {
            byte u = br.ReadByte();
            byte v = br.ReadByte();
            return new XbimPackedNormal(u,v);
        }

    }
}
