using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable RedundantCast
// ReSharper disable BuiltInTypeReferenceStyle

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// represents a 3d mesh
    /// </summary>
    public class XbimShapeTriangulation : IXbimTriangulatedFaceSet
    {
        private readonly List<XbimPoint3D> _vertices;
        private readonly List<XbimFaceTriangulation> _faces;
        private readonly byte _version;

        public IList<XbimPoint3D> Vertices
        {
            get { return _vertices; }
        }

        public IList<XbimFaceTriangulation> Faces
        {
            get { return _faces; }
        }
        
        public byte Version
        {
            get { return _version; }
        }

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
        public static int TriangleCount(byte[] triangulationData)
        {
            return BitConverter.ToInt32(triangulationData, sizeof(byte) + sizeof(Int32));
        }

        /// <summary>
        /// Returns the number of vertices in the XbimShapeTriangulation data
        /// </summary>
        /// <param name="triangulationData"></param>
        /// <returns></returns>
        public static int VerticesCount(byte[] triangulationData)
        {
            return BitConverter.ToInt32(triangulationData, sizeof(byte));
        }

        public XbimShapeTriangulation Transform(XbimMatrix3D matrix3D)
        {
            var vertices =_vertices.Select(matrix3D.Transform).ToList();
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
                    bw.Write((Int32) xbimFaceTriangulation.TriangleCount);
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

        private struct PointAndNormal
        {
            internal readonly int Index;
            internal XbimPackedNormal Normal;
            
            public PointAndNormal(int index, XbimPackedNormal n) 
            {
                Index = index;
                Normal = n;
            }
        }

        /// <summary>
        /// Exports the data to positions and indices, trying to remove duplicates in positions as much as possible.
        /// </summary>
        /// <param name="positions">The destination position list, [0,1,2] for position [3,4,5] for normal.</param>
        /// <param name="indices">the destination of indices pointing to positions.</param>
        public void ToPointsWithNormalsAndIndices(out List<float[]> positions, out List<int> indices)
        {
            var ps = new Dictionary<PointAndNormal, int>();
            indices = new List<int>();
            foreach (var face in Faces)
            {
                if (face.IsPlanar)
                {
                    var n = face.Normals[0];
                    foreach (var index in face.Indices)
                    {
                        var p = new PointAndNormal(index, n);
                        int id;
                        if (!ps.TryGetValue(p, out id))
                        {
                            id = ps.Count;
                            ps.Add(p, id);
                        }
                        indices.Add(id);
                    }                 
                }
                else
                {
                    for (var i = 0; i < face.Indices.Count; i++)
                    {
                        var index = face.Indices[i];
                        var n = face.Normals[i];
                        var p = new PointAndNormal(index, n);
                        int id;
                        if (!ps.TryGetValue(p, out id))
                        {
                            id = ps.Count;
                            ps.Add(p, id);
                        }
                        indices.Add(id);
                    }
                }
            }
            positions = new List<float[]>(ps.Count);

            foreach (var pointAndNormal in ps.Keys)
            {
                var f = new float[6];

                var p = _vertices[pointAndNormal.Index];
                f[0] = (float)p.X;
                f[1] = (float)p.Y;
                f[2] = (float)p.Z;

                var v = pointAndNormal.Normal.Normal;
                f[3] = (float)v.X;
                f[4] = (float)v.Y;
                f[5] = (float)v.Z;

                positions.Add(f);
            }
        }
    }
}
