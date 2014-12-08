#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralPointReaction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.StructuralLoadResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   Instances of the entity IfcStructuralPointAction are used to define point actions. 
    ///   Structural loads applicable to point actions are IfcStructuralLoadSingleForce (and subtype), and IfcStructuralLoadSingleDisplacement (and subtype). 
    ///   The structural load, defining the point action is given by the attribute AppliedLoad at the supertype IfcStructuralActivitiy. 
    ///   The coordinate system, in which the AppliedLoad is defined is given by the attribute ObjectPlacement at the supertype IfcProduct.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralPointReaction : IfcStructuralReaction
    {
        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(AppliedLoad is IfcStructuralLoadSingleDisplacement || AppliedLoad is IfcStructuralLoadSingleForce))
                baseErr +=
                    "WR61 StructuralPointReaction : A structural point reaction should have as a result either a single force, or a single displacement.\n";
            return baseErr;
        }
    }
}