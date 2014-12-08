#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMember.cs
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
    ///   An IfcMember is a structural member designed to carry loads between or beyond points of support.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcMember is a structural member designed to carry loads between or beyond points of support. It is not required to be load bearing. The location of the member (being horizontal, vertical or sloped) is not relevant to its definition (in contrary to IfcBeam and IfcColumn).
    ///   The material of the IfcMember is defined by the IfcMaterial and attached by the IfcRelAssociatesMaterial. It is accessible by the inverse HasAssociations relationship.
    ///   A particular usage type for the IfcMember can be given by refering to the type object IfcMemberType, using the IfcRelDefinesByType relationship, or (if only occurrence information is given) by using the ObjectType attribute. Recommended values are 'member' (the default), 'brace', 'collar', 'member', 'post', 'purlin', 'rafter', 'stringer', 'strut'.
    ///   HISTORY  New entity in IFC Release 2x2 Addendum. 
    ///   IFC2x Edition 2 Addendum CHANGE  The entity IfcMember has been added. Upward compatibility for file based exchange is guaranteed.
    ///   Property Set Use Definition
    ///   The property sets relating to the IfcMember are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcMember are part of this IFC release:
    ///   Pset_MemberCommon: common property set for all member occurrences 
    ///   Quantity Use Definition
    ///   The quantities relating to the IfcMember are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following quantities are foreseen, but will be subjected to the local standard of measurement used:
    ///   Name Description Value Type 
    ///   NominalLength Total nominal length of the member, not taking into account any cut-out's or other processing features. IfcQuantityLength 
    ///   CrossSectionArea Total area of the cross section (or profile) of the member. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   OuterSurfaceArea Total area of the extruded surfaces of the member (not taking into account the end cap areas), normally generated as perimeter * length. IfcQuantityArea 
    ///   TotalSurfaceArea Total area of the member, normally generated as perimeter * length + 2 * cross section area. IfcQuantityArea 
    ///   GrossVolume Total gross volume of the member, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Total net volume of the member, taking into account possible processing features (cut-out's, etc.) or openings and recesses. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   GrossWeight Total gross weight of the member without add-on parts, not taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   NetWeight Total net weight of the member without add-on parts, taking into account possible processing features (cut-out's, etc.) or openings and recesses. IfcQuantityWeight 
    ///   Containment Use Definitions
    ///   The IfcMember, as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second (optional) relationship is the aggregation within an element assembly.
    ///   The IfcMember is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcMember may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype. 
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcMember is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local Placement
    ///   The local placement for IfcMember is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the IfcMember is part of an assembly, the PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the container element, e.g.  IfcElementAssembly. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', 'Brep' and 'MappedRepresentation' representations is supported. In addition the general representation types 'SurfaceModel' and 'BoundingBox' are allowed. The geometry use definition for 'BoundingBox', 'SurfaceModel' and 'Brep' is explained at IfcBuildingElement.
    ///   SweptSolid Representation
    ///   The standard geometric representation of IfcMember is defined using the 'SweptSolid' representation. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   The following additional constraints apply to the 'SweptSolid' representation: 
    ///   Solid: IfcExtrudedAreaSolid shall be supported 
    ///   Profile: All subtypes of IfcParameterizedProfileDef and IfcArbitraryClosedProfileDef shall be supported (exclusions need to be agreed upon by implementer agreements). 
    ///   Extrusion: The profile shall be extruded in any direction. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   EXAMPLE: standard geometric representation. 
    ///   The following interpretation of dimension parameter applies for rectangular members: 
    ///   IfcRectangleProfileDef.YDim interpreted as member width 
    ///   IfcRectangleProfileDef.XDim interpreted as member depth 
    ///   The following interpretation of dimension parameter applies for circular members: 
    ///   IfcCircleProfileDef Radius interpreted as member radius. 
    ///  
    ///   Advanced SweptSolid and Clipping Representation
    ///   The advanced geometric representation of IfcMember is defined using the 'SweptSolid' (enhanced by additional profile types) or 'Clipping' geometry. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid', or 'Clipping' 
    ///   The following constraints apply to the advanced representation: 
    ///   Solid: see standard geometric representation, 
    ///   Profile: see standard geometric representation, 
    ///   Extrusion: All extrusion directions shall be supported. 
    ///   Boolean result: The IfcBooleanClippingResult shall be supported, allowing for Boolean differences between the swept solid (here IfcExtrudedAreaSolid) and one or several IfcHalfSpaceSolid (or its subtypes). 
    ///   EXAMPLE advanced geometric representation 
    ///   Use of non-perpendicular extrusion to create the IfcExtrudedAreaSolid. 
    ///   Use of IfcBooleanClippingResult between an IfcExtrudedAreaSolid and an IfcHalfSpaceSolid to create a clipped body. 
    ///   MappedRepresentation
    ///   In addition to the standard and advanced geometric representation of IfcMember that is defined using the 'SweptSolid' or 'Clipping' geometry, also the 'MappedRepresentation' shall be supported as it allows for reusing the geometry definition of the member type at all occurrences of the same type. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'MappedRepresentation' 
    ///   The same constraints, as given for the standard 'SweptSolid' and the advanced 'SweptSolid' and 'Clipping' geometric representation, shall apply to the MappedRepresentation of the IfcRepresentationMap.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcMember : IfcBuildingElement
    {
    }
}