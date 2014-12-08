using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationAppearanceResource;

namespace Xbim.IO
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
            IfcStyledItem styledItem = repItem.ModelOf.Instances.Where<IfcStyledItem>(s => s.Item == repItem).FirstOrDefault();
            if (styledItem != null)
            {
                foreach (var presStyle in styledItem.Styles)
                {
                    if (presStyle != null)
                    {
                        IfcSurfaceStyle aSurfaceStyle = presStyle.Styles.OfType<IfcSurfaceStyle>().FirstOrDefault();
                        if (aSurfaceStyle != null) return aSurfaceStyle;
                    }
                    
                }

            }
            return null;
        }

      
    }
}
