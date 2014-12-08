#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEquipmentElement.cs
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
    ///   Generalization of all equipment related objects, those objects are characterized as being pre-manufactured and being movable, and which provide some building service related or other servicing function.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Generalization of all equipment related objects, those objects are characterized as being pre-manufactured and being movable, and which provide some building service related or other servicing function. The term fixture is often used as a synonym or similar concept. The IfcEquipmentElement covers the fixture aspect as well.
    ///   HISTORY New entity in IFC Release 2x
    ///   IFC2x2 NOTE: The entity IfcEquipmentElement is deprecated and shall no longer be used.
    ///   NOTE: The concept of equipment, that has a distribution function, such as electrical equipment, is now handled as distribution elements and class definitions are provided by subtypes of IfcDistributionElement for occurrences and IfcDistributionElementType for types. Equipment, that has no distribution function, is now handled within the IfcSharedComponentElements schema.
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcEquipmentElement is given by the IfcProductDefinitionShape, allowing multiple geometric representation. 
    ///   Local Placement
    ///   The local placement for IfcEquipmentElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement , which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   SurfaceModel Representation
    ///   Any IfcEquipmentElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple surface models, based on either shell or face based models. It is ensured by assigning the value 'SurfaceModel' to the RepresentationType attribute of IfcShapeRepresentation. In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   Brep Representation
    ///   Any IfcEquipmentElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple Boundary Representation elements (which are restricted to faceted Brep with or without voids). The Brep representation allows for the representation of complex element shape. It is ensured by assigning the value 'Brep' to the RepresentationType attribute of IfcShapeRepresentation. In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   MappedRepresentation
    ///   The new mapped item, IfcMappedItem, should be used if appropriate as it allows for reusing the geometry definition of the equipment type at occurrences of the same equipement type. In this case the IfcShapeRepresentation.RepresentationType = MappedRepresentation is used.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcEquipmentElement : IfcElement
    {
    }
}