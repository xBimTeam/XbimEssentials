#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStyleModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcStyleModel represents the concept of a particular presentation style defined for a material (or other characteristic) of a product or a product component within a representation context.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcStyleModel represents the concept of a particular presentation style defined for a material (or other characteristic) of a product or a product component within a representation context. This representation context may (but has not to be) a geometric representation context. 
    ///   The IfcStyleModel can be a style representation (presentation style) of a material (via IfcMaterialDefinitionRepresentation), potentially differentiated for different representation contexts (e.g. different material hatching depending on the scale of the target representation context).
    ///   HISTORY  New entity in Release IFC 2x Edition
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcStyleModel : IfcRepresentation
    {
        #region Constructors

        #endregion
    }
}