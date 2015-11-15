using System;

namespace Xbim.Common.Geometry
{
    public interface IXbimEdge : IXbimGeometryObject, IEquatable<IXbimEdge>
    {
       
        /// <summary>
        /// Start point (vertex) of the edge
        /// </summary>
        IXbimVertex EdgeStart { get; }
        /// <summary>
        /// End point (vertex) of the edge
        /// </summary>
        IXbimVertex EdgeEnd { get; }
        /// <summary>
        /// The curve defining the form of the edge
        /// </summary>
        IXbimCurve EdgeGeometry { get; }
        /// <summary>
        /// The length of edge, including any curvature
        /// </summary>
        double Length { get; }
        
    }
}
