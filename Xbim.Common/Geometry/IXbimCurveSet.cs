using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimCurveSet : IEnumerable<IXbimCurve>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimCurve First { get; }
    }
}
