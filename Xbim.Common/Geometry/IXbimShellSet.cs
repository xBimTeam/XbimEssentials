using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimShellSet : IEnumerable<IXbimShell>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimShell First { get; }
        bool IsPolyhedron { get; }
        IXbimGeometryObjectSet Cut(IXbimSolidSet toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Cut(IXbimSolid toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Union(IXbimSolidSet toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Union(IXbimSolid toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Intersection(IXbimSolidSet toCut, double tolerance, ILogger logger = null);
        IXbimGeometryObjectSet Intersection(IXbimSolid toCut, double tolerance, ILogger logger = null);
        void Add(IXbimGeometryObject shape);
        /// <summary>
        /// Unions all elements in the  set and updates the set to the result
        /// </summary>
        /// <param name="tolerance"></param>
        void Union(double tolerance);
    }
}
