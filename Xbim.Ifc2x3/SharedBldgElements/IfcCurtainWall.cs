#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCurtainWall.cs
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
    ///   Non load bearing wall positioned on the outside of a building and enclosing it.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Non load bearing wall positioned on the outside of a building and enclosing it.
    ///   Definition of IAI: Exterior wall of a building which is an assembly of components, hung from the edge of the floor/roof structure rather than bearing on a floor. Curtain wall is represented as a building element assembly and implemented as a subtype of IfcBuildingElement that uses an IfcRelAggregates relationship.
    ///   HISTORY New Entity in IFC Release 2.0 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcCurtainWall are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcCurtainWall are part of this IFC release:
    ///   Pset_CurtainWallCommon: common property set for all curtain wall occurrences 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcCurtainWall is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Independent geometric representations, as described below, should only be used when the IfcCurtainWall is not defined as an aggregate. If defined as an aggregate, the geometric representation is the sum of the representations of the components within the aggregate. 
    ///   Local placement
    ///   The local placement for IfcCurtainWall is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   If the IfcCurtainWall establishes an aggregate, then all contained elements (defined by the IsDecomposedBy inverse attribute) shall be placed relative to the IfcCurtainWall.ObjectPlacement.
    ///   Geometric Representation 
    ///   Currently, the use of 'BoundingBox', 'SurfaceModel', 'Brep' and 'MappedRepresentation' representations of IfcCurtainWall are supported. The conventions to use these representations are given at the level of the supertype, IfcBuildingElement.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcCurtainWall : IfcBuildingElement
    {
    }
}