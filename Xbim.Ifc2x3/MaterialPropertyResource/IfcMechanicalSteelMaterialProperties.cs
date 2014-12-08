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
    /// This is a collection of mechanical properties related to steel (or other metallic and isotropic) materials. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcMechanicalSteelMaterialProperties : IfcMechanicalMaterialProperties 
    {
        #region Fields

        private IfcPressureMeasure? _yieldStress;
        private IfcPressureMeasure? _ultimateStress;
        private IfcPositiveRatioMeasure? _ultimateStrain;
        private IfcModulusOfElasticityMeasure? _hardeningModule;
        private IfcPressureMeasure? _proportionalStress;
        private IfcPositiveRatioMeasure? _plasticStrain;
        private XbimSet<IfcRelaxation> _relaxations;

        #endregion

        #region Properties

        /// <summary>
        /// A measure of the yield stress (or characteristic 0.2 percent proof stress) of the material. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPressureMeasure? YieldStress
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _yieldStress;
            }
            set { this.SetModelValue(this, ref _yieldStress, value, v => YieldStress = v, "YieldStress"); }
        }

        /// <summary>
        /// A measure of the ultimate stress of the material. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPressureMeasure? UltimateStress
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _ultimateStress;
            }
            set { this.SetModelValue(this, ref _ultimateStress, value, v => UltimateStress = v, "UltimateStress"); }
        }

        /// <summary>
        /// A measure of the (engineering) strain at the state of ultimate stress of the material. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? UltimateStrain
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _ultimateStrain;
            }
            set { this.SetModelValue(this, ref _ultimateStrain, value, v => UltimateStrain = v, "UltimateStrain"); }
        }

        /// <summary>
        /// A measure of the hardening module of the material (slope of stress versus strain curve after yield range). 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcModulusOfElasticityMeasure? HardeningModule
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _hardeningModule;
            }
            set { this.SetModelValue(this, ref _hardeningModule, value, v => HardeningModule = v, "HardeningModule"); }
        }

        /// <summary>
        /// A measure of the proportional stress of the material. It describes the stress before the first plastic 
        /// deformation occurs and is commonly measured at a deformation of 0.01%. 
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcPressureMeasure? ProportionalStress
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _proportionalStress;
            }
            set { this.SetModelValue(this, ref _proportionalStress, value, v => ProportionalStress = v, "ProportionalStress"); }
        }

        /// <summary>
        /// A measure of the permanent displacement, as in slip or twinning, which remains after the stress has been removed. 
        /// Currently applied to a strain of 0.2% proportional stress of the material. 
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure? PlasticStrain
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _plasticStrain;
            }
            set { this.SetModelValue(this, ref _plasticStrain, value, v => PlasticStrain = v, "PlasticStrain"); }
        }

        /// <summary>
        /// Measures of decrease in stress over long time intervals resulting from plastic flow. 
        /// Different relaxation values for different initial stress levels for a material may be given.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public XbimSet<IfcRelaxation> Relaxations
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _relaxations;
            }
            set { this.SetModelValue(this, ref _relaxations, value, v => Relaxations = v, "Relaxations"); }
        }


        #endregion

        #region Part 21 Step file Parse routines

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
                    _yieldStress = value.RealVal;
                    break;
                case 7:
                    _ultimateStress = value.RealVal;
                    break;
                case 8:
                    _ultimateStrain = value.RealVal;
                    break;
                case 9:
                    _hardeningModule = value.RealVal;
                    break;
                case 10:
                    _proportionalStress = value.RealVal;
                    break;
                case 11:
                    _plasticStrain = value.RealVal;
                    break;
                case 12:
                    if (_relaxations == null) _relaxations = new XbimSet<IfcRelaxation>(this);
                    _relaxations.Add((IfcRelaxation)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(_yieldStress.HasValue && _yieldStress >= 0))
            {
                baseErr = "WR31 IfcMechanicalSteelMaterialProperties : Yield stress, if given, shall be non-negative.";
            }

            if (!(_ultimateStress.HasValue && _ultimateStress >= 0))
            {
                baseErr = "WR32 IfcMechanicalSteelMaterialProperties : Ultimate stress, if given, shall be non-negative.";
            }

            if (!(_hardeningModule.HasValue && _hardeningModule >= 0))
            {
                baseErr = "WR33 IfcMechanicalSteelMaterialProperties : Hardening module, if given, shall be non-negative.";
            }

            if (!(_proportionalStress.HasValue && _proportionalStress >= 0))
            {
                baseErr = "WR34 IfcMechanicalSteelMaterialProperties : Proportional stress, if given, shall be non-negative.";
            }
            return baseErr;
        }

        #endregion

    }
}
