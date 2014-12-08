using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    /// <summary>
    /// A mixed collection of geometry objects
    /// </summary>
    public interface IXbimGeometryObjectSet : IEnumerable<IXbimGeometryObject>, IXbimGeometryObject
    {
        int Count { get; }
        IXbimGeometryObject First { get; }
    }

}
