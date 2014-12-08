#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcResource.cs
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
    ///   The IfcResource contains the information needed to represent the costs, schedule, and other impacts from the use of a thing in a process.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcResource contains the information needed to represent the costs, schedule, and other impacts from the use of a thing in a process. It is not intended to use IfcResource to model the general properties of the things themselves, while an optional linkage from IfcResource to the things to be used can be specified (i.e. the relationship from subtypes of IfcResource to IfcProduct through the IfcRelAssignsToResource relationship). 
    ///   There are two basic intended use of IfcResource. First, if the attributes of the thing are not needed for the purpose of the use of IfcResource, or the types of things are not explicitly modeled in IFC yet, then the linkage between the resource and the thing doesn’t have to be instantiated in the system. That is, the attributes of IfcResource (or its subtypes) alone are sufficient to represent the use of the thing as a resource for the purpose of the project. 
    ///   EXAMPLE: construction equipment such as earth-moving vehicles or tools are not currently modeled within the IFC. For the purpose of estimating and scheduling, these can be represented using subtypes of IfcResource alone.
    ///   Second, if the attributes of the thing are needed for the use of IfcResource objects, and they are modeled explicitly as objects (e.g. classes or properties), then the IfcResource instances can be linked to the instances of the type of the things being referenced. Things that might be used as resources and that are already modeled in the IFC include physical products, people and organizations, and materials. The relationship object IfcRelAssignsToResource is provided for this approach.
    ///   The inherited attribute ObjectType is used as a textual code that identifies the resource type. 
    ///   HISTORY New entity in IFC Release 1.0 
    ///   IFC2x PLATFORM CHANGE: The attributes BaseUnit and ResourceConsumption have been removed from the abstract entity, they are reintroduced at a lower level in the hierarchy.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcResource : IfcObject
    {
        /// <summary>
        ///   Reference to the IfcRelAssignsToResource relationship and thus pointing to those objects, which are used as resources.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set)]
        public IEnumerable<IfcRelAssignsToResource> ResourceOf
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelAssignsToResource>(c => c.RelatingResource == this);
            }
        }
    }
}