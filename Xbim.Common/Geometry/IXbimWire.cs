#region Directives

using System;
using System.Collections.Generic;
using Xbim.Common.Geometry;

#endregion

namespace XbimGeometry.Interfaces
{
    /// <summary>
    ///     A wire is a connected set of one or more edges
    /// </summary>
    public interface IXbimWire : IXbimGeometryObject, IEquatable<IXbimWire>
    {
      

        /// <summary>
        /// List of connected oriented edges
        /// </summary>
        IXbimEdgeSet Edges { get; }

        /// <summary>
        /// Unique vertices in the shape, not in order
        /// </summary>
        IXbimVertexSet Vertices { get; }

        /// <summary>
        /// Points in order of the wire
        /// </summary>
        IEnumerable<XbimPoint3D> Points { get; }


        /// <summary>
        ///  The normal of the loop, calculated using the Newell's normal method
        /// </summary>
        XbimVector3D Normal { get; }

        /// <summary>
        /// The vertices lay on a planar surface within the specified tolerance of the vertices
        /// </summary>
        bool IsPlanar { get; }

        /// <summary>
        /// The wire is a closed loop
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// First point of the wire
        /// </summary>
        XbimPoint3D Start { get; }

        /// <summary>
        /// Last point of the wire
        /// </summary>
        XbimPoint3D End { get; }

        /// <summary>
        /// Length of the wire
        /// </summary>
        double Length { get; }

        /// <summary>
        /// Returns a segment of the wire from start to end position
        /// </summary>
        /// <param name="start">The distanceto trim from the start of the wire</param>
        /// <param name="end">The distance to trim to, from the start of the wire</param>
        /// <param name="tolerance">The distance at which two points are considered to be the same</param>
        /// <returns></returns>
        IXbimWire Trim(double start, double end, double tolerance);
    }
}