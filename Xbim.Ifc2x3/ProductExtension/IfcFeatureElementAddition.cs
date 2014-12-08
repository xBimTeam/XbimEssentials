#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFeatureElementAddition.cs
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
    ///   A specialization of the general feature element, that represents an existence dependent element which modifies the shape and appearance of the associated master element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A specialization of the general feature element, that represents an existence dependent element which modifies the shape and appearance of the associated master element. The IfcFeatureElementAddition offers the ability to handle shape modifiers as semantic objects within the IFC object model that add to the shape of the master element. 
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    ///   NOTE: The entity is introduced as an upward compatible extension of the IFC2x platform. It is an intermediate abstract supertype without defining its own explicit attributes. 
    ///   The IfcFeatureElementAddition is associated to its master element by virtue of the objectified relationship IfcRelProjectsElement. This relationship implies a Boolean 'union' operation between the shape of the master element and the shape of the addition feature.
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcFeatureElementAddition is given by the IfcProductDefinitionShape, allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for IfcFeatureElementAddition is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is refeRenced by all geometric representations. The local placement is always defined in relation to the local placement of the element to which the feature element is added: 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcElement, which is used in the HasAdditionFeature.RelatingElement inverse attribute. 
    ///   Shape Representation
    ///   The geometry use definitions for the shape representation of the IfcFeatureElementAddition is given at the level of its subtypes.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcFeatureElementAddition : IfcFeatureElement
    {
        /// <summary>
        ///   Inverse. Reference to the IfcRelProjectsElement relationship that uses this IfcFeatureElementAddition to create a volume addition at an element. The IfcFeatureElementAddition can only be used to create a single addition at a single element using Boolean addition operation.
        /// </summary>
        public IfcRelProjectsElement ProjectsElements
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}