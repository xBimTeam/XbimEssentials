#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEnergyProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcEnergyProperties : IfcPropertySetDefinition
    {
        #region fields

        private IfcEnergySequenceEnum? _energySequence;
        private IfcLabel? _userDefinedEnergySequence;

        #endregion

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcEnergySequenceEnum? EnergySequence
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _energySequence;
            }
            set { this.SetModelValue(this, ref _energySequence, value, v => EnergySequence = v, "EnergySequence"); }
        }

        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedEnergySequence
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedEnergySequence;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedEnergySequence, value,
                                           v => UserDefinedEnergySequence = v, "UserDefinedEnergySequence");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _energySequence =
                        (IfcEnergySequenceEnum) Enum.Parse(typeof (IfcEnergySequenceEnum), value.EnumVal, true);
                    break;
                case 5:
                    _userDefinedEnergySequence = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_energySequence.HasValue && _energySequence.Value == IfcEnergySequenceEnum.USERDEFINED &&
                !UserDefinedEnergySequence.HasValue)
                return
                    "WR1 EnergyProperties : This attribute UserDefinedEnergySequence must be defined if the EnergySequence is USERDEFINED.";
            else
                return "";
        }
    }
}