using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IXbimFaceSet : IEnumerable<IXbimFace>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimFace First { get; }
    }
}
