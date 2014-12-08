#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    MeshGeometry.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#endregion

namespace Xbim.IO.GeometryCache
{
    public class MeshGeometry
    {
        private List<MeshGeometry> _children = new List<MeshGeometry>();

        public List<MeshGeometry> Children
        {
            get { return _children; }
            set { _children = value; }
        }


        private Int32Collection _indices = new Int32Collection();

        public Int32Collection TriangleIndices
        {
            get { return _indices; }
            set { _indices = value; }
        }

        private Vector3DCollection _normals = new Vector3DCollection();

        public Vector3DCollection Normals
        {
            get { return _normals; }
            set { _normals = value; }
        }

        private Point3DCollection _positions = new Point3DCollection();

        public Point3DCollection Positions
        {
            get { return _positions; }
            set { _positions = value; }
        }

        internal void AddChild(MeshGeometry meshGeometry)
        {
            if (_children == null)
                _children = new List<MeshGeometry>();
            _children.Add(meshGeometry);
        }
    }
}