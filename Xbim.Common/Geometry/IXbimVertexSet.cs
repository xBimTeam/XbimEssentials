using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimVertexSet : IEnumerable<IXbimVertex>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimVertex First { get; }
    }
}
