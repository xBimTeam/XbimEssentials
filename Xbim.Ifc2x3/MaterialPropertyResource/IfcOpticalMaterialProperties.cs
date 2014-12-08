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
    public class IfcOpticalMaterialProperties : IfcMaterialProperties
    {
        private IfcPositiveRatioMeasure? _VisibleTransmittance;

        /// <summary>
        /// Transmittance at normal incidence (visible). Defines the fraction of the visible spectrum of solar radiation that passes through per unit area, perpendicular to the surface. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? VisibleTransmittance
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _VisibleTransmittance;
            }
            set { this.SetModelValue(this, ref _VisibleTransmittance, value, v => VisibleTransmittance = v, "VisibleTransmittance"); }
        }

        private IfcPositiveRatioMeasure? _SolarTransmittance;

        /// <summary>
        /// Transmittance at normal incidence (solar). Defines the fraction of solar radiation that passes through per unit area, perpendicular to the surface. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? SolarTransmittance
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SolarTransmittance;
            }
            set { this.SetModelValue(this, ref _SolarTransmittance, value, v => SolarTransmittance = v, "SolarTransmittance"); }
        }

        private IfcPositiveRatioMeasure _ThermalIrTransmittance;

        /// <summary>
        /// Thermal IR transmittance at normal incidence. Defines the fraction of thermal energy that passes through per unit area, perpendicular to the surface. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure ThermalIrTransmittance
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ThermalIrTransmittance;
            }
            set { this.SetModelValue(this, ref _ThermalIrTransmittance, value, v => ThermalIrTransmittance = v, "ThermalIrTransmittance"); }
        }

        private IfcPositiveRatioMeasure _ThermalIrEmissivityBack;

        /// <summary>
        /// Thermal IR emissivity: back side. Defines the fraction of thermal energy emitted per unit area to "blackbody" at the same temperature, through the "back" side of the material. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure ThermalIrEmissivityBack
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ThermalIrEmissivityBack;
            }
            set { this.SetModelValue(this, ref _ThermalIrEmissivityBack, value, v => ThermalIrEmissivityBack = v, "ThermalIrEmissivityBack"); }
        }

        private IfcPositiveRatioMeasure _ThermalIrEmissivityFront;

        /// <summary>
        /// Thermal IR emissivity: front side. Defines the fraction of thermal energy emitted per unit area to "blackbody" at the same temperature, through the "front" side of the material. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure ThermalIrEmissivityFront
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ThermalIrEmissivityFront;
            }
            set { this.SetModelValue(this, ref _ThermalIrEmissivityFront, value, v => ThermalIrEmissivityFront = v, "ThermalIrEmissivityFront"); }
        }

        private IfcPositiveRatioMeasure _VisibleReflectanceBack;

        /// <summary>
        /// Reflectance at normal incidence (visible): back side. Defines the fraction of the solar ray in the visible spectrum that is reflected and not transmitted when the ray passes from one medium into another, at the "back" side of the other material, perpendicular to the surface. Dependent on material and surface characteristics. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure VisibleReflectanceBack
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _VisibleReflectanceBack;
            }
            set { this.SetModelValue(this, ref _VisibleReflectanceBack, value, v => VisibleReflectanceBack = v, "VisibleReflectanceBack"); }
        }

        private IfcPositiveRatioMeasure _VisibleReflectanceFront;

        /// <summary>
        /// Reflectance at normal incidence (visible): front side. Defines the fraction of the solar ray in the visible spectrum that is reflected and not transmitted when the ray passes from one medium into another, at the "front" side of the other material, perpendicular to the surface. Dependent on material and surface characteristics. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure VisibleReflectanceFront
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _VisibleReflectanceFront;
            }
            set { this.SetModelValue(this, ref _VisibleReflectanceFront, value, v => VisibleReflectanceFront = v, "VisibleReflectanceFront"); }
        }

        private IfcPositiveRatioMeasure _SolarReflectanceFront;

        /// <summary>
        /// Reflectance at normal incidence (solar): front side. Defines the fraction of the solar ray that is reflected and not transmitted when the ray passes from one medium into another, at the "front" side of the other material, perpendicular to the surface. Dependent on material and surface characteristics. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure SolarReflectanceFront
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SolarReflectanceFront;
            }
            set { this.SetModelValue(this, ref _SolarReflectanceFront, value, v => SolarReflectanceFront = v, "SolarReflectanceFront"); }
        }

        private IfcPositiveRatioMeasure _SolarReflectanceBack;

        /// <summary>
        /// Reflectance at normal incidence (solar): back side. Defines the fraction of the solar ray that is reflected and not transmitted when the ray passes from one medium into another, at the "back" side of the other material, perpendicular to the surface. Dependent on material and surface characteristics. 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure SolarReflectanceBack
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SolarReflectanceBack;
            }
            set { this.SetModelValue(this, ref _SolarReflectanceBack, value, v => SolarReflectanceBack = v, "SolarReflectanceBack"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _VisibleTransmittance = value.RealVal;
                    break;
                case 2:
                    _SolarTransmittance = value.RealVal;
                    break;
                case 3:
                    _ThermalIrTransmittance = value.RealVal;
                    break;
                case 4:
                    _ThermalIrEmissivityBack = value.RealVal;
                    break;
                case 5:
                    _ThermalIrEmissivityFront = value.RealVal;
                    break;
                case 6:
                    _VisibleReflectanceBack = value.RealVal;
                    break;
                case 7:
                    _VisibleReflectanceFront = value.RealVal;
                    break;
                case 8:
                    _SolarReflectanceFront = value.RealVal;
                    break;
                case 9:
                    _SolarReflectanceBack = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
