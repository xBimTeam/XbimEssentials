#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ElementExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public class ElementSort : IComparer<IfcElement>
    {
        public int Compare(IfcElement x, IfcElement y)
        {
            return string.Compare(x.GetType().Name, y.GetType().Name);
        }
    }

    public static class ElementExtensions
    {
        /// <summary>
        ///   Returns the Element that contains the void that this element fills, null if this is not a void filler
        ///   i.e. will return the wall that contains this door
        /// </summary>
        /// <param name = "elem"></param>
        /// <returns></returns>
        public static IfcElement GetFilledElement(this IfcElement elem, IModel model)
        {
            IfcRelFillsElement rel =
                model.Instances.Where<IfcRelFillsElement>(r => r.RelatedBuildingElement == elem).FirstOrDefault();
            return rel != null ? rel.RelatingOpeningElement.GetFeatureElement(model) : null;
        }

        public static IEnumerable<IfcFeatureElementSubtraction> GetFeatureElementSubtractions(this IfcElement elem,
                                                                                              IModel model)
        {
            IEnumerable<IfcRelVoidsElement> subs =
                model.Instances.Where<IfcRelVoidsElement>(r => r.RelatingBuildingElement == elem);
            return subs.Select(rv => rv.RelatedOpeningElement);
        }
    }
}