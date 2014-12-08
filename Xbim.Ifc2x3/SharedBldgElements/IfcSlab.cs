#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSlab.cs
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
    ///   A slab is a component of the construction that normally encloses a space vertically. The slab may provide the lower support (floor) or upper construction (roof slab) in any space in a building.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A slab is a component of the construction that normally encloses a space vertically. The slab may provide the lower support (floor) or upper construction (roof slab) in any space in a building. It shall be noted, that only the core or constructional part of this construction is considered to be a slap. The upper finish (flooring, roofing) and the lower finish (ceiling, suspended ceiling) are considered to be coverings. A special type of slab is the landing, described as a floor section to which one or more stair flights or ramp flights connect. May or may not be adjacent to a building storey floor. 
    ///   A slab may have openings, such as floor openings. They are defined by an IfcOpeningElement attached to the slab using the inverse relationship HasOpenings pointing to IfcRelVoidsElement.
    ///   A particular usage type for the IfcSlab can be given (if type information is available) by refering to the type object IfcSlabType, using the IfcRelDefinesByType relationship, or (if only occurrence information is given) by using the PredefinedType attribute. Values of the enumeration are 'Floor' (the default), 'Roof', 'Landing', 'Baseslab', 'Notdefined'. If the value 'Userdefined' is chosen, the user defined value needs to be given at the attribute ObjectType. 
    ///   HISTORY: New entity in IFC Release 2.0, it is a merger of the two previous entities IfcFloor, IfcRoofSlab, introduced in IFC Release 1.0 
    ///   Type Use Definition
    ///   IfcSlabdefines the occuurence of any slab, common information about slab types (or styles) is handled by IfcSlabType. The IfcSlabType (if present) may establish the common type name, usage (or predefined) type, common set of properties, common material layer set, and common shape representations (using IfcRepresentationMap). The IfcSlabType is attached using the IfcRelDefinedByType.RelatingType objectified relationship and is accessible by the inverse IsDefinedBy attribute.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcSlab are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcSlab are part of this IFC release:
    ///   Pset_SlabCommon: common property set for all slab occurrences 
    ///   Material Use Definition
    ///   The material of the IfcSlab is defined by IfcMaterialLayerSet or IfcMaterial and attached by the IfcRelAssociatesMaterial.RelatingMaterial. It is accessible by the inverse HasAssociations relationship. Material information can also be given at the IfcSlabType, defining the common attribute data for all occurrences of the same type. It is then accessible by the inverse IsDefinedBy relationship pointing toIfcSlabType.HasAssociations and via IfcRelAssociatesMaterial.RelatingMaterial to IfcMaterial or IfcMaterialList. If both are given, then the material directly assigned to IfcSlab overrides the material assigned to IfcSlabType.
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcSlab are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   NominalWidth Total nominal (or average) width (or thickness) of the slab. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   Perimeter Perimeter measured along the outer boundaries of the slab. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   GrossFootprintArea Total area of the extruded area (or foot print) of the slab. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetFootprintArea Total area of the extruded area (or foot print) of the slab, taking into account possible slab openings. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossVolume Total gross volume of the slab, not taking into account possible openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Total net volume of the slab, taking into account possible openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   GrossWeight Total gross weight of the slab, not taking into account possible openings and recesses or projections. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityWeight 
    ///   NetWeight Total net weight of the slab, taking into account possible openings and recesses or projections. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityWeight 
    ///   Containment Use Definition
    ///   The IfcSlab, as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second (optional) relationship is the aggregation within an element assembly.
    ///   The IfcSlab is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcSlab may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype. 
    ///   In this case it should not be additionally contained in the project spatial hierarchy, i.e. SELF\IfcElement.ContainedInStructure should be NIL. 
    ///   The IfcSlab may also be an aggregate i.e. being composed by other elements and acting as an assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.IsDecomposedBy. Components of a slab are described by instances of subtypes of IfcBuildingElement, with IfcBuildingElementPart as a special focus subtype that are aggregated to form a complex slab. In this case, the contained elements should not be additionally contained in the project spatial hierarchy, i.e. the inverse attribute SELF\IfcElement.ContainedInStructure of IfcBuildingElementPart (or other subtypes of IfcBuildingElement) should be NIL.
    ///   Geometry Use Definitions: 
    ///   The geometric representation of IfcSlab is given by the IfcProductDefinitionShape, allowing multiple geometric representation. Included are: 
    ///   NOTE. If the IfcSlab is of type Landing and is used within an IfcStair or IfcRamp, the special agreements to handle stair and ramp geometry will also affect the geometric representation of the IfcSlab.
    ///   Local Placement
    ///   The local placement for IfcSlab is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the placement of the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the IfcSlab is of type Landing and is used by an IfcStair or IfcRamp, and this container class defines its own local placement, then the PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the aggregate. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', 'Brep' and 'MappedRepresentation' representations is supported. In addition the general representation type 'BoundingBox' is allowed. The geometry use definitions for 'BoundingBox', and 'Brep' are explained at IfcBuildingElement.
    ///   SweptSolid Representation
    ///   The standard geometric representation of IfcSlab is defined using the swept solid representation. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used: 
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required, 
    ///   Profile: IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded vertically, i.e., in the direction of the z-axis of the co-ordinate system of the referred spatial structure element. It might be further constrained to be in the direction of the global z-axis in implementers agreements. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   EXAMPLE for standard geometric representation. 
    ///   NOTE The following interpretation of dimension parameter applies for polygonal slabs (in ground floor view): 
    ///   IfcArbitraryClosedProfileDef .OuterCurve: closed bounded curve interpreted as area (or foot print) of the slab. 
    ///  
    ///   Clipping representation
    ///   The advanced geometric representation of IfcSlab is defined using the swept area geometry with additional clippings applied. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used: 
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'Clipping' 
    ///   The following constraints apply to the advanced representation: 
    ///   Solid: see standard geometric representation, 
    ///   Profile: see standard geometric representation, 
    ///   Extrusion: All extrusion directions shall be supported. 
    ///   Boolean result: The IfcBooleanClippingResult shall be supported, allowing for Boolean differences between the swept solid (here IfcExtrudedAreaSolid) and one or several IfcHalfSpaceSolid. 
    ///   EXAMPLE for advanced geometric representation. 
    ///   Definition of a roof slab using advanced geometric representation. The profile is extruded non-perpendicular and the slab body
    ///   Formal Propositions:
    ///   WR2   :   The attribute UserDefinedType shall be given, if the predefined type is set to USERDEFINED.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcSlab : IfcBuildingElement
    {
        #region Fields

        private IfcSlabTypeEnum? _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Predefined generic types for a slab that are specified in an enumeration. There may be a property set given for the predefined types.
        /// </summary>
        /// <remarks>
        ///   NOTE: The use of the predefined type directly at the occurrence object level of IfcSlab is only permitted, if no type object IfcSlabType is assigned.
        /// </remarks>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcSlabTypeEnum? PredefinedType
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
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _predefinedType = (IfcSlabTypeEnum?) Enum.Parse(typeof (IfcSlabTypeEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}