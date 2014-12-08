using System;
using Xbim.Common.Geometry;
namespace XbimGeometry.Interfaces
{
    public interface IXbimVertex : IXbimGeometryObject, IEquatable<IXbimVertex>
    {
        /// <summary>
        /// The geometric point, which defines the position in geometric space of the vertex.
        /// </summary>
        XbimPoint3D VertexGeometry { get; }
    }
}
