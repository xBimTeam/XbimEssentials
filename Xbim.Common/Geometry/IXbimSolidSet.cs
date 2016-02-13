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
        /// <param name="solid"></param>
        void Add(IXbimGeometryObject shape);
        bool IsPolyhedron { get; }
        IXbimSolidSet Cut(IXbimSolidSet toCut, double tolerance);
        IXbimSolidSet Cut(IXbimSolid toCut, double tolerance);
        IXbimSolidSet Union(IXbimSolidSet toUnion, double tolerance);
        IXbimSolidSet Union(IXbimSolid toUnion, double tolerance);
        IXbimSolidSet Intersection(IXbimSolidSet toIntersect, double tolerance);
        IXbimSolidSet Intersection(IXbimSolid toIntersect, double tolerance);
        bool IsSimplified { get; }
        /// <summary>
        /// Converts the object to a string in BRep format
        /// </summary>
        String ToBRep { get; }
    }
}
