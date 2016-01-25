using System;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A manifold closed shell
    /// </summary>
    public interface IXbimSolid : IXbimGeometryObject, IEquatable<IXbimSolid>
    {
       
        IXbimShellSet Shells { get; }
        IXbimFaceSet Faces { get; }
        IXbimEdgeSet Edges { get; }
        IXbimVertexSet Vertices { get; }
        double Volume { get; }
        double SurfaceArea { get; }
        bool IsPolyhedron { get; }
        IXbimSolidSet Cut(IXbimSolidSet toCut, double tolerance);
        IXbimSolidSet Cut(IXbimSolid toCut, double tolerance);
        IXbimSolidSet Union(IXbimSolidSet toUnion, double tolerance);
        IXbimSolidSet Union(IXbimSolid toUnion, double tolerance);
        IXbimSolidSet Intersection(IXbimSolidSet toIntersect, double tolerance);
        IXbimSolidSet Intersection(IXbimSolid toIntersect, double tolerance);
        IXbimFaceSet Section(IXbimFace toSection, double tolerance);
        void SaveAsBrep(string fileName);
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
    }

}
