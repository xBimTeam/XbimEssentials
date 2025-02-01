using Microsoft.Extensions.Logging;
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
        double SurfaceArea { get; }
        bool IsPolyhedron { get; }
        IXbimSolidSet Cut(IXbimSolidSet toCut, double tolerance, ILogger logger=null);
        IXbimSolidSet Cut(IXbimSolid toCut, double tolerance, ILogger logger = null);
        IXbimSolidSet Union(IXbimSolidSet toUnion, double tolerance, ILogger logger = null);
        IXbimSolidSet Union(IXbimSolid toUnion, double tolerance, ILogger logger = null);
        IXbimSolidSet Intersection(IXbimSolidSet toIntersect, double tolerance, ILogger logger = null);
        IXbimSolidSet Intersection(IXbimSolid toIntersect, double tolerance, ILogger logger = null);
        IXbimFaceSet Section(IXbimFace toSection, double tolerance, ILogger logger = null);
        void SaveAsBrep(string fileName);
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
    }

}
