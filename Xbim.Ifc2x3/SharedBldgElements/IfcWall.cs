#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWall.cs
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
    ///   Vertical construction usually in masonry or in concrete which bounds or subdivides a construction works and fulfils a load bearing or retaining function.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Vertical construction usually in masonry or in concrete which bounds or subdivides a construction works and fulfils a load bearing or retaining function.
    ///   Definition from IAI: The wall represents a vertical construction that bounds or subdivides spaces. Wall are usually vertical, or nearly vertical, planar elements, often designed to bear structural loads. A wall is however not required to be load bearing. The IFC specification provides two entities for wall occurrences:
    ///   IfcWallStandardCase  used for all occurrences of walls, that have a non-changing thickness along the wall path and where the thickness parameter can be fully described by a material layer set. These walls are always represented geometrically by a SweptSolid geometry, if a 3D geometric representation is assigned, 
    ///   IfcWall  used for all other occurrences of wall, particularly for walls with changing thickness along the wall path (e.g. polygonal walls), or walls with a non-rectangular cross sections (e.g. L-shaped retaining walls). 
    ///   HISTORY New entity in IFC Release 1.0, the entity has changed in IFC Release 2x.
    ///   Type Use Definition
    ///   IfcWall (or the subtype IfcWallStandardCase) defines the occuurence of any wall, common information about wall types (or styles) is handled by IfcWallType. The IfcWallType (if present) may establish the common type name, usage (or predefined) type, common material layer set, common set of properties and common shape representations (using IfcRepresentationMap). The IfcWallType is attached using the IfcRelDefinedByType.RelatingType objectified relationship and is accessible by the inverse IsDefinedBy attribute.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcWall are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcWall are part of this IFC release:
    ///   Pset_WallCommon: common property set for all wall occurrences 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcWall are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   NominalLength Total nominal (or average) length of the wall along the wall path. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   NominalWidth Total nominal (or average) width (or thickness) of the wall perpendicular to the wall path. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   NominalHeight Total nominal (or average) height of the wall along the wall path. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   GrossFootprintArea Area of the wall as viewed by a ground floor view, not taking any wall modifications (like recesses) into account. It is also referred to as the foot print of the wall. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetFootprintArea Area of the wall as viewed by a ground floor view, taking all wall modifications (like recesses) into account. It is also referred to as the foot print of the wall. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossSideAreaLeft Area of the wall as viewed by an elevation view of the left side (when viewed along the wall path orientation). It does not take into account any wall modifications (such as openings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetSideAreaLeft Area of the wall as viewed by an elevation view of the left side (when viewed along the wall path orientation). It does take into account all wall modifications (such as openings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossSideAreaRight Area of the wall as viewed by an elevation view of the right side (when viewed along the wall path orientation). It does not take into account any wall modifications (such as openings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetSideAreaRight Area of the wall as viewed by an elevation view of the right side (when viewed along the wall path orientation). It does take into account all wall modifications (such as openings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossVolume Volume of the wall, without taking into account the openings and the connection geometry. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Volume of the wall, after subtracting the openings and after considering the connection geometry. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   Containment Use Definition
    ///   The IfcWall (and the subtype IfcWallStandardCase) as any subtype of IfcBuildingElement, may participate in two different containment relationships. The first (and in most implementation scenarios mandatory) relationship is the hierachical spatial containment, the second (optional) relationship is the aggregation within an element assembly.
    ///   The IfcWall is places within the project spatial hierarchy using the objectified relationship IfcRelContainedInSpatialStructure, refering to it by its inverse attribute SELF\IfcElement.ContainedInStructure. Subtypes of IfcSpatialStructureElement are valid spatial containers, with IfcBuildingStorey being the default container. 
    ///   The IfcWall may be aggregated into an element assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.Decomposes. Any subtype of IfcElement can be an element assembly, with IfcElementAssembly as a special focus subtype. 
    ///   In this case the wall should not be additionally contained in the project spatial hierarchy, i.e. SELF\IfcElement.ContainedInStructure should be NIL. 
    ///   The IfcWall may also be an aggregate i.e. being composed by other elements and acting as an assembly using the objectified relationship IfcRelAggregates, refering to it by its inverse attribute SELF\IfcObjectDefinition.IsDecomposedBy. Components of a wall are described by instances of IfcBuildingElementPart that are aggregated to form a complex wall. 
    ///   In this case, the contained IfcBuildingElementPart's should not be additionally contained in the project spatial hierarchy, i.e. the inverse attribute SELF\IfcElement.ContainedInStructure of IfcBuildingElementPart should be NIL.
    ///   Geometry Use Definitions: 
    ///   The geometric representation of IfcWall is given by the IfcProductDefinitionShape, allowing multiple geometric representation. Included are: 
    ///   Local Placement
    ///   The local placement for IfcWall is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'SweptSolid', 'Clipping', and 'Brep' representations is supported. In addition the general representation types 'SurfaceModel' and 'BoundingBox' are allowed. The geometry use definition for 'BoundingBox', 'SurfaceModel' and 'Brep' is explained at IfcBuildingElement. A more restricted geometry definition is given at the level of the subtype IfcWallStandardCase.
    ///   SweptSolid representation
    ///   The standard geometric representation (for body) of IfcWall is defined using the 'SweptSolid' representation. It is based on the vertical extrusion of a polygonal footprint of the wall body. The IfcShapeRepresentation shall have the following values:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' 
    ///   The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required, 
    ///   Profile: IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded vertically, i.e., in the direction of the z-axis of the co-ordinate system of the referred spatial structure element. It might be further constraint to be in the direction of the global z-axis in implementers agreements. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   Connection Geometry
    ///   The connection between two walls is represented by the IfcRelConnectsPathElements. The use of the parameter of that relationship object is defined at the level of the subtypes of IfcWall and at the IfcRelConnectsPathElements.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWall : IfcBuildingElement
    {
        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        #endregion
    }
}