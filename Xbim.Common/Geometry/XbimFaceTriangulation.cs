using System;
using System.Collections.Generic;
using System.IO;

namespace Xbim.Common.Geometry
{
    public class XbimFaceTriangulation
    {
        public List<int> _indices;
        public List<XbimPackedNormal> _normals;
        public XbimFaceTriangulation(int numTriangles, int numNormals)
        {
            _normals = new List<XbimPackedNormal>(numNormals);
            _indices = new List<int>(numTriangles * 3);
        }



        internal void AddNormal(XbimPackedNormal xbimPackedNormal)
        {
            _normals.Add(xbimPackedNormal);
        }

        internal void AddIndex(int p)
        {
            _indices.Add(p);
        }

        public bool IsPlanar
        {
            get { return _normals.Count == 1; }
        }

        public int TriangleCount
        {
            get { return _indices.Count / 3; }
        }
        public int NormalCount
        {
            get { return _normals.Count; }
        }

        public IList<XbimPackedNormal> Normals
        {
            get { return _normals; }
        }

        public void WriteIndices(BinaryWriter bw, int vertexCount)
        {

            if (vertexCount <= 0xFF)
                foreach (var triangle in _indices) bw.Write((byte)triangle);
            else if (vertexCount <= 0xFFFF)
                foreach (var triangle in _indices) bw.Write((UInt16)triangle);
            else
                foreach (var triangle in _indices) bw.Write(triangle);
        }

        public void WriteIndicesAndNormals(BinaryWriter bw, int vertexCount)
        {

            if (vertexCount <= 0xFF)
                for (int i = 0; i < _indices.Count; i++)
                {
                    bw.Write((byte)_indices[i]);
                    _normals[i].Write(bw);
                }

            else if (vertexCount <= 0xFFFF)
                for (int i = 0; i < _indices.Count; i++)
                {
                    bw.Write((UInt16)_indices[i]);
                    _normals[i].Write(bw);
                }
            else
                for (int i = 0; i < _indices.Count; i++)
                {
                    bw.Write(_indices[i]);
                    _normals[i].Write(bw);
                }
        }

        public XbimFaceTriangulation Transform(XbimQuaternion q)
        {
            var result = new XbimFaceTriangulation(_indices.Count, _normals.Count);
            foreach (var normal in _normals)
                result.AddNormal(normal.Transform(q));
            foreach (var index in _indices)
                result.AddIndex(index);
            return result;
        }
    }
}
