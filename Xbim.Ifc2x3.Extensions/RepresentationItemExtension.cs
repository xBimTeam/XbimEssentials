#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    RepresentationItemExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationAppearanceResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class RepresentationItemExtension
    {
        public static IfcPresentationStyleAssignment GetPresentationStyleAssignment(this IfcRepresentationItem repItem)
        {
            IfcStyledItem item = repItem.StyledByItem.FirstOrDefault();
            if (item != null && item.Styles != null)
                return item.Styles.FirstOrDefault();
            else
                return null;
        }
    }
}