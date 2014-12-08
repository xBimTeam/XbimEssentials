using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.MaterialPropertyResource
{
    /// <summary>
    /// This is a collection of mechanical material properties normally used for structural analysis purpose.
    /// It contains all properties which are independent of the actual material type.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcMechanicalMaterialProperties : IfcMaterialProperties
    {
        #region Fields 

        private IfcDynamicViscosityMeasure? _dynamicViscosity;
        private IfcModulusOfElasticityMeasure? _youngModulus;
        private IfcModulusOfElasticityMeasure? _shearModulus;
        private IfcPositiveRatioMeasure? _poissonRatio;
        private IfcThermalExpansionCoefficientMeasure? _thermalExpansionCoefficient;
 
        #endregion

        #region Properties

        /// <summary>
        /// A measure of the viscous resistance of the material. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcDynamicViscosityMeasure? DynamicViscosity
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _dynamicViscosity;
            }
            set { this.SetModelValue(this, ref _dynamicViscosity, value, v => DynamicViscosity = v, "DynamicViscosity"); }
        }

        /// <summary>
        /// A measure of the Young’s modulus of elasticity of the material. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcModulusOfElasticityMeasure? YoungModulus
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _youngModulus;
            }
            set { this.SetModelValue(this, ref _youngModulus, value, v => YoungModulus = v, "YoungModulus"); }
        }

        /// <summary>
        /// A measure of the shear modulus of elasticity of the material. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcModulusOfElasticityMeasure? ShearModulus
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _shearModulus;
            }
            set { this.SetModelValue(this, ref _shearModulus, value, v => ShearModulus = v, "ShearModulus"); }
        }

        /// <summary>
        /// A measure of the lateral deformations in the elastic range. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? PoissonRatio
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _poissonRatio;
            }
            set { this.SetModelValue(this, ref _poissonRatio, value, v => PoissonRatio = v, "PoissonRatio"); }
        }

        /// <summary>
        /// A measure of the expansion coefficient for warming up the material about one Kelvin.  
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcThermalExpansionCoefficientMeasure? ThermalExpansionCoefficient
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _thermalExpansionCoefficient;
            }
            set { this.SetModelValue(this, ref _thermalExpansionCoefficient, value, v => ThermalExpansionCoefficient = v, "ThermalExpansionCoefficient"); }
        }

        #endregion


        #region Part 21 Step file Parse routines

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _dynamicViscosity = value.RealVal;
                    break;
                case 2:
                    _youngModulus = value.RealVal;
                    break;
                case 3:
                    _shearModulus = value.RealVal;
                    break;
                case 4:
                    _poissonRatio = value.RealVal;
                    break;
                case 5:
                    _thermalExpansionCoefficient = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        //TODO: UNIQUE	UR11 : Material in IfcMechanicalMaterialProperties see http://www.steptools.com/support/stdev_docs/express/ifc2x3/html/t_ifcme-08.html ???

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(_youngModulus.HasValue && _youngModulus >= 0))
            {
                baseErr = "WR21 IfcMechanicalMaterialProperties : Young modulus of a material may not be negative.";
            }

            if (!(_shearModulus.HasValue && _shearModulus >= 0))
            {
                baseErr = "WR22 IfcMechanicalMaterialProperties : Shear modulus of a material may not be negative.";
            }
            return baseErr;
        }

        #endregion
    }
}
