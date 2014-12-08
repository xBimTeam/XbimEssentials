#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFeatureElementSubtraction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;

using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   A specialization of the general feature element, that represents an existence dependent elements which modify the shape and appearance of the associated master element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A specialization of the general feature element, that represents an existence dependent elements which modify the shape and appearance of the associated master element. The IfcFeatureElementSubtraction offers the ability to handle shape modifiers as semantic objects within the IFC object model that subtract from the shape of the master element. 
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    ///   NOTE: The entity is introduced as an upward compatible extension of the IFC2x platform. It is an intermediate abstract supertype without defining its own explicit attributes. The existing IfcOpeningElement is subtyped from it.
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcFeatureElementSubtraction is given by the IfcProductDefinitionShape, allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for IfcFeatureElementSubtraction is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. The local placement is always defined in relation to the local placement of the building element from which the feature element substration is substracted: 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcElement, which is used in the VoidsElements.RelatingElement inverse attribute. 
    ///   Shape Representation
    ///   The geometry use definitions for the shape representation of the IfcFeatureElementSubtraction is given at the level of its subtypes.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcFeatureElementSubtraction : IfcFeatureElement
    {
        #region Fields

        #endregion

        #region Inverses

        /// <summary>
        ///   Inverse. Reference to the Voids Relationship that uses this Opening Element to create a void within an Element. 
        ///   The Opening Element can only be used to create a single void within a single Element.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        protected IfcRelVoidsElement VoidsElements
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelVoidsElement>(r => r.RelatedOpeningElement == this).
                        FirstOrDefault();
            }
        }

        #endregion
    }
}