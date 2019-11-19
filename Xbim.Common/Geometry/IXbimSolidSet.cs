using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimSolidSet : IEnumerable<IXbimSolid>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimSolid First { get; }
        /// <summary>
        /// Will add any solids in the geomtry object to the set
        /// </summary>
        void Add(IXbimGeometryObject shape);
        bool IsPolyhedron { get; }
        IXbimSolidSet Cut(IXbimSolidSet toCut, double tolerance, ILogger logger = null);
        IXbimSolidSet Cut(IXbimSolid toCut, double tolerance, ILogger logger = null);
        IXbimSolidSet Union(IXbimSolidSet toUnion, double tolerance, ILogger logger = null);
        IXbimSolidSet Union(IXbimSolid toUnion, double tolerance, ILogger logger = null);
        IXbimSolidSet Intersection(IXbimSolidSet toIntersect, double tolerance, ILogger logger = null);
        IXbimSolidSet Intersection(IXbimSolid toIntersect, double tolerance, ILogger logger = null);
        bool IsSimplified { get; }
        IXbimSolidSet Range(int start, int count);
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
