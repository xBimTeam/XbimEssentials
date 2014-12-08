using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimWireSet : IEnumerable<IXbimWire>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimWire First { get; }
    }
}
