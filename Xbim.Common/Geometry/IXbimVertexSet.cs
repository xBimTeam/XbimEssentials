using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimVertexSet : IEnumerable<IXbimVertex>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimVertex First { get; }
    }
}
