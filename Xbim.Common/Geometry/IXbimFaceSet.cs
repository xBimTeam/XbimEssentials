using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IXbimFaceSet : IEnumerable<IXbimFace>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimFace First { get; }
    }
}
