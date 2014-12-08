#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlate.cs
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
    ///   An IfcPlate is a planar and often flat part with constant thickness.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcPlate is a planar and often flat part with constant thickness. A plate can be a structural part carrying loads between or beyond points of support, however it is not required to be load bearing. The location of the plate (being horizontal, vertical or sloped) is not relevant to its definition (in contrary to IfcWall and IfcSlab (as floor slab)). 
    ///   NOTE  Plates are normally made of steel, other metallic material, or by glass panels. However the definition of IfcPlate is material independent and specific material information shall be handled by using IfcAssociatesMaterial to assign a material specification to the IfcPlate. 
    ///   NOTE  Although not necessarily, plates are often add-on parts. This is represented by the IfcRelAggregates decomposition mechanism used to aggregate parts, such as IfcPlate, into a container element, e.g. IfcElementAssembly, or IfcCurtainWall.  
    ///   An instance IfcPlate should preferably get its geometric representation and material assignment through the type definition by IfcPlateType assigned using the IfcRelDefinesByType relationship. This allows identical plates in a construction to be represented by the same instance of IfcPlateType. 
    ///   HISTORY New entity in IFC Release 2x2 
    ///   Containment Use Definitions
    ///   The IfcPlate, as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second relationship is the aggregation within an element assembly.
    ///   The IfcPlate is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcPlate may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype. In this case, no additional relationship to the spatial hierarchy shall be given (i.e. SELF\IfcElement.ContainedInStructure = NIL), the relationship to the spatial container is handled by the element assembly. 
    ///   Geometry use definition
    ///   The geometric representation of IfcPlate is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local Placement
    ///   The local placement for IfcPlate is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the IfcPlate is part of an assembly, the PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the container element, e.g.  IfcElementAssembly, 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', 'Brep' and 'MappedRepresentation' representations is supported. In addition the general representation types 'SurfaceModel' and 'BoundingBox' are allowed. The geometry use definition for 'BoundingBox', 'SurfaceModel' and 'Brep' is explained at IfcBuildingElement.
    ///   SweptSolid Representation
    ///   The standard geometric representation of IfcPlate is defined using the 'SweptSolid' representation. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   Clipping Representation
    ///   The advanced geometric representation of IfcMember is defined using the 'Clipping' geometry. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'Clipping' 
    ///   MappedRepresentation
    ///   In addition to the standard geometric representation of IfcPlate that is defined using the 'SweptSolid' or 'Clipping' geometry, also the 'MappedRepresentation' shall be supported as it allows for reusing the geometry definition of the member type at all occurrences of the same type. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'MappedRepresentation' 
    ///   The same constraints, as given for the standard 'SweptSolid' and the advanced 'SweptSolid' and 'Clipping' geometric representation, shall apply to the MappedRepresentation of the IfcRepresentationMap.
    ///   Use definition for steel members
    ///   When using the IfcPlate for steel members in steel construction applications the following additional conventions apply: 
    ///   Definition by non-geometric properties
    ///   Additional non-geometric properties can be specified through the class IfcPropertySet, which is attached to the inverse attribute IfcObject.IsDefinedBy through the objectified relationship IfcRelDefinesByProperties. This allows for attaching country-specific information to structural members. 
    ///   Decomposition
    ///   An instance of IfcPlate can be part of a decomposition through the IfcRelAggregates relationship - both as sub-ordinate or as a super-ordinate component. 
    ///   If the IfcPlate instance is a sub-ordinate component (i.e. the plate is an add-on part), its local placement shall be relative to that of the super-ordinate instance. 
    ///   As a super-ordinate component, the sub-ordinates of IfcPlate can be other plates or instances of IfcProxy, or IfcDiscreteAccessory (like IfcFastener). 
    ///   Position number
    ///   The position number is assigned through the attribute IfcElement.Tag
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPlate : IfcBuildingElement
    {
    }
}