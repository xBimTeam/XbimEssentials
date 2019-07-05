using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcProjectExtension
    {
        public static IEnumerable<IIfcSpatialStructureElement> GetSpatialStructuralElements(this IIfcProject project)
        {
            return project.IsDecomposedBy.SelectMany(rel => rel.RelatedObjects.OfType<IIfcSpatialStructureElement>());
        }
    }
}
