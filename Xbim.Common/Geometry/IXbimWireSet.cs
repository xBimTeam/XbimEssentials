using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimWireSet : IEnumerable<IXbimWire>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimWire First { get; }
    }
}
