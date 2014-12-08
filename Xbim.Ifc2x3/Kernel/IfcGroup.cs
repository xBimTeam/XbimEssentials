#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGroup.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;
using System.Linq;
#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   Definition from IAI: The IfcGroup is an generalization of any arbitrary group. A group is a logical collection of objects. 
    ///   It does not have its own position, nor can it hold its own shape representation. 
    ///   Therefore a group is an aggregation under some non-geometrical / topological grouping aspects. 
    ///   NOTE  Use IfcRelDecomposes together with the appropriate subtypes of IfcProduct to define an aggregation of products that may have its own position and shape representation.
    ///   EXAMPLE  An example for a group is the system, since it groups elements under the aspect of their role, regardless of their position in a building. 
    ///   A group can hold any collection of objects (being products, processes, controls, resources, actors or other groups). Thus groups can be nested. An object can be part of zero, one, or many groups.
    ///   Grouping relationships are not required to be hierarchical.
    ///   NOTE  Use IfcRelDecomposes together with the appropriate subtypes of IfcProduct to define an hieraarchical aggregation of products. 
    ///   The group collection is handled by an instance of IfcRelAssignsToGroup, which assigns all group members to the IfcGroup.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcGroup : IfcObject
    {
        /// <summary>
        ///   Contains the relationship that assigns the group members to the group object.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory)]
        public IfcRelAssignsToGroup IsGroupedBy
        {
            get
            {
                IEnumerable<IfcRelAssignsToGroup> grps = ModelOf.Instances.Where<IfcRelAssignsToGroup>(r => r.RelatingGroup == this);
                return grps.FirstOrDefault(); //should onl ever be one
            }
        }
    }
}