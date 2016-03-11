using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcDirectionTransient :PersistEntityTransient, Ifc4.Interfaces.IIfcDirection
    {
        readonly List<double> _ratios = new List<double>{0,0,1} ;
        
        public IEnumerable<IfcReal> DirectionRatios
        {
            get { return _ratios.Select(r => (IfcReal)r); }
        }

        public IfcDimensionCount Dim { get { return _ratios.Count(r => !double.IsNaN(r)); } }

        public double X
        {
            get { return _ratios[0]; }
        }
        public double Y {
            get { return _ratios[1]; }
        }
        public double Z {
            get { return _ratios[2]; }
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
