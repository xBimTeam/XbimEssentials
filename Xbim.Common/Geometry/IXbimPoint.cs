using System;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// A 3 Dimensional Point
    /// </summary>
    public interface IXbimPoint : IXbimGeometryObject, IEquatable<IXbimPoint>
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        XbimPoint3D Point { get; }
        double Tolerance { get; }
    }
}
