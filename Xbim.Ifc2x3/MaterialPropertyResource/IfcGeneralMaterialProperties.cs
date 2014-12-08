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
    /// A container class with general material properties defined in IFC specification.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcGeneralMaterialProperties : IfcMaterialProperties
    {
        private IfcMolecularWeightMeasure? _MolecularWeight;

        /// <summary>
        /// Molecular weight of material (typically gas), measured in g/mole. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcMolecularWeightMeasure? MolecularWeight
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MolecularWeight;
            }
            set { this.SetModelValue(this, ref _MolecularWeight, value, v => MolecularWeight = v, "MolecularWeight"); }
        }

        private IfcNormalisedRatioMeasure? _Porosity;

        /// <summary>
        /// The void fraction of the total volume occupied by material (Vbr - Vnet)/Vbr [m3/m3]. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? Porosity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Porosity;
            }
            set { this.SetModelValue(this, ref _Porosity, value, v => Porosity = v, "Porosity"); }
        }

        private IfcMassDensityMeasure? _MassDensity;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcMassDensityMeasure? MassDensity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MassDensity;
            }
            set { this.SetModelValue(this, ref _MassDensity, value, v => MassDensity = v, "MassDensity"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _MolecularWeight = value.RealVal;
                    break;
                case 2:
                    _Porosity = value.RealVal;
                    break;
                case 3:
                    _MassDensity = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
