#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStyledRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.PresentationAppearanceResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcStyledRepresentation represents the concept of a styled presentation being a representation of a product or a product component, like material. within a representation context.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcStyledRepresentation represents the concept of a styled presentation being a representation of a product or a product component, like material. within a representation context. This representation context does not need to be (but may be) a geometric representation context.
    ///   NOTE  Current usage of IfcStyledRepresentation includes the assignment of presentation information to an material. The IfcStyledRepresentation then includes presentation styles (IfcAnnotationCurveOccurrence, IfcAnnotationFillAreaOccurrence, IfcAnnotationSurfaceOccurrence) that define that a material should be shown within a particular (eventually view and scale dependent) representation context. All instances of IfcStyledRepresentation are referenced by IfcMaterialDefinitionRepresentation, and assigned to IfcMaterial by IfcMaterialDefinitionRepresentation.RepresentedMaterial. 
    ///   A styled representation has to include one or several styled items or annotation occurrences with the associated style information (curve, symbol, text, fill area, or surface styles). It may also contain the geometric representation items that are styled. 
    ///   HISTORY: New entity in Release IFC 2x Edition 2.
    ///   Formal Propositions:
    ///   WR21   :   Only IfcStyledItem's (or subtypes) are allowed as members in the list of Items, inherited from IfcRepresentation. 
    ///   IFC2x Edition 3 CHANGE  New where rule to ensure the usage for material definition representations, and other non-shape representations
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcStyledRepresentation : IfcStyleModel
    {
        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Items.OfType<IfcStyledItem>().Count() != Items.Count)
                baseErr +=
                    "WR21 StyledRepresentation: Only StyledItem's (or subtypes) are allowed as members in the list of Items, inherited from IfcRepresentation";
            return baseErr;
        }

        #endregion
    }
}