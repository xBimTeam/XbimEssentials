using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A wire that is open
    /// </summary>
    public interface IXbimCurve : IXbimGeometryObject
    {
        IEnumerable<XbimPoint3D> Intersections(IXbimCurve intersector);     
        double Length { get; }
        XbimPoint3D Start { get; }
        XbimPoint3D End { get; }
        double GetParameter(XbimPoint3D point);
        XbimPoint3D GetPoint(double parameter);
        bool IsClosed { get; }
    }
}
