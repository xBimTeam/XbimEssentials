using Xbim.Common;
using Xbim.Ifc2x3.RepresentationResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ShapeModelListExtension
    {
        public static IfcShapeModel Lookup(this IItemSet<IfcShapeModel> models, string identifier)
        {
            return models.FirstOrDefault(item => 
                item.RepresentationIdentifier != null && 
                string.Compare(item.RepresentationIdentifier, identifier, true) == 0);
        }
    }
}
