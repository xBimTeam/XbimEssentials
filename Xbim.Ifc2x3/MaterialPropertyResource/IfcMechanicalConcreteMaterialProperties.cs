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
    /// Mechanical material properties for concrete.
    /// HISTORY: New entity in Release IFC2x Edition 2.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcMechanicalConcreteMaterialProperties : IfcMechanicalMaterialProperties 
    {
        #region Fields 

        private IfcPressureMeasure? _CompressiveStrength;
        private IfcPositiveLengthMeasure? _MaxAggregateSize;
        private IfcText? _AdmixturesDescription;
        private IfcText? _Workability;
        private IfcNormalisedRatioMeasure? _ProtectivePoreRatio;
        private IfcText? _WaterImpermeability;
 
        #endregion

        #region Properties

        /// <summary>
        /// The compressive strength of the concrete.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPressureMeasure? CompressiveStrength
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _CompressiveStrength;
            }
            set { this.SetModelValue(this, ref _CompressiveStrength, value, v => CompressiveStrength = v, "CompressiveStrength"); }
        }

        /// <summary>
        /// The maximum aggregate size of the concrete.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MaxAggregateSize
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _MaxAggregateSize;
            }
            set { this.SetModelValue(this, ref _MaxAggregateSize, value, v => MaxAggregateSize = v, "MaxAggregateSize"); }
        }

        /// <summary>
        /// 	 Description of the admixtures added to the concrete mix.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcText? AdmixturesDescription
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _AdmixturesDescription;
            }
            set { this.SetModelValue(this, ref _AdmixturesDescription, value, v => AdmixturesDescription = v, "AdmixturesDescription"); }
        }

        /// <summary>
        /// 	 Description of the workability of the fresh concrete defined according to local standards.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcText? Workability
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _Workability;
            }
            set { this.SetModelValue(this, ref _Workability, value, v => Workability = v, "Workability"); }
        }

        /// <summary>
        /// The protective pore ratio indicating the frost-resistance of the concrete.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? ProtectivePoreRatio
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _ProtectivePoreRatio;
            }
            set { this.SetModelValue(this, ref _ProtectivePoreRatio, value, v => ProtectivePoreRatio = v, "ProtectivePoreRatio"); }
        }

        /// <summary>
        /// Description of the water impermeability denoting the water repelling properties.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcText? WaterImpermeability
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _WaterImpermeability;
            }
            set { this.SetModelValue(this, ref _WaterImpermeability, value, v => WaterImpermeability = v, "WaterImpermeability"); }
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
                    _CompressiveStrength = value.RealVal;
                    break;
                case 7:
                    _MaxAggregateSize = value.RealVal;
                    break;
                case 8:
                    _AdmixturesDescription = value.StringVal;
                    break;
                case 9:
                    _Workability = value.StringVal;
                    break;
                case 10:
                    _ProtectivePoreRatio = value.RealVal;
                    break;
                case 11:
                    _WaterImpermeability = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}
