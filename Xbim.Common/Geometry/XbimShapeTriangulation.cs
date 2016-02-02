using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbim.Common.Geometry
{
    public class XbimShapeTriangulation : IXbimTriangulatedFaceSet
    {
        public readonly List<XbimPoint3D> _vertices;
        public readonly List<XbimFaceTriangulation> _faces;
        public readonly byte _version;

        public XbimShapeTriangulation(List<XbimPoint3D> vertices, List<XbimFaceTriangulation> faces, byte version)
        {
            _vertices = vertices;
            _faces = faces;
            _version = version;
        }



        /// <summary>
        /// Returns the number of triangles in the XbimShapeTriangulation data
        /// </summary>
        /// <param name="triangulationData"></param>
        /// <returns></returns>
        static public int TriangleCount(byte[] triangulationData)
        {
            return BitConverter.ToInt32(triangulationData, sizeof(byte) + sizeof(Int32));
        }

        /// <summary>
        /// Returns the number of vertices in the XbimShapeTriangulation data
        /// </summary>
        /// <param name="triangulationData"></param>
        /// <returns></returns>
        static public int VerticesCount(byte[] triangulationData)
        {
            return BitConverter.ToInt32(triangulationData, sizeof(byte));
        }

        public XbimShapeTriangulation Transform(XbimMatrix3D matrix3D)
        {
            var vertices = _vertices.Select(matrix3D.Transform).ToList();
            var faces = new List<XbimFaceTriangulation>(_faces.Count);
            var q = matrix3D.GetRotationQuaternion();
            faces.AddRange(_faces.Select(face => face.Transform(q)));
            return new XbimShapeTriangulation(vertices, faces, _version);
        }


        public void Write(BinaryWriter bw)
        {
            bw.Write((byte)_version); //stream format version
            bw.Write((Int32)_vertices.Count);
            bw.Write((Int32)_faces.Sum(face => face.TriangleCount));
            foreach (var v in _vertices)
            {
                bw.Write((float)v.X);
                bw.Write((float)v.Y);
                bw.Write((float)v.Z);
            }

            bw.Write((Int32)_faces.Count);
            foreach (var xbimFaceTriangulation in _faces)
            {

                if (xbimFaceTriangulation.IsPlanar)
                {
                    bw.Write((Int32)xbimFaceTriangulation.TriangleCount);
                    xbimFaceTriangulation.Normals[0].Write(bw);
                    xbimFaceTriangulation.WriteIndices(bw, _vertices.Count);
                }
                else
                {
                    bw.Write((Int32)(-xbimFaceTriangulation.TriangleCount));
                    xbimFaceTriangulation.WriteIndicesAndNormals(bw, _vertices.Count);
                }
            }
        }
    }
}
