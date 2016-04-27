using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimEdgeSet : IEnumerable<IXbimEdge>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimEdge First { get; }
    }
}
