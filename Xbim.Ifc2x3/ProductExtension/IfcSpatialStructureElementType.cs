#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpatialStructureElementType.cs
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
    ///   The element type (IfcSpatialStructureElementType) defines a list of commonly shared property set definitions of a spatial structure element and an optional set of product representations.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The element type (IfcSpatialStructureElementType) defines a list of commonly shared property set definitions of a spatial structure element and an optional set of product representations. It is used to define an element specification (i.e. the specific element information, that is common to all occurrences of that element type).
    ///   NOTE  The product representations are defined as representation maps (at the level of the supertype IfcTypeProduct, which gets assigned by an element occurrence instance through the IfcShapeRepresentation.Item[1] being an IfcMappedItem.
    ///   A spatial structure element type is used to define the common properties of a certain type of a spatial structure element that may be applied to many instances of that type to assign a specific style. Spatial structure element types (i.e. the instantiable subtypes) may be exchanged without being already assigned to occurrences.
    ///   NOTE  The spatial structure element types are often used to represent catalogues of predefined spatial types for shared attributes, less so for sharing a common representation map.
    ///   The occurrences of subtypes of the abstract IfcSpatialStructureElementType are represented by instances of subtypes of IfcSpatialStructureElement.
    ///   HISTORY  New entity in Release IFC2x Edition 3.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcSpatialStructureElementType : IfcElementType
    {
    }
}