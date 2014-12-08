#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralReaction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   A structural reaction is a structural activity that results from a structural action imposed to a structural item or building element.
    ///   A support is an example for a structural reaction.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcStructuralReaction : IfcStructuralActivity
    {
        #region Properties

        /// <summary>
        ///   Optional reference to instances of IfcStructuralAction which directly depend on this reaction. 
        ///   This reference is only needed if dependencies between structural analysis models must be captured.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcStructuralAction> Causes
        {
            get { return ModelOf.Instances.Where<IfcStructuralAction>(sa => sa.CausedBy == this); }
        }

        #endregion
    }
}