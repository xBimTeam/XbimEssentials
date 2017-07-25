using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.Extensions
{
    public static class RoofExtensions
    {
        public static IEnumerable<IfcSlab> GetSlabs(this IfcRoof roof)
        {
            IEnumerable<IfcRelDecomposes> slabs = roof.IsDecomposedBy;
            return slabs.SelectMany(rel=>rel.RelatedObjects.OfType<IfcSlab>());
        }
    }
}
