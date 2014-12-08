using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimMeshSet : IEnumerable<IXbimMesh>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimMesh First { get; }
    }
}
