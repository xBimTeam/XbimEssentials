#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFluidFlowProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcFluidFlowProperties : IfcPropertySetDefinition
    {
        #region fields

        private IfcPropertySourceEnum _propertySource;
        private IfcTimeSeries _flowConditionTimeSeries;
        private IfcTimeSeries _velocityTimeSeries;
        private IfcTimeSeries _flowrateTimeSeries;
        private IfcMaterial _fluid;
        private IfcTimeSeries _pressureTimeSeries;
        private IfcLabel _userDefinedPropertySource;
        private IfcThermodynamicTemperatureMeasure _temperatureSingleValue;
        private IfcThermodynamicTemperatureMeasure _wetBulbTemperatureSingleValue;
        private IfcTimeSeries _wetBulbTemperatureTimeSeries;
        private IfcTimeSeries _temperatureTimeSeries;
        private object _flowrateSingleValue;
        private IfcPositiveRatioMeasure _flowConditionSingleValue;
        private IfcLinearVelocityMeasure _velocitySingleValue;
        private IfcPressureMeasure _pressureSingleValue;

        #endregion

        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPropertySourceEnum PropertySource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _propertySource;
            }
            set { this.SetModelValue(this, ref _propertySource, value, v => PropertySource = v, "PropertySource"); }
        }

        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcTimeSeries FlowConditionTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flowConditionTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _flowConditionTimeSeries, value, v => FlowConditionTimeSeries = v,
                                           "FlowConditionTimeSeries");
            }
        }

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcTimeSeries VelocityTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _velocityTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _velocityTimeSeries, value, v => VelocityTimeSeries = v,
                                           "VelocityTimeSeries");
            }
        }

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcTimeSeries FlowrateTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flowrateTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _flowrateTimeSeries, value, v => FlowrateTimeSeries = v,
                                           "FlowrateTimeSeries");
            }
        }

        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcMaterial Fluid
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fluid;
            }
            set { this.SetModelValue(this, ref _fluid, value, v => Fluid = v, "Fluid"); }
        }

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcTimeSeries PressureTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pressureTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _pressureTimeSeries, value, v => PressureTimeSeries = v,
                                           "PressureTimeSeries");
            }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
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

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure TemperatureSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _temperatureSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _temperatureSingleValue, value, v => TemperatureSingleValue = v,
                                           "TemperatureSingleValue");
            }
        }

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure WetBulbTemperatureSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _wetBulbTemperatureSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _wetBulbTemperatureSingleValue, value,
                                           v => WetBulbTemperatureSingleValue = v, "WetBulbTemperatureSingleValue");
            }
        }

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcTimeSeries WetBulbTemperatureTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _wetBulbTemperatureTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _wetBulbTemperatureTimeSeries, value,
                                           v => WetBulbTemperatureTimeSeries = v, "WetBulbTemperatureTimeSeries");
            }
        }

        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcTimeSeries TemperatureTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _temperatureTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _temperatureTimeSeries, value, v => TemperatureTimeSeries = v,
                                           "TemperatureTimeSeries");
            }
        }

        [IfcAttribute(16, IfcAttributeState.Optional)]
        public object FlowrateSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flowrateSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _flowrateSingleValue, value, v => FlowrateSingleValue = v,
                                           "FlowrateSingleValue");
            }
        }

        [IfcAttribute(17, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure FlowConditionSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flowConditionSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _flowConditionSingleValue, value, v => FlowConditionSingleValue = v,
                                           "FlowConditionSingleValue");
            }
        }

        [IfcAttribute(18, IfcAttributeState.Optional)]
        public IfcLinearVelocityMeasure VelocitySingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _velocitySingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _velocitySingleValue, value, v => VelocitySingleValue = v,
                                           "VelocitySingleValue");
            }
        }

        [IfcAttribute(19, IfcAttributeState.Optional)]
        public IfcPressureMeasure PressureSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pressureSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _pressureSingleValue, value, v => PressureSingleValue = v,
                                           "PressureSingleValue");
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
                    _propertySource =
                        (IfcPropertySourceEnum) Enum.Parse(typeof (IfcPropertySourceEnum), value.EnumVal, true);
                    break;
                case 5:
                    _flowConditionTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 6:
                    _velocityTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 7:
                    _flowrateTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 8:
                    _fluid = (IfcMaterial) value.EntityVal;
                    break;
                case 9:
                    _pressureTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 10:
                    _userDefinedPropertySource = value.StringVal;
                    break;
                case 11:
                    _temperatureSingleValue = value.RealVal;
                    break;
                case 12:
                    _wetBulbTemperatureSingleValue = value.RealVal;
                    break;
                case 13:
                    _wetBulbTemperatureTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 14:
                    _temperatureTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 15:
                    _flowrateSingleValue = value.EntityVal;
                    break;
                case 16:
                    _flowConditionSingleValue = value.RealVal;
                    break;
                case 17:
                    _velocitySingleValue = value.RealVal;
                    break;
                case 18:
                    _pressureSingleValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }

        #endregion
    }
}