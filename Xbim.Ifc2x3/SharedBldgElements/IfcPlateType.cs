#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlateType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   The element type IfcPlateType defines a list of commonly shared property set definitions of a thin planar element and an optional set of product representations (i.e. the specific product information, that is common to all occurrences of that product type).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The element type IfcPlateType defines a list of commonly shared property set definitions of a thin planar element and an optional set of product representations (i.e. the specific product information, that is common to all occurrences of that product type).
    ///   NOTE  The product representations are defined as representation maps (at the level of the supertype IfcTypeProduct, which gets assigned by an element occurrence instance through the IfcShapeRepresentation.Item[1] being an IfcMappedItem.
    ///   A plate type is used to define the common properties of a certain type of a plate that may be applied to many instances of that type to assign a specific style. Plate types may be exchanged without being already assigned to occurrences. It is used to define a planar, or plate-like parts to be located by one or several instances of IfcPlate. 
    ///   The occurrences of the IfcPlateType are represented by instances of IfcPlate.
    ///   HISTORY New entity in IFC Release 2x2 
    ///   Use definition for steel members
    ///   When using the IfcPlateType as underlying type for steel members in steel construction applications the following additional conventions apply: 
    ///   Material association:
    ///   The IfcPlateType is associated with exactly one instance of IfcMaterial by the IfcRelAssociatesMaterial relationship. This material association assigns a common material to all occurrences (IfcPlate) of the IfcPlateType. If an individual occurrence has an own material assignment (see IfcPlate), then that assignment overrides the material assignment given at the IfcPlateType. 
    ///   Geometric representation:
    ///   The plate type must have a full geometric representation, normally given by IfcExtrudedAreaSolid. Possibly standardized profile names for the plate have no meaning. The IfcPlateType has (at least) one representation map assigned through the RepresentationMaps relation. The representation map has a full geometric representation given by: 
    ///   Solid: Only IfcExtrudedAreaSolid shall be supported. 
    ///   Profile: Only IfcArbitraryClosedProfileDefshall be supported. The profile represents the contour of the plate. 
    ///   Extrusion: The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the position of the IfcExtrudedAreaSolid. Since the profile instance represents the contour of the plate, the extrusion direction corresponds to the plate thickness. 
    ///   Position number:
    ///   The position number is specified in the attribute IfcTypeProduct.Tag. 
    ///   Non geometric profile properties:
    ///   Non geometric profile properties (for instance mechanical properties) are specified through IfcProfileProperties (and its specific subtypes that are related to the cross section). These properties are attached to IfcPlateType by the relationship IfcRelAssociatesProfileProperties. If an individual occurrence has an own profile property assignment (see IfcPlate), then this assignment overrides the profile property assignment given in IfcPlateType. 
    ///   Quantity related properties:
    ///   Quantity related properties,which do not relate to the profile, are specified through IfcElementQuantity (and its specific subtypes). These properties are attached to the IfcPlateType by the relationship IfcRelDefinesByProperties. If an individual occurrence has an own element quantity assignment (see IfcPlate), then this assignment overrides the quantity assignment given in IfcPlateType. The following quantities are foreseen, but will be subjected to the local standard of measurement used: 
    ///   Name Description Value Type 
    ///   CrossSectionArea Total area of the cross section (or profile) of the plate (or its basic surface area). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossVolume Total gross volume of the plate, not taking into account possible processing features (cut-outs, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Total net volume of the plate, taking into account possible processing features (cut-outs, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   GrossWeight Total gross weight of the plate without add-on parts, not taking into account possible processing features (cut-outs, etc.) or openings and recesses. IfcQuantityWeight 
    ///   NetWeight Total net weight of the plate without add-on parts, taking into account possible processing features (cut-outs, etc.) or openings and recesses. IfcQuantityWeight
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPlateType : IfcBuildingElementType
    {
        #region Fields

        private IfcPlateTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(10, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcPlateTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _predefinedType = (IfcPlateTypeEnum) Enum.Parse(typeof (IfcPlateTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}