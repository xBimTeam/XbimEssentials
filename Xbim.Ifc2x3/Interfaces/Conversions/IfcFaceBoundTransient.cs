using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcFaceBoundTransient : PersistEntityTransient, Xbim.Ifc4.Interfaces.IIfcFaceBound
    {
        IfcPolyLoopTransient _polyLoop;
        public IfcFaceBoundTransient(IEnumerable<IfcCartesianPoint> points)
        {
            _polyLoop = new IfcPolyLoopTransient(points);
        }

        public Ifc4.Interfaces.IIfcLoop Bound
        {
            get { return _polyLoop; }
        }

        public bool Orientation
        {
            get { return true; }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcPresentationLayerAssignment> LayerAssignment
        {
            get { return Enumerable.Empty<Ifc4.Interfaces.IIfcPresentationLayerAssignment>(); }
        }

        public IEnumerable<Ifc4.Interfaces.IIfcStyledItem> StyledByItem
        {
            get { return Enumerable.Empty<Ifc4.Interfaces.IIfcStyledItem>(); }
        }
    }
}
