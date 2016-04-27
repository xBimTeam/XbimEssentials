using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions.DataProviders;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ShapeModelListExtension
    {
        public static IfcShapeModel Lookup(this ItemSet<IfcShapeModel> models, string identifier)
        {
            return models.FirstOrDefault(item => 
                item.RepresentationIdentifier != null && 
                string.Compare(item.RepresentationIdentifier, identifier, true) == 0);
        }
    }
}
