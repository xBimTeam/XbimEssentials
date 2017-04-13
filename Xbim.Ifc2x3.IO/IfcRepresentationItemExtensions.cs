using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationAppearanceResource;

namespace Xbim.Ifc2x3.IO
{
    public static class IfcRepresentationItemExtensions
    {
        /// <summary>
        /// Returns the first IfcSurfaceStyle associated with the representation item
        /// </summary>
        /// <param name="repItem"></param>
        /// <returns></returns>
        public static IfcSurfaceStyle SurfaceStyle(this IfcRepresentationItem repItem)
        {
            var styledItem = repItem.Model.Instances.Where<IfcStyledItem>(s => s.Item == repItem).FirstOrDefault();
            if (styledItem != null)
            {
                foreach (var presStyle in styledItem.Styles)
                {
                    if (presStyle != null)
                    {
                        var aSurfaceStyle = presStyle.Styles.OfType<IfcSurfaceStyle>().FirstOrDefault();
                        if (aSurfaceStyle != null) return aSurfaceStyle;
                    }
                    
                }

            }
            return null;
        }

      
    }
}
