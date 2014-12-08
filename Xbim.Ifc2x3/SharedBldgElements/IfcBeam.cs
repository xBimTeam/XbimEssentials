#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBeam.cs
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
    ///   Structural member designed to carry loads between or beyond points of support, usually narrow in relation to its length and horizontal or nearly so.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Structural member designed to carry loads between or beyond points of support, usually narrow in relation to its length and horizontal or nearly so.
    ///   Definition from IAI: An IfcBeam is a horizontal, or nearly horizontal, structural member. It represents such a member from an architectural point of view. It is not required to be load bearing.
    ///   NOTE  For any longitudial structural member, not constrained to be predominately horizontal nor vertical, or where this semantic information is irrelevant, the entity IfcMember exists.
    ///   A particular usage type for the IfcBeam can be given (if type information is available) by refering to the type object IfcBeamType, using the IfcRelDefinesByType relationship, or (if only occurrence information is given) by using the ObjectType attribute. Recommended values are 'beam' (the default), 'joist', 'lintel', 't-beam'.
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Type Use Definition
    ///   IfcBeam defines the occuurence of any beam, common information about beam types (or styles) is handled by IfcBeamType. The IfcBeamType (if present) may establish the common type name, usage (or predefined) type, common set of properties and common shape representations (using IfcRepresentationMap). The IfcBeamType is attached using the IfcRelDefinedByType.RelatingType objectified relationship and is accessible by the inverse IsDefinedBy attribute.
    ///   Property Set Use Definition
    ///   The property sets relating to the IfcBeam are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcBeam are part of this IFC release:
    ///   Pset_BeamCommon: common property set for all beam occurrences 
    ///   Material Use Definition
    ///   The material of the IfcBeam is defined by the IfcMaterial or IfcMaterialList and attached by the IfcRelAssociatesMaterial.RelatingMaterial. It is accessible by the inverse HasAssociations relationship. Material information can also be given at the IfcBeamType, defining the common attribute data for all occurrences of the same type. It is then accessible by the inverse IsDefinedBy relationship pointing toIfcBeamType.HasAssociations and via IfcRelAssociatesMaterial.RelatingMaterial to IfcMaterial or IfcMaterialList. If both are given, then the material directly assigned to IfcBeam overrides the material assigned to IfcBeamType.
    ///   Quantity Use Definition
    ///   The quantities relating to the IfcBeam are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following quantities are foreseen, but will be subjected to the local standard of measurement used:
    ///   Name Description Value Type 
    ///   NominalLength Total nominal length of the beam, not taking into account any cut-out's or other processing features. IfcQuantityLength 
    ///   CrossSectionArea Total area of the cross section (or profile) of the beam. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   OuterSurfaceArea Total area of the extruded surfaces of the beam (not taking into account the end cap areas), normally generated as perimeter * length. IfcQuantityArea 
    ///   TotalSurfaceArea Total area of the beam, normally generated as perimeter * length + 2 * cross section area. IfcQuantityArea 
    ///   GrossVolume Total gross volume of the beam, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Total net volume of the beam, taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   GrossWeight Total gross weight of the beam without add-on parts, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   NetWeight Total net weight of the beam without add-on parts, taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   Containment Use Definition
    ///   The IfcBeam, as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second (optional) relationship is the aggregation within an element assembly.
    ///   The IfcBeam is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcBeam may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype. 
    ///   In this case it should not be additionally contained in the project spatial hierarchy, i.e. SELF\IfcElement.ContainedInStructure should be NIL. 
    ///   Geometry Use Definition
    ///   The geometric representation of IfcBeam is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local Placement
    ///   The local placement for IfcBeam is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', 'Brep' and 'MappedRepresentation' representations is supported. In addition the general representation types 'SurfaceModel' and 'BoundingBox' are allowed. The geometry use definition for 'BoundingBox', 'SurfaceModel' and 'Brep' is explained at IfcBuildingElement.
    ///   SweptSolid Representation
    ///   The standard geometric representation of IfcBeam is defined using the 'SweptSolid' representation. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   The following additional constraints apply to the 'SweptSolid' representation: 
    ///   Solid: IfcExtrudedAreaSolid shall be supported 
    ///   Profile: IfcRectangleProfileDef and IfcCircleProfileDef shall be supported 
    ///   Extrusion: The profile shall be extruded horizontally, i.e., parallel to the XY Plane of the coordinate system of the referred spatial structure element. It might be further constrained to be parallel to the global XY Plane in implemention agreements. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   EXAMPLE: standard geometric representation.
    ///   The following interpretation of dimension parameter applies for rectangular beams: 
    ///   IfcRectangleProfileDef.YDim interpreted as beam width 
    ///   IfcRectangleProfileDef.XDim interpreted as beam depth 
    ///   The following interpretation of dimension parameter applies for circular beams: 
    ///   IfcCircleProfileDef Radius interpreted as beam radius. 
    ///  
    ///   Advanced SweptSolid and Clipping Representation
    ///   The advanced geometric representation of IfcBeam is defined using the 'SweptSolid' (enhanced by additional profile types) or 'Clipping' geometry. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid', or 'Clipping' 
    ///   The following constraints apply to the advanced representation: 
    ///   Solid: see standard geometric representation, 
    ///   Profile: IfcRectangleProfileDef, IfcCircleProfileDef, IfcIshapeProfileDef and IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: All extrusion directions shall be supported. 
    ///   Boolean result: The IfcBooleanClippingResult shall be supported, allowing for Boolean differences between the swept solid (here IfcExtrudedAreaSolid) and one or several IfcHalfSpaceSolid (or its subtypes). 
    ///   EXAMPLE advanced geometric representation
    ///   Use of non-perpendicular extrusion to create the IfcExtrudedAreaSolid. 
    ///   Use of IfcBooleanClippingResult between an IfcExtrudedAreaSolid and an IfcHalfSpaceSolid to create a clipped body. 
    ///   MappedRepresentation
    ///   In addition to the standard and advanced geometric representation of IfcBeam that is defined using the SweptSolid or Clipping geometry, also the MappedRepresentation, shall be supported as it allows for reusing the geometry definition of the beam type at all occurrences of the same type. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'MappedRepresentation' 
    ///   The same constraints, as given for the standard SweptSolid and the advanced SweptSolid and Clipping geometric representation, shall apply to the MappedRepresentation of the IfcRepresentationMap.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBeam : IfcBuildingElement
    {
        #region Part 21 Step file Parse routines

        #endregion
    }
}