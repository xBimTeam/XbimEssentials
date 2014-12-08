#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElectricalBaseProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcElectricalBaseProperties : IfcEnergyProperties
    {
        #region fields

        private IfcElectricCurrentEnum _electricCurrentType;
        private IfcElectricVoltageMeasure _inputVoltage;
        private IfcFrequencyMeasure _inputFrequency;
        private IfcElectricCurrentMeasure _fullLoadCurrent;
        private IfcElectricCurrentMeasure _minimumCircuitCurrent;
        private IfcPowerMeasure _maximumPowerInput;
        private IfcPowerMeasure _ratedPowerInput;
        private IfcInteger _inputPhase;

        #endregion

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcElectricCurrentEnum ElectricCurrentType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _electricCurrentType;
            }
            set
            {
                this.SetModelValue(this, ref _electricCurrentType, value, v => ElectricCurrentType = v,
                                           "ElectricCurrentType");
            }
        }

        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcElectricVoltageMeasure InputVoltage
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _inputVoltage;
            }
            set { this.SetModelValue(this, ref _inputVoltage, value, v => InputVoltage = v, "InputVoltage"); }
        }

        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcFrequencyMeasure InputFrequency
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _inputFrequency;
            }
            set { this.SetModelValue(this, ref _inputFrequency, value, v => InputFrequency = v, "InputFrequency"); }
        }

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcElectricCurrentMeasure FullLoadCurrent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fullLoadCurrent;
            }
            set
            {
                this.SetModelValue(this, ref _fullLoadCurrent, value, v => FullLoadCurrent = v,
                                           "FullLoadCurrent");
            }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcElectricCurrentMeasure MinimumCircuitCurrent
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _minimumCircuitCurrent;
            }
            set
            {
                this.SetModelValue(this, ref _minimumCircuitCurrent, value, v => MinimumCircuitCurrent = v,
                                           "MinimumCircuitCurrent");
            }
        }

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPowerMeasure MaximumPowerInput
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _maximumPowerInput;
            }
            set
            {
                this.SetModelValue(this, ref _maximumPowerInput, value, v => MaximumPowerInput = v,
                                           "MaximumPowerInput");
            }
        }

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcPowerMeasure RatedPowerInput
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _ratedPowerInput;
            }
            set
            {
                this.SetModelValue(this, ref _ratedPowerInput, value, v => RatedPowerInput = v,
                                           "RatedPowerInput");
            }
        }

        [IfcAttribute(14, IfcAttributeState.Mandatory)]
        public IfcInteger InputPhase
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _inputPhase;
            }
            set { this.SetModelValue(this, ref _inputPhase, value, v => InputPhase = v, "InputPhase"); }
        }

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
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _electricCurrentType =
                        (IfcElectricCurrentEnum) Enum.Parse(typeof (IfcElectricCurrentEnum), value.EnumVal, true);
                    break;
                case 7:
                    _inputVoltage = value.RealVal;
                    break;
                case 8:
                    _inputFrequency = value.RealVal;
                    break;
                case 9:
                    _fullLoadCurrent = value.RealVal;
                    break;
                case 10:
                    _minimumCircuitCurrent = value.RealVal;
                    break;
                case 11:
                    _maximumPowerInput = value.RealVal;
                    break;
                case 12:
                    _ratedPowerInput = value.RealVal;
                    break;
                case 13:
                    _inputPhase = value.IntegerVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}