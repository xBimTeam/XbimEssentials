#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcColumn.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   Structural member of slender form, usually vertical, that transmits to its base the forces, primarily in compression, that are applied to it.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Structural member of slender form, usually vertical, that transmits to its base the forces, primarily in compression, that are applied to it.
    ///   Definition from IAI: An IfcColumn is a vertical structural member which often is aligned with a structural grid intersection. It represents a vertical, or nearly vertical, structural member from an architectural point of view. It is not required to be load bearing.
    ///   NOTE  For any longitudial structural member, not constrained to be predominately horizontal nor vertical, or where this semantic information is irrelevant, the entity IfcMember exists.
    ///   A particular usage type for the IfcColumn can be given (if type information is available) by refering to the type object IfcColumnType, using the IfcRelDefinesByType relationship, or (if only occurrence information is given) by using the ObjectType attribute. Recommended values are 'column' (the default).
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Type Use Definition
    ///   IfcColumn defines the occuurence of any beam, common information about column types (or styles) is handled by IfcColumnType. The IfcColumnType (if present) may establish the common type name, usage (or predefined) type, common set of properties and common shape representations (using IfcRepresentationMap). The IfcColumnType is attached using the IfcRelDefinedByType.RelatingType objectified relationship and is accessible by the inverse IsDefinedBy attribute.
    ///   Property Set Use Definition
    ///   The property sets relating to the IfcColumn are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcColumn are part of this IFC release:
    ///   Pset_ColumnCommon: common property set for all column occurrences 
    ///   Material Use Definition
    ///   The material of the IfcColumn is defined by the IfcMaterial or IfcMaterialList and attached by the IfcRelAssociatesMaterial.RelatingMaterial. It is accessible by the inverse HasAssociations relationship. Material information can also be given at the IfcColumnType, defining the common attribute data for all occurrences of the same type. It is then accessible by the inverse IsDefinedBy relationship pointing to IfcColumnType.HasAssociations and via IfcRelAssociatesMaterial.RelatingMaterial to IfcMaterial or IfcMaterialList. If both are given, then the material directly assigned to IfcColumn overrides the material assigned to IfcColumnType.
    ///   Quantity Use Definition
    ///   The quantities relating to the IfcColumn are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. It is accessible by the inverse IsDefinedBy relationship. The following quantities are foreseen, but will be subjected to the local standard of measurement used:
    ///   Name Description Value Type 
    ///   NominalLength Total nominal length of the column, not taking into account any cut-out's or other processing features. IfcQuantityLength 
    ///   CrossSectionArea Total area of the cross section (or profile) of the column. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   OuterSurfaceArea Total area of the extruded surfaces of the column (not taking into account the end cap areas), normally generated as perimeter * length. IfcQuantityArea 
    ///   TotalSurfaceArea Total area of the column, normally generated as perimeter * length + 2 * cross section area. IfcQuantityArea 
    ///   GrossVolume Total gross volume of the column, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Total net volume of the column, taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   GrossWeight Total gross weight of the column without add-on parts, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   NetWeight Total net weight of the column without add-on parts, taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   Containment Use Definition
    ///   The IfcColumn, as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second (optional) relationship is the aggregation within an element assembly.
    ///   The IfcColumn, is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcColumn, may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype.
    ///   In this case it should not be additionally contained in the project spatial hierarchy, i.e. SELF\IfcElement.ContainedInStructure should be NIL. 
    ///   Geometry Use Definition
    ///   The geometric representation of IfcColumn is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local Placement
    ///   The local placement for IfcColumn is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', 'Brep' and 'MappedRepresentation' representations is supported. In addition the general representation types 'SurfaceModel' and 'BoundingBox' are allowed. The geometry use definition for 'BoundingBox', 'SurfaceModel' and 'Brep' is explained at IfcBuildingElement.
    ///   Swept Solid Representation
    ///   The standard geometric representation of IfcColumn is defined using the 'SweptSolid' representation. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used: 
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   The following additional constraints apply to the 'SweptSolid' representation: 
    ///   Solid: IfcExtrudedAreaSolid shall be supported 
    ///   Profile: IfcRectangleProfileDef and IfcCircleProfileDef shall be supported 
    ///   Extrusion: The profile shall be extruded vertically, i.e., in the direction of the z-axis of the coordinate system of the referred spatial structure element. It might be further constrained to be in the direction of the global z-axis in implemention agreements. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   EXAMPLE for standard geometric representation. 
    ///   The following interpretation of dimension parameter applies for rectangular columns: 
    ///   IfcRectangleProfileDef.YDim interpreted as column width 
    ///   IfcRectangleProfileDef.XDim interpreted as column height. 
    ///   The following interpretation of dimension parameter applies for round columns:
    ///   IfcCircleProfileDef.Radius interpreted as column radius 
    ///  
    ///   Advanced SweptSolid and Clipping Representation
    ///   The advanced geometric representation of IfcColumn is defined using the 'SweptSolid' (enhanced by additional profile types) or 'Clipping' geometry. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid', or 'Clipping' 
    ///   The following constraints apply to the advanced representation: 
    ///   Solid: see standard geometric representation, 
    ///   Profile: IfcRectangleProfileDef, IfcCircleProfileDef, IfcIshapeProfileDef and IfcArbitraryProfileDef shall be supported. 
    ///   Extrusion: All extrusion directions shall be supported 
    ///   Boolean result: The IfcBooleanClippingResult shall be supported, allowing for Boolean differences between the swept solid (here IfcExtrudedAreaSolid) and one or several IfcHalfSpaceSolid. 
    ///   EXAMPLE for advanced geometric representation.
    ///   Use of a special profile type (here IfcIshapeProfileDef) for the definition of the IfcExtrudedAreaSolid. 
    ///   MappedRepresentation
    ///   In addition to the standard and advanced geometric representation of IfcColumn that is defined using the 'SweptSolid' or 'Clipping' geometry, also the 'MappedRepresentation' shall be supported as it allows for reusing the geometry definition of the column type at all occurrences of the same type. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'MappedRepresentation' 
    ///   The same constraints, as given for the standard 'SweptSolid' and the advanced 'SweptSolid' and 'Clipping' geometric representation, shall apply to the MappedRepresentation of the IfcRepresentationMap.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcColumn : IfcBuildingElement
    {
        #region Part 21 Step file Parse routines

        #endregion
    }
}