#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRampFlight.cs
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
    ///   Inclined slab segment, normally providing a human circulation link between two landings, floors or slabs at different elevations.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Inclined slab segment, normally providing a human circulation link between two landings, floors or slabs at different elevations. 
    ///   An IfcRampFlight is normally aggregated by a ramp (IfcRamp) through the IfcRelAggregates relationship, the ramp flight is then included in the set of IfcRelAggregates.RelatedObjects.
    ///   A ramp flight normally connects the floor slab of zero to two different storeys (or partial storeys) within a building. The connection relationship between the IfcRampFlight and the IfcSlab is expressed using the IfcRelConnectsElements relationship.
    ///   HISTORY New Entity in IFC Release 2.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcRampFlight are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcRampFlight are part of this IFC release:
    ///   Pset_RampFlightCommon: common property set for all ramp flight occurrences 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcRampFlight is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are:
    ///   Local placement
    ///   The local placement for IfcRampFlight is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the IfcRampFlight, however, is used by an IfcRamp, and this container class defines its own local placement, then the PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the aggregate. 
    ///   Standard Geometric Representation
    ///   The standard geometric representation of IfcRampFlight is defined using the swept area geometry. The following constraints apply to the standard representation: 
    ///   Solid: IfcExtrudedAreaSolid is required, 
    ///   Profile: IfcRectangleProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded in any direction relative to the XY plane of the position coordinate system of the IfcExtrudedAreaSolid. Therefore non-perpendicular sweep operation has to be supported. It might be further constrained to be in the direction of the global z-axis in implementers agreements. 
    ///   Example of geometric representation of IfcRampFlight.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRampFlight : IfcBuildingElement
    {
    }
}