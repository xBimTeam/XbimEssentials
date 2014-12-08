#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProcess.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

using System.Collections.Generic;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcProcess : IfcObject
    {
        #region Inverse Relationships
        /// <summary>
        ///   Inverse. Set of Relationships to objects that are operated on by the process.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelAssignsToProcess> OperatesOn
        {
            get
            {
                return
                    this.ModelOf.Instances.Where<IfcRelAssignsToProcess>( r =>  r.RelatingProcess == this);
        }
        }

        //TODO: Check validity of IsSuccessorFrom and IsPredecessorTo

        /// <summary>
        ///  Inverse. Relative placement in time, refers to the previous processes for which this process is successor.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelSequence> IsSuccessorFrom
        {
            get
        {
                return
                    this.ModelOf.Instances.Where<IfcRelSequence>(r => r.RelatedProcess == this);
            }
        }

        /// <summary>
        ///  Inverse. Relative placement in time, refers to the subsequent processes for which this process is predecessor.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelSequence> IsPredecessorTo
        {
            get
        {
                return
                    this.ModelOf.Instances.Where<IfcRelSequence>(r =>  r.RelatingProcess == this);
            }
        }
        #endregion
    }
}