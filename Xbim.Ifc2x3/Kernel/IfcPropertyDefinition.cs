#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyDefinition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The IfcPropertyDefinition defines the generalization of all characteristics (i.e. a grouping of individual properties), that may be assigned to objects. 
    ///   Currently, subtypes of IfcPropertyDefinition include property set definitions, and property sets.
    ///   Property definitions define information that is shared among multiple instances of objects. 
    ///   The assignment of the shared information to objects is handled by the IfcRelDefines relationship.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPropertyDefinition : IfcRoot
    {
        #region Inverse Relationships

        /// <summary>
        ///   Inverse: Reference to the relationship IfcRelAssociates and thus to those externally defined concepts, like classifications, documents, or library information, which are associated to the property definition.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelAssociates> HasAssociations
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelAssociates>(r => r.RelatedObjects.Contains(this));
            }
        }

        #endregion
    }
}