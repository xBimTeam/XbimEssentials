namespace Xbim.Common.Geometry
{
    public interface IXbimMesh : IXbimShell
    {
        /// <summary>
        /// The mesh is a closed manifold shape
        /// </summary>
        bool IsSolid { get; }

        IXbimMesh Cut(IXbimMesh toCut, double tolerance);
        IXbimMesh Union(IXbimMesh toUnion, double tolerance);
        IXbimMesh Intersection(IXbimMesh toIntersect, double tolerance);
        IXbimMesh Section(IXbimMesh mesh, double tolerance);
    }
}
