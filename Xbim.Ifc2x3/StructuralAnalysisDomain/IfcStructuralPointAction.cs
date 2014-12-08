#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralPointAction.cs
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
    [IfcPersistedEntityAttribute]
    public class IfcStructuralPointAction : IfcStructuralAction
    {
        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(AppliedLoad is IfcStructuralLoadSingleDisplacement || AppliedLoad is IfcStructuralLoadSingleForce))
                baseErr +=
                    "WR61 StructuralPointReaction : A structural point action should have as a result either a single force, or a single displacement.\n";
            return baseErr;
        }
    }
}