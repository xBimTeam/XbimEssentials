namespace Xbim.Common.Geometry
{
    /// <summary>
    /// An oriented edge is an edge constructed from another edge and contains a BOOLEAN direction flag to indicate whether or not the orientation of the constructed edge agrees with the orientation of the original edge.
    /// </summary>
    public interface IXbimOrientedEdge : IXbimGeometryObject
    {
        /// <summary>
        /// Edge entity used to construct this oriented edge.
        /// </summary>
        IXbimEdge EdgeElement { get; }

        /// <summary>
        /// If TRUE the topological orientation as used coincides with the orientation from start vertex to end vertex of the edge element. If FALSE otherwise.
        /// </summary>
        bool SameSense { get; }

        /// <summary>
        /// Start point (vertex) of the edge, after considering orientation
        /// </summary>
        IXbimVertex EdgeStart { get; }

        /// <summary>
        /// End point (vertex) of the edge, after considering orientation
        /// </summary>
        IXbimVertex EdgeEnd { get; }

    }
}
