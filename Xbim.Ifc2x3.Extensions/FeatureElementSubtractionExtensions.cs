#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    FeatureElementSubtractionExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class FeatureElementSubtractionExtensions
    {
        /// <summary>
        ///   Returns the Element that this opening is formed in, i.e. returns the wall that contains this opening
        /// </summary>
        /// <param name = "elem"></param>
        /// <param name = "model"></param>
        /// <returns></returns>
        public static IfcElement GetFeatureElement(this IfcFeatureElement elem, IModel model)
        {
            IfcRelVoidsElement rel =
                model.Instances.Where<IfcRelVoidsElement>(r => r.RelatedOpeningElement == elem).FirstOrDefault();
            return rel != null ? rel.RelatingBuildingElement : null;
        }
    }
}