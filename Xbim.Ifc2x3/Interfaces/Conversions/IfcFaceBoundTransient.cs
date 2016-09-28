using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcFaceBoundTransient : PersistEntityTransient, Ifc4.Interfaces.IIfcFaceBound
    {
        readonly IfcPolyLoopTransient _polyLoop;
        public IfcFaceBoundTransient(IEnumerable<IfcCartesianPoint> points)
        {
            _polyLoop = new IfcPolyLoopTransient(points);
        }

        public Ifc4.Interfaces.IIfcLoop Bound
        {
            get { return _polyLoop; }
            set { throw new NotSupportedException();}
        }

        public IfcBoolean Orientation
        {
            get { return true; }
            set { throw new NotSupportedException(); }
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
