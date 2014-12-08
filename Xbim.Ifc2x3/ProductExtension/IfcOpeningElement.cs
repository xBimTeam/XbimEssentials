#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOpeningElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The opening element stands for opening, recess or chase, all reflecting voids.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The opening element stands for opening, recess or chase, all reflecting voids. It represents a void within any element that has physical manifestation. Openings must be handled by all sectors and disciplines in AEC/FM industry, therefore the interoperability for opening elements is provided at this high level. 
    ///   There are two different types of opening elements:
    ///   an opening, where the thickness of the opening is greater or equal to the thickness of the element; 
    ///   a recess or niche, where the thickness of the recess is smaller than the thickness of the element. 
    ///   The inherited attribute ObjectType should be used to capture the differences, 
    ///   the attribute is set to 'Opening' for an opening or 
    ///   the attribute is set to 'Recess' for a recess or niche. 
    ///   If the value for ObjectType is omitted, opening is assumed. 
    ///   An IfcOpeningElement has to be inserted into a building element (all subtypes of IfcBuildingElement) by using the IfcRelVoidsElement relationship. It is also directly linked to the spatial structure of the project (and here normally to the IfcBuildingStorey) by using the IfcRelContainedInSpatialStructure relationship.
    ///   HISTORY New entity in IFC Release 1.0
    ///   IFC2x PLATFORM CHANGE: The intermediate ABSTRACT supertypes IfcFeatureElement and IfcFeatureSubtraction have been added between IfcElement and IfcOpeningElement with upward compatibility. 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcOpeningElement are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcOpeningElement are part of this IFC release:
    ///   Pset_OpeningElementCommon: common property set for all opening occurrences 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcOpeningElement are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   OpeningArea Area of the opening as viewed by an elevation view (for wall openings) or as viewed by a ground floor view (for floor openings). The exact definition and calculation rules depend on the method of measurement used. IfcAreaQuantity  
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcOpeningElement is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for IfcOpeningElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement should point to the local placement of the same element, which is voided by the opening, i.e. referred to by VoidsElement.RelatingBuildingElement. 
    ///   Geometric Representation
    ///   Currently, the use of 'SweptSolid', 'Brep', and 'MappedRepresentation' representation is supported.
    ///   Swept Solid Representation, Horizontal Extrusion
    ///   The standard geometric representation of IfcOpeningElement is defined using the swept area solid geometry. The attribute RepresentationType of IfcShapeRepresentation should have the value 'SweptSolid'. The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required. 
    ///   Profile: IfcRectangleProfileDef, IfcCircleProfileDef and IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded horizontally (i.e. perpendicular to the extrusion direction of the voided element), e.g. for wall openings, or vertically (i.e. in the extrusion direction of the voided element), e.g., for floor openings. 
    ///   Special agreement for defining openings in round building elements, e.g., in round walls. The opening width, in case of a rectangular opening equal with the IfcRectangleProfileDef.XDim, is defined as the straight line distance between two parallel jambs. If the jambs are defined radial (to the center of the arc used to define the round wall) then the opening width is defined to be the outer arc length.
    ///   NOTE: In case of non-parallel jambs, the geometry expression is not an extruded area solid, it has to be represented using other types of representations.
    ///   EXAMPLE for openings
    ///   The following interpretation of dimension parameter applies for rectangular openings: 
    ///   IfcRectangleProfileDef.YDim interpreted as opening width 
    ///   IfcRectangleProfileDef.XDim interpreted as opening height 
    ///   NOTE: Rectangles are now defined centric, the placement location has to be set:
    ///   IfcCartesianPoint(XDim/2,YDim/2) 
    ///  
    ///   EXAMPLE for recesses
    ///   The following interpretation of dimension parameter applies for rectangular recess: 
    ///   IfcRectangleProfileDef.YDim interpreted as recess width 
    ///   IfcRectangleProfileDef.XDim interpreted as recess height 
    ///   IfcExtrudedAreaSolid.Depth is interpreted as recess depth 
    ///   NOTE: Rectangles are now defined centric, the placement location has to be set:
    ///   IfcCartesianPoint(XDim/2,YDim/2) 
    ///   NOTE: The local placement directions for the IfcOpeningRecess are only given as an example, other directions are valid as well.
    ///  
    ///   NOTE  In case of recesses also profiles of type IfcArbitraryProfileDefWithVoid shall be supported as a 'SweptSolid' representation.
    ///   Swept Solid Representation, Vertical Extrusion
    ///   The advanced geometric representation of IfcOpeningElement is defined using the swept area solid geometry, however the extrusion direction may be vertical, i.e. in case of a wall opening, the extrusion would be in the direction of the wall height. The attribute RepresentationType of IfcShapeRepresentation should have the value 'SweptSolid'. The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required. 
    ///   Profile: IfcRectangleProfileDef, IfcCircleProfileDef and IfcArbitraryClosedProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded vertically, i.e. for wall openings along the extrusion direction of the voided element. 
    ///   Vertical extrusions shall be used when an opening or recess has a non rectangular foot print geometry that does not change along the height of the opening or recess. 
    ///   Brep Representation
    ///   The general b-rep geometric representation of IfcOpeningElement is defined using the Brep geometry. The Brep representation allows for the representation of complex element shape. It is ensured by assigning the value 'Brep' to the RepresentationType attribute of IfcShapeRepresentation The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcOpeningElement : IfcFeatureElementSubtraction
    {
        #region Fields

        #endregion

        #region Part 21 Step file Parse routines

        #endregion

        /// <summary>
        ///   Inverse. Reference to the Filling Relationship that is used to assign Elements as Fillings for this Opening Element. The Opening Element can be filled with zero-to-many Elements.
        /// </summary>
        protected IEnumerable<IfcRelFillsElement> HasFillings
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelFillsElement>(r => r.RelatingOpeningElement == this);
            }
        }
    }
}