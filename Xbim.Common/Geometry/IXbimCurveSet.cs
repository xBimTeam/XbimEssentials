using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimCurveSet : IEnumerable<IXbimCurve>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimCurve First { get; }
    }
}
