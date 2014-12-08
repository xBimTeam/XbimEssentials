#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpaceThermalLoadProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcSpaceThermalLoadProperties : IfcPropertySetDefinition
    {
        #region fields

        private IfcPositiveRatioMeasure _applicableValueRatio;
        private IfcThermalLoadSourceEnum _thermalLoadSource;
        private IfcPropertySourceEnum _propertySource;
        private IfcText _sourceDescription;
        private IfcPowerMeasure _maximumValue;
        private IfcPowerMeasure? _minimumValue;

        private IfcTimeSeries _thermalLoadTimeSeriesValues;
        private IfcLabel _userDefinedThermalLoadSource;
        private IfcLabel _userDefinedPropertySource;
        private IfcThermalLoadTypeEnum _thermalLoadType;

        #endregion

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure ApplicableValueRatio
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _applicableValueRatio;
            }
            set
            {
                this.SetModelValue(this, ref _applicableValueRatio, value, v => ApplicableValueRatio = v,
                                           "ApplicableValueRatio");
            }
        }

        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcThermalLoadSourceEnum ThermalLoadSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thermalLoadSource;
            }
            set
            {
                this.SetModelValue(this, ref _thermalLoadSource, value, v => ThermalLoadSource = v,
                                           "ThermalLoadSource");
            }
        }

        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcPropertySourceEnum PropertySource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _propertySource;
            }
            set { this.SetModelValue(this, ref _propertySource, value, v => PropertySource = v, "PropertySource"); }
        }

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcText SourceDescription
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sourceDescription;
            }
            set
            {
                this.SetModelValue(this, ref _sourceDescription, value, v => SourceDescription = v,
                                           "SourceDescription");
            }
        }

        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcPowerMeasure MaximumValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _maximumValue;
            }
            set { this.SetModelValue(this, ref _maximumValue, value, v => MaximumValue = v, "MaximumValue"); }
        }

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPowerMeasure? MinimumValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _minimumValue;
            }
            set { this.SetModelValue(this, ref _minimumValue, value, v => MinimumValue = v, "MinimumValue"); }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcTimeSeries ThermalLoadTimeSeriesValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thermalLoadTimeSeriesValues;
            }
            set
            {
                this.SetModelValue(this, ref _thermalLoadTimeSeriesValues, value,
                                           v => ThermalLoadTimeSeriesValues = v, "ThermalLoadTimeSeriesValues");
            }
        }

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcLabel UserDefinedThermalLoadSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedThermalLoadSource;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedThermalLoadSource, value,
                                           v => UserDefinedThermalLoadSource = v, "UserDefinedThermalLoadSource");
            }
        }

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcLabel UserDefinedPropertySource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedPropertySource;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedPropertySource, value,
                                           v => UserDefinedPropertySource = v, "UserDefinedPropertySource");
            }
        }

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcThermalLoadTypeEnum ThermalLoadType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _thermalLoadType;
            }
            set
            {
                this.SetModelValue(this, ref _thermalLoadType, value, v => ThermalLoadType = v,
                                           "ThermalLoadType");
            }
        }

        #region ISupportIfcParser Members

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
                    _applicableValueRatio = value.RealVal;
                    break;
                case 5:
                    _thermalLoadSource =
                        (IfcThermalLoadSourceEnum) Enum.Parse(typeof (IfcThermalLoadSourceEnum), value.EnumVal, true);
                    break;
                case 6:
                    _propertySource =
                        (IfcPropertySourceEnum) Enum.Parse(typeof (IfcPropertySourceEnum), value.EnumVal, true);
                    break;
                case 7:
                    _sourceDescription = value.StringVal;
                    break;
                case 8:
                    _maximumValue = value.RealVal;
                    break;
                case 9:
                    _minimumValue = value.RealVal;
                    break;
                case 10:
                    _thermalLoadTimeSeriesValues = (IfcTimeSeries) value.EntityVal;
                    break;
                case 11:
                    _userDefinedThermalLoadSource = value.StringVal;
                    break;
                case 12:
                    _userDefinedPropertySource = value.StringVal;
                    break;
                case 13:
                    _thermalLoadType =
                        (IfcThermalLoadTypeEnum) Enum.Parse(typeof (IfcThermalLoadTypeEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}