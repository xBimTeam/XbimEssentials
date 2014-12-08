#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFeatureElement.cs
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
    ///   Generalization of all existence dependent elements which modify the shape and appearance of the associated master element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Generalization of all existence dependent elements which modify the shape and appearance of the associated master element. The IfcFeatureElement offers the ability to handle shape modifiers as semantic objects within the IFC object model.
    ///   NOTE: The term "feature" has a predefined meaning in a context of "feature-based modeling" and within steel construction work. It is introduced here in a broad sense to cover all existence dependent, but semantically described, modifiers of an element's shape and appearance. It is envisioned that future releases enhance the feature-based capabilities of the IFC model.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    ///   NOTE: The entity is introduced as an upward compatible extension of the IFC2x platform. It is an intermediate abstract supertype without defining its own explicit attributes. 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcFeatureElement is given by the IfcProductDefinitionShape, allowing multiple geometric representation. 
    ///   Local Placement
    ///   The local placement for IfcFeatureElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the master IfcElement (its relevant subtypes), which is associated to the IfcFeatureElement by the appropriate relationship object. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Shape Representation
    ///   The geometry use definitions for the shape representation of the IfcFeatureElement is given at the level of its subtypes.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcFeatureElement : IfcElement
    {
    }
}