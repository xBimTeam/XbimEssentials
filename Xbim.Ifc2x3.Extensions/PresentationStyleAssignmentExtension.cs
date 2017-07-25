#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    PresentationStyleAssignmentExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Linq;
using Xbim.Ifc2x3.PresentationAppearanceResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class PresentationStyleAssignmentExtension
    {
        /// <summary>
        ///   returns the surfacestyle associated with this PresentationStyleAssignment, null if no sufacestyle is defined
        /// </summary>
        /// <param name = "style"></param>
        /// <returns></returns>
        public static IfcSurfaceStyle GetSurfaceStyle(this IfcPresentationStyleAssignment style)
        {
            return style.Styles.OfType<IfcSurfaceStyle>().FirstOrDefault();
        }
    }
}