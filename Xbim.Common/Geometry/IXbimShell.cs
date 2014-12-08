using System;
using System.Collections.Generic;
using Xbim.Common.Geometry;

namespace XbimGeometry.Interfaces
{
    /// <summary>
    /// A set of connected faces
    /// </summary>
    public interface IXbimShell : IXbimGeometryObject, IXbimBooleanOperand, IEquatable<IXbimShell>
    {

        IXbimFaceSet Faces { get; }
        IXbimEdgeSet Edges { get; }
        IXbimVertexSet Vertices { get; }
        double SurfaceArea { get; }
        bool IsPolyhedron { get; }
        /// <summary>
        /// The shell is a closed manifold shape
        /// </summary>
        bool IsClosed { get; }
    }
}
