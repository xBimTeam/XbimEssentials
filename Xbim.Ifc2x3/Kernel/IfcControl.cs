#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcControl.cs
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
    ///   The IfcControl is the abstract generalization of all concepts that control or constrain products or processes in general. It can be seen as a specification, regulation, cost schedule or other requirement applied to a product or process whose requirements and provisions must be fulfilled. 
    ///   Controls are assigned to products, processes, or other objects by using the IfcRelAssignsToControl relationship.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcControl : IfcObject
    {
        /// <summary>
        ///   Reference to the relationship that associates the control to the object(s) being controlled.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set)]
        public IEnumerable<IfcRelAssignsToControl> Controls
        {
            get { return ModelOf.Instances.Where<IfcRelAssignsToControl>(c => c.RelatingControl == this); }
        }
    }
}