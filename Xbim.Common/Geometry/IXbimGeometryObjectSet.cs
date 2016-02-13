using System;
using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A mixed collection of geometry objects
    /// </summary>
    public interface IXbimGeometryObjectSet : IEnumerable<IXbimGeometryObject>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimGeometryObject First { get; }
        IXbimSolidSet Solids { get; }
        IXbimShellSet Shells { get; }
        IXbimFaceSet Faces { get; }
        IXbimEdgeSet Edges { get; }
        IXbimVertexSet Vertices { get; }
        void Add(IXbimGeometryObject shape);
        IXbimGeometryObjectSet Cut(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Cut(IXbimSolid toCut, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolidSet toUnion, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolid toUnion, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolidSet toIntersect, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolid toIntersect, double tolerance);
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }

    }

}
