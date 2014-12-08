#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionElement.cs
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
    ///   Generalization of all elements that participate in a distribution system. It is further specialized in the IFC model.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Generalization of all elements that participate in a distribution system. It is further specialized in the IFC model.
    ///   HISTORY New entity in IFC Release 1.5.
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcDistributionElement is given by the IfcProductDefinitionShape, allowing multiple geometric representation. 
    ///   Local Placement
    ///   The local placement for IfcDistributionElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement , which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representation
    ///   The geometric representation of IfcDistributionElement is defined using different geometric representation types for the various subtypes. In general, the following guidelines should be used:
    ///   all fixtures (all non distribution flow elements, i.e. everything which is not a duct, pipe or later cable) should be defined by an b-rep - the new mapped item should be used if appropriate as it allows for reusing the geometry definition of the element type at element instances. In this case the IfcShapeRepresentation.RepresentationType = MappedRepresentation. 
    ///   all "simple" distribution flow elements (general ducts and pipes) are defined by sweep geometry . In this case the IfcShapeRepresentation.RepresentationType = SweptSolid. 
    ///   an additional representation type for all "simple" distribution flow elements (general ducts and pipes) is the ability to have a simple line based representation. In this case the IfcShapeRepresentation.RepresentationType = GeometricSet. 
    ///   distribution flow elements with size changes (e.g. reducer) are defined by sectioned spine representations. In this case the IfcShapeRepresentation.RepresentationType = SectionedSpine. 
    ///   all complex distribution elements (e.g. Y branch or T branch) are defined using b-rep geometry. In this case the IfcShapeRepresentation.RepresentationType = Brep.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcDistributionElement : IfcElement
    {
    }
}