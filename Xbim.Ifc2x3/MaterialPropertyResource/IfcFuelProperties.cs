using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.MaterialPropertyResource
{
    [IfcPersistedEntity]
    public class IfcFuelProperties : IfcMaterialProperties
    {
        private IfcThermodynamicTemperatureMeasure? _CombustionTemperature;

        /// <summary>
        /// Combustion temperature of the material when air is at 298 K and 100 kPa. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? CombustionTemperature
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _CombustionTemperature;
            }
            set { this.SetModelValue(this, ref _CombustionTemperature, value, v => CombustionTemperature = v, "CombustionTemperature"); }
        }

        private IfcPositiveRatioMeasure? _CarbonContent;

        /// <summary>
        /// The carbon content in the fuel. This is measured in weight of carbon per unit weight of fuel and is therefore unitless. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? CarbonContent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _CarbonContent;
            }
            set { this.SetModelValue(this, ref _CarbonContent, value, v => CarbonContent = v, "CarbonContent"); }
        }

        private IfcHeatingValueMeasure? _LowerHeatingValue;

        /// <summary>
        /// ower Heating Value is defined as the amount of energy released (MJ/kg) when a fuel is burned completely, and H2O is in vapor form in the combustion products. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcHeatingValueMeasure? LowerHeatingValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LowerHeatingValue;
            }
            set { this.SetModelValue(this, ref _LowerHeatingValue, value, v => LowerHeatingValue = v, "LowerHeatingValue"); }
        }

        private IfcHeatingValueMeasure? _HigherHeatingValue;

        /// <summary>
        /// Higher Heating Value is defined as the amount of energy released (MJ/kg) when a fuel is burned completely, and H2O is in liquid form in the combustion products. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcHeatingValueMeasure? HigherHeatingValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _HigherHeatingValue;
            }
            set { this.SetModelValue(this, ref _HigherHeatingValue, value, v => HigherHeatingValue = v, "HigherHeatingValue"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _CombustionTemperature = value.RealVal;
                    break;
                case 2:
                    _CarbonContent = value.RealVal;
                    break;
                case 3:
                    _LowerHeatingValue = value.RealVal;
                    break;
                case 4:
                    _HigherHeatingValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }

        }
    }
}
