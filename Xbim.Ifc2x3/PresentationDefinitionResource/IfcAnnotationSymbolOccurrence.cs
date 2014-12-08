#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotationSymbolOccurrence.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    public class IfcAnnotationSymbolOccurrence : IfcAnnotationOccurrence, IfcDraughtingCalloutElement
    {
        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Item != null && !(Item is IfcDefinedSymbol))
                baseErr +=
                    "WR31 AnnotationSymbolOccurrence : The Item that is styled by an IfcAnnotationCurveOccurrence relation shall be (if provided) a subtype of IfcDefinedSymbol. ";
            return baseErr;
        }
    }
}