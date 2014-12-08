#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralConnection.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.StructuralLoadResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcStructuralConnection : IfcStructuralItem
    {
        #region Fields

        private IfcBoundaryCondition _appliedCondition;

        #endregion

        #region Properties

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcBoundaryCondition AppliedCondition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _appliedCondition;
            }
            set
            {
                this.SetModelValue(this, ref _appliedCondition, value, v => AppliedCondition = v,
                                           "AppliedCondition");
            }
        }

        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public IEnumerable<IfcRelConnectsStructuralMember> ConnectsStructuralMembers
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsStructuralMember>(
                        r => r.RelatedStructuralConnection == this);
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _appliedCondition = (IfcBoundaryCondition) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}