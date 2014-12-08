using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.MaterialPropertyResource
{
    /// <summary>
    /// A container class with material thermal properties defined in IFC specification. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcThermalMaterialProperties : IfcMaterialProperties
    {
        private IfcSpecificHeatCapacityMeasure? _SpecificHeatCapacity;

        /// <summary>
        /// Defines the specific heat of the material: heat energy absorbed per temperature unit. Usually measured in [J/kg K]. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcSpecificHeatCapacityMeasure? SpecificHeatCapacity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SpecificHeatCapacity;
            }
            set { this.SetModelValue(this, ref _SpecificHeatCapacity, value, v => SpecificHeatCapacity = v, "SpecificHeatCapacity"); }
        }

        private IfcThermodynamicTemperatureMeasure? _BoilingPoint;

        /// <summary>
        /// The boiling point of the material (fluid). Usually measured in Kelvin. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? BoilingPoint
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BoilingPoint;
            }
            set { this.SetModelValue(this, ref _BoilingPoint, value, v => BoilingPoint = v, "BoilingPoint"); }
        }

        private IfcThermodynamicTemperatureMeasure? _FreezingPoint;

        /// <summary>
        /// The freezing point of the material (fluid). Usually measured in Kelvin. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? FreezingPoint
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _FreezingPoint;
            }
            set { this.SetModelValue(this, ref _FreezingPoint, value, v => FreezingPoint = v, "FreezingPoint"); }
        }

        private IfcThermalConductivityMeasure? _ThermalConductivity;

        /// <summary>
        /// The rate at which thermal energy is transmitted through the material.Usually in [W/m K]. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcThermalConductivityMeasure? ThermalConductivity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ThermalConductivity;
            }
            set { this.SetModelValue(this, ref _ThermalConductivity, value, v => ThermalConductivity = v, "ThermalConductivity"); }
        }

        public override string WhereRule()
        {
            return base.WhereRule();
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _SpecificHeatCapacity = value.RealVal;
                    break;
                case 2:
                    _BoilingPoint = value.RealVal;
                    break;
                case 3:
                    _FreezingPoint = value.RealVal;
                    break;
                case 4:
                    _ThermalConductivity = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
