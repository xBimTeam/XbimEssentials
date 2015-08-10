using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimShellSet : IEnumerable<IXbimShell>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimShell First { get; }
        bool IsPolyhedron { get; }
        IXbimGeometryObjectSet Cut(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Cut(IXbimSolid toCut, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Union(IXbimSolid toCut, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolidSet toCut, double tolerance);
        IXbimGeometryObjectSet Intersection(IXbimSolid toCut, double tolerance);
        void Add(IXbimGeometryObject shape);
        /// <summary>
        /// Unions all elements in the  set and updates the set to the result
        /// </summary>
        /// <param name="tolerance"></param>
        void Union(double tolerance);
    }
}
