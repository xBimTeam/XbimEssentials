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
    /// Common definition to capture the properties of water typically used within the context of building services and flow distribution systems.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcWaterProperties : IfcMaterialProperties
    {
        private IfcBoolean? _IsPotable;

        /// <summary>
        ///  If TRUE, then the water is considered potable. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcBoolean? IsPotable
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _IsPotable;
            }
            set { this.SetModelValue(this, ref _IsPotable, value, v => IsPotable = v, "IsPotable"); }
        }

        private IfcIonConcentrationMeasure? _Hardness;

        /// <summary>
        /// Water hardness as positive, multivalent ion concentration in the water (usually concentrations of calcium and magnesium ions in terms of calcium carbonate). 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcIonConcentrationMeasure? Hardness
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Hardness;
            }
            set { this.SetModelValue(this, ref _Hardness, value, v => Hardness = v, "Hardness"); }
        }

        private IfcIonConcentrationMeasure? _AlkalinityConcentration;

        /// <summary>
        /// Maximum alkalinity concentration (maximum sum of concentrations of each of the negative ions substances measured as CaCO3). 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcIonConcentrationMeasure? AlkalinityConcentration
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _AlkalinityConcentration;
            }
            set { this.SetModelValue(this, ref _AlkalinityConcentration, value, v => AlkalinityConcentration = v, "AlkalinityConcentration"); }
        }

        private IfcIonConcentrationMeasure? _AcidityConcentration;

        /// <summary>
        /// Maximum CaCO3 equivalent that would neutralize the acid. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcIonConcentrationMeasure? AcidityConcentration
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _AcidityConcentration;
            }
            set { this.SetModelValue(this, ref _AcidityConcentration, value, v => AcidityConcentration = v, "AcidityConcentration"); }
        }

        private IfcNormalisedRatioMeasure? _ImpuritiesContent;

        /// <summary>
        /// Fraction of impurities such as dust to the total amount of water. This is measured in weight of impurities per weight of water and is therefore unitless. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? ImpuritiesContent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ImpuritiesContent;
            }
            set { this.SetModelValue(this, ref _ImpuritiesContent, value, v => ImpuritiesContent = v, "ImpuritiesContent"); }
        }

        private IfcPHMeasure? _PHLevel;

        /// <summary>
        /// Maximum water ph in a range from 0-14. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPHMeasure? PHLevel
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PHLevel;
            }
            set { this.SetModelValue(this, ref _PHLevel, value, v => PHLevel = v, "PHLevel"); }
        }

        private IfcNormalisedRatioMeasure? _DissolvedSolidsContent;

        /// <summary>
        /// Fraction of the dissolved solids to the total amount of water. This is measured in weight of dissolved solids per weight of water and is therefore unitless. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? DissolvedSolidsContent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _DissolvedSolidsContent;
            }
            set { this.SetModelValue(this, ref _DissolvedSolidsContent, value, v => DissolvedSolidsContent = v, "DissolvedSolidsContent"); }
        }

        public override string WhereRule()
        {
            var result =  base.WhereRule();

            if (ModelOf.Instances.Where<IfcWaterProperties>(wp => wp.Material == Material).Count() > 1)
                result += "UR11: Material; \n";

            return result;
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _IsPotable = value.BooleanVal;
                    break;
                case 2:
                    _Hardness = value.RealVal;
                    break;
                case 3:
                    _AlkalinityConcentration = value.RealVal;
                    break;
                case 4:
                    _AcidityConcentration = value.RealVal;
                    break;
                case 5:
                    _ImpuritiesContent = value.RealVal;
                    break;
                case 6:
                    _PHLevel = value.RealVal;
                    break;
                case 7:
                    _DissolvedSolidsContent = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
