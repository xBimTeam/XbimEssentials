using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimMeshSet : IEnumerable<IXbimMesh>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimMesh First { get; }
    }
}
