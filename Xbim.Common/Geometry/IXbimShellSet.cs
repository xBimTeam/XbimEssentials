using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimShellSet : IEnumerable<IXbimShell>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimShell First { get; }
    }
}
