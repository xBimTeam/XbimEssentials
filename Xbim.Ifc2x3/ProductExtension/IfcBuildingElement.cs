#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingElement.cs
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
    ///   Major functional part of a building, examples are foundation, floor, roof, wall.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Major functional part of a building, examples are foundation, floor, roof, wall.
    ///   Definition from IAI: The building element comprises all elements that are primarily part of the construction of a building, i.e., its structural and space separating system. 
    ///   EXAMPLEs of building elements are walls, beams, or doors, they are all physically existent and tangible things. 
    ///   They are separated from other elements, since they are dealt with in separate AEC processes. The IfcBuildingElement utilizes the following capabilities mainly through inverse referenced to objectified relationships:
    ///   Grouping - being part of a logical group of objects 
    ///   Classification - assigned reference to an external classification 
    ///   Documentation - assigned reference to an external documentation 
    ///   Type - reference to the product type information for the element occurrence 
    ///   Properties - reference to all attached properties, including quantities 
    ///   Cost control - reference to cost elements associated with this building element 
    ///   Work processes - reference to work tasks, in which this building element is used 
    ///   Aggregation - aggregated together with other elements to form an aggregate 
    ///   Connection - connectivity to other elements, including the definition of the joint 
    ///   Ports - information, whether the building element has ports for system connections 
    ///   Realization - information, whether the building element is used to realize a connection 
    ///   Assignment to spatial structure - hierarchical assignment to the right level within the spatial structure 
    ///   Material - assignment of material used by this building element 
    ///   Boundary - provision of space boundaries through this building element 
    ///   Opening - information, whether the building element includes openings 
    ///   Projection - information, whether the building element has projections 
    ///   Filling - information whether the building element is used to fill openings 
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcBuildingElement are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. A detailed specification for individual quantities is introduced at the level of subtypes of IfcBuildingElement.
    ///   Geometry Use Definitions
    ///   The geometric representation of any IfcBuildingElement is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for any IfcBuildingElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. Further constraints are defined at the level of its subtypes.
    ///   Geometric Representations
    ///   Bounding Box Representation
    ///   Any IfcBuildingElement may be represented as a bounding box, which shows the maximum extend of the body within the coordinated system established by the IfcLocalPlacement. The bounding box representation is the simplest geometric representation available. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'BoundingBox' 
    ///   The bounding box representation is given by an IfcShapeRepresentation, which includes a single item, an IfcBoundingBox.
    ///  
    ///   SurfaceModel Representation
    ///   Any IfcBuildingElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple surface models, based on either shell or face based models. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used: 
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SurfaceModel' 
    ///   In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   The surface model representation is given by an IfcShapeRepresentation, which includes a single item, which is either: 
    ///   IfcShellBasedSurfaceModel, or 
    ///   IfcFaceBasedSurfaceModel. 
    ///  
    ///   Brep Representation
    ///   Any IfcBuildingElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple Boundary Representation elements (which are restricted to faceted Brep with or without voids). The Brep representation allows for the representation of complex element shape. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'Brep' 
    ///   In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   TheBrep representation is given by an IfcShapeRepresentation, which includes one or more items, all of type IfcManifoldSolidBrep. 
    ///   MappedRepresentation
    ///   Any IfcBuildingElement (so far no further constraints are defined at the level of its subtypes) may be represented using the MappedRepresentation. This shall be supported as it allows for reusing the geometry definition of a type at all occurrences of the same type. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'MappedRepresentation' 
    ///   The same constraints, as given for the 'SurfaceModel' and the 'Brep' geometric representation, shall apply to the MappedRepresentation of the IfcRepresentationMap.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcBuildingElement : IfcElement
    {
    }
}