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
    public class IfcHygroscopicMaterialProperties : IfcMaterialProperties
    {
        private IfcPositiveRatioMeasure? _UpperVaporResistanceFactor;
            
        /// <summary>
        /// The vapor permeability relationship of air/material (typically value > 1), measured in high relative humidity (typically in 95/50 % RH). 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? UpperVaporResistanceFactor
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _UpperVaporResistanceFactor;
            }
            set { this.SetModelValue(this, ref _UpperVaporResistanceFactor, value, v => UpperVaporResistanceFactor = v, "UpperVaporResistanceFactor"); }
        }

        private IfcPositiveRatioMeasure? _LowerVaporResistanceFactor;

        /// <summary>
        /// The vapor permeability relationship of air/material (typically value > 1), measured in low relative humidity (typically in 0/50 % RH). 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? LowerVaporResistanceFactor
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LowerVaporResistanceFactor;
            }
            set { this.SetModelValue(this, ref _LowerVaporResistanceFactor, value, v => LowerVaporResistanceFactor = v, "LowerVaporResistanceFactor"); }
        }

        private IfcIsothermalMoistureCapacityMeasure? _IsothermalMoistureCapacity;

        /// <summary>
        /// Based on water vapor density, usually measured in [m3/ kg]. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcIsothermalMoistureCapacityMeasure? IsothermalMoistureCapacity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _IsothermalMoistureCapacity;
            }
            set { this.SetModelValue(this, ref _IsothermalMoistureCapacity, value, v => IsothermalMoistureCapacity = v, "IsothermalMoistureCapacity"); }
        }

        private IfcVaporPermeabilityMeasure? _VaporPermeability;

        /// <summary>
        ///  	Usually measured in [kg/s m Pa]. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcVaporPermeabilityMeasure? VaporPermeability
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _VaporPermeability;
            }
            set { this.SetModelValue(this, ref _VaporPermeability, value, v => VaporPermeability = v, "VaporPermeability"); }
        }

        private IfcMoistureDiffusivityMeasure? _MoistureDiffusivity;
            
        /// <summary>
        /// Usually measured in [m3/s]. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcMoistureDiffusivityMeasure? MoistureDiffusivity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MoistureDiffusivity;
            }
            set { this.SetModelValue(this, ref _MoistureDiffusivity, value, v => MoistureDiffusivity = v, "MoistureDiffusivity"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _UpperVaporResistanceFactor = value.RealVal;
                    break;
                case 2:
                    _LowerVaporResistanceFactor = value.RealVal;
                    break;
                case 3:
                    _IsothermalMoistureCapacity = value.RealVal;
                    break;
                case 4:
                    _VaporPermeability = value.RealVal;
                    break;
                case 5:
                    _MoistureDiffusivity = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

    }
}
