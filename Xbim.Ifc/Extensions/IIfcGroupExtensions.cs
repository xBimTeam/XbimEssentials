using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcGroupExtensions
    {
        public static IEnumerable<IIfcObjectDefinition> GetGroupedObjects(this IIfcGroup gr)
        {
            return gr.IsGroupedBy.SelectMany(r => r.RelatedObjects);
        }

        public static IEnumerable<T> GetGroupedObjects<T>(this IIfcGroup gr) where T : IIfcObjectDefinition
        {
            return GetGroupedObjects(gr).OfType<T>();
        }
    }
}
