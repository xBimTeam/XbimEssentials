﻿using System;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A set of connected faces
    /// </summary>
    public interface IXbimShell : IXbimGeometryObject, IEquatable<IXbimShell>
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
        bool CanCreateSolid();
        IXbimSolid CreateSolid();
        IXbimGeometryObjectSet Cut(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Cut(IXbimSolid toCut, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolid toCut, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolid toCut, double tolerance);
        IXbimFaceSet Section(IXbimFace toSection, double tolerance);
        void SaveAsBrep(string fileName);
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
    }
}
