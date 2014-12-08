using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimEdgeSet : IEnumerable<IXbimEdge>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimEdge First { get; }
    }
}
