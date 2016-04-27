namespace Xbim.Common.Geometry
{
    public enum XbimGeometryType : byte
    {
        Undefined = 0x0,
        /// <summary>
        /// This type can be transformed to XbimRect3D via XbimRect3D.FromArray(geomdata.ShapeData)
        /// </summary>
        BoundingBox = 0x01,
        MultipleBoundingBox = 0x02,
        TriangulatedMesh = 0x03,
        /// <summary>
        /// Regions (clusters of elements in a model) are stored for the project in one database row.
        /// Use XbimRegionCollection.FromArray(ShapeData) for deserialising.
        /// </summary>
        Region = 0x4,
        /// <summary>
        /// For products with no geometry use TransformOnly to store the transform matrix associated with the placement.
        /// </summary>
        TransformOnly = 0x5,
        /// <summary>
        /// 128 bit hash of a geometry
        /// </summary>
        TriangulatedMeshHash = 0x6,
        /// <summary>
        /// The xBIM variant of the PLY format, a set of nominally planar polygons, stored in ascii format
        /// </summary>
        Polyhedron = 0x7,
        /// <summary>
        /// A triangulated Polyhedron mesh
        /// </summary> = 
        TriangulatedPolyhedron = 0x8,
        /// <summary>
        /// The xBIM variant of the PLY format, a set of nominally planar polygons but stored in binary format
        /// </summary>
        PolyhedronBinary = 0x9,
    }
}
