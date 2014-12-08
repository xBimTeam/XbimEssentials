#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcVirtualElement.cs
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
    ///   A special element used to provide imaginary boundaries, such as between two adjacent, but not separated, spaces.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A special element used to provide imaginary boundaries, such as between two adjacent, but not separated, spaces. Virtual elements are usually not displayed and does not have quantities and other measures. Therefore IfcVirtualElement does not have material information and quantities attached.
    ///   NOTE The main purpose of IfcVirtualElement is the provision of a virtual space boundary. The IfcVirtualElement provides the 2D curve or 3D surface representation of the virtual space connection and is referenced by two instances of IfcRelSpaceBoundary, each pointing to one of the two adjacent IfcSpaces.
    ///   HISTORY New entity in IFC Release 2x2 Addendum 1. 
    ///   IFC2x2 ADDENDUM CHANGE: The entity IfcVirtualElement has been added. Upward compatibility for file based exchange is guaranteed.
    ///   Geometry Use Definitions
    ///   The geometric representation of any IfcVirtualElement is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations.
    ///   Local Placement
    ///   The local placement for IfcVirtualElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of 'FootPrint' and 'Surface' representation is supported.
    ///   Two-dimensional Representation using foot print representation
    ///   The 2D geometric representation of IfcVirtualElement is defined using the 'FootPring' or 'Surface' representation. The following attribute values should be inserted 
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'FootPrint'. 
    ///   IfcShapeRepresentation.RepresentationType = 'Curve2D' or 'GeometricCurveSet' . 
    ///   The following constraints apply to the 2D curve representation: 
    ///   Curve: IfcPolyline, IfcTrimmedCurve or IfcCompositeCurve 
    ///   Three-dimensional Representation using surface model representation
    ///   The 3D geometric representation of IfcVirtualElement is defined using the 'SurfaceModel' geometry. The following attribute values should be inserted 
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Surface'. 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricSet . 
    ///   The following constraints apply to the 3D surface representation: 
    ///   Surface: IfcSurfaceOfLinearExtrusion 
    ///   Profile: IfcArbitraryOpenProfileDef 
    ///   Extrusion: The extrusion direction shall be vertically, i.e., along the positive Z Axis of the co-ordinate system of the containing spatial structure element.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcVirtualElement : IfcElement
    {
    }
}