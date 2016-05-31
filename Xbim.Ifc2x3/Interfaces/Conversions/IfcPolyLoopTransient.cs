using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcPolyLoopTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcPolyLoop
    {
        readonly IEnumerable<IfcCartesianPoint> _points;
        public IfcPolyLoopTransient(IEnumerable<IfcCartesianPoint> points)
        {
            _points = points;
        }
        public IItemSet<Ifc4.Interfaces.IIfcCartesianPoint> Polygon
        {
            get {
                return _points;
            }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcPresentationLayerAssignment> LayerAssignment
        {
            get { return Enumerable.Empty<Ifc4.Interfaces.IIfcPresentationLayerAssignment>(); }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcStyledItem> StyledByItem
        {
            get { return Enumerable.Empty<Ifc4.Interfaces.IIfcStyledItem>();  }
        }
    }
}
