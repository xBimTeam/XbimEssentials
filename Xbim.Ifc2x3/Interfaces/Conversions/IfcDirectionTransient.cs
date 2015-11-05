using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcDirectionTransient :PersistEntityTransient, Xbim.Ifc4.Interfaces.IIfcDirection
    {
        List<double> ratios = new List<double>(){0,0,1} ;
        
        public IEnumerable<double> DirectionRatios
        {
            get { return ratios; }
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
