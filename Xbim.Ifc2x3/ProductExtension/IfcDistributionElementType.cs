#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionElementType.cs
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
    ///   The element type (IfcDistributionElementType) defines a list of commonly shared property set definitions of an element and an optional set of product representations.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The element type (IfcDistributionElementType) defines a list of commonly shared property set definitions of an element and an optional set of product representations. It is used to define an element specification (i.e. the specific product information, that is common to all occurrences of that product type).
    ///   NOTE  The product representations are defined as representation maps (at the level of the supertype IfcTypeProduct, which gets assigned by an element occurrence instance through the IfcShapeRepresentation.Item[1] being an IfcMappedItem. 
    ///   A distribution element type is used to define the common properties of a certain type of a distribution element that may be applied to many instances of that feature type to assign a specific style. Distribution element types (or the instantiable subtypes) may be exchanged without being already assigned to occurrences.
    ///   The occurrences of the IfcDistributionElementType are represented by instances of IfcDistributionElement (or its subtypes).
    ///   HISTORY  New entity in Release IFC2x Edition 2.
    ///   IFC2x Edition 3 CHANGE  The entity has been made non-abstract
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcDistributionElementType : IfcElementType
    {
    }
}