using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcPolyLoopTransient : PersistEntityTransient, Xbim.Ifc4.Interfaces.IIfcPolyLoop
    {
        IEnumerable<Xbim.Ifc2x3.GeometryResource.IfcCartesianPoint> _points;
        public IfcPolyLoopTransient(IEnumerable<Xbim.Ifc2x3.GeometryResource.IfcCartesianPoint> points)
        {
            _points = points;
        }
        public IEnumerable<Ifc4.Interfaces.IIfcCartesianPoint> Polygon
        {
            get
            {
                foreach (var point in _points)
                {
                    yield return point;
                }
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
