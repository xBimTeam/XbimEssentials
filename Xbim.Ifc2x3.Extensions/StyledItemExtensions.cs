#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    StyledItemExtensions.cs
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
    public static class StyledItemExtensions
    {
        /// <summary>
        ///   IFC only allows one presentation style assignment
        /// </summary>
        /// <param name = "si"></param>
        /// <returns></returns>
        public static IfcPresentationStyleAssignment GetPresentationStyleAssignment(this IfcStyledItem si)
        {
            return si.Styles.OfType<IfcPresentationStyleAssignment>().FirstOrDefault();
        }
    }
}