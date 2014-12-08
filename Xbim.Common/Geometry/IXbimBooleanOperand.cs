namespace XbimGeometry.Interfaces
{
    public interface IXbimBooleanOperand
    {
        IXbimGeometryObject Cut(IXbimGeometryObject toCut, double tolerance);
        IXbimGeometryObject Union(IXbimGeometryObject toUnion, double tolerance);
        IXbimGeometryObject Intersection(IXbimGeometryObject toIntersect, double tolerance);
        IXbimFaceSet Section(IXbimFace toSection, double tolerance);
        
    }
}
