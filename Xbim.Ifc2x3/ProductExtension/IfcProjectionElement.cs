#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProjectionElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The IfcProjectionElement is a specialization of the general feature element to represent projections applied to building elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcProjectionElement is a specialization of the general feature element to represent projections applied to building elements. It represents a solid attached to any element that has physical manifestation. Projections must be handled by all sectors and disciplines in AEC/FM industry, therefore the interoperability for opening elements is provided at this high level. 
    ///   EXAMPLE: A wall projection such as a pilaster strip is handled by IfcProjectionElement
    ///   An IfcProjectionElement has to be inserted into a building element (all subtypes of IfcBuildingElement) by using the IfcRelProjectsElement relationship. It is also directly linked to the spatial structure of the project (and here normally to the IfcBuildingStorey) by using the IfcRelContainedInSpatialStructure relationship.
    ///   HISTORY New entity in Release IFC2x Edition 2. 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcProjectionElement is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for IfcOpeningRecess is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement should point to the local placement of the same element, which is voided by the opening, i.e. referred to by ProjectsElement.RelatingBuildingElement. 
    ///   Swept Solid Representation
    ///   The geometric representation of IfcProjectionElement is defined using the swept area solid geometry. The attribute RepresentationType of IfcShapeRepresentation should have the value 'SweptSolid'. The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'. The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required. 
    ///   Profile: IfcRectangleProfileDef, IfcCircleProfileDef and IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded horizontally (i.e. perpendicular to the extrusion direction of the modified element), e.g. for wall projections, or vertically (i.e. in the extrusion direction of the projected element), e.g., for floor projections. 
    ///   EXAMPLE
    ///   The following interpretation of dimension parameter applies for rectangular projection: 
    ///   IfcRectangleProfileDef.YDim interpreted as projection width 
    ///   IfcRectangleProfileDef.XDim interpreted as projection height 
    ///   IfcExtrudedAreaSolid.Depth is interpreted as projection depth 
    ///   NOTE: Rectangles are now defined centric, the placement location has to be set:
    ///   IfcCartesianPoint(XDim/2,YDim/2) 
    ///   NOTE: The local placement directions for the IfcProjectionElement are only given as an example, other directions are valid as well.
    ///  
    ///   Brep Representation
    ///   The general b-rep geometric representation of IfcProjectionElement is defined using the Brep geometry. The Brep representation allows for the representation of complex element shape. It is ensured by assigning the value 'Brep' to the RepresentationType attribute of IfcShapeRepresentation The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'. 
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcProjectionElement : IfcFeatureElementAddition
    {
    }
}