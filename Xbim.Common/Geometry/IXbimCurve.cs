using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A curve in 2D or 3D
    /// </summary>
    public interface IXbimCurve : IXbimGeometryObject
    {
        IEnumerable<XbimPoint3D> Intersections(IXbimCurve intersector, double tolerance);     
        double Length { get; }
        XbimPoint3D Start { get; }
        XbimPoint3D End { get; }
        double GetParameter(XbimPoint3D point, double tolerance);
        XbimPoint3D GetPoint(double parameter);
        bool IsClosed { get; }
        bool Is3D { get; }
    }
}
