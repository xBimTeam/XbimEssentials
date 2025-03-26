using Microsoft.Extensions.Logging;
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
        IXbimGeometryObjectSet Cut(IXbimSolidSet toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Cut(IXbimSolid toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Union(IXbimSolidSet toUnion, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Union(IXbimSolid toUnion, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Intersection(IXbimSolidSet toIntersect, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Intersection(IXbimSolid toIntersect, double tolerance, ILogger logger = null);
        /// <summary>
        /// Sews the objects to remove duplicate vertices and edges and make the highest level topology
        /// </summary>
        /// <returns>True if the object was sewn, false it is already sewn</returns>
        bool Sew();
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
        /// <summary>
        /// Returns the partial volume of the set which is closed and valid.
        /// </summary>
        double VolumeValid { get; }
    }

}
