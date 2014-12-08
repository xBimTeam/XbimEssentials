using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedComponentElements
{
    /// <summary>
    /// A feature describing the edge shape of an building element. 
    /// </summary>
    [IfcPersistedEntity]
    public abstract class IfcEdgeFeature : IfcFeatureElementSubtraction
    {
        private IfcPositiveLengthMeasure? _FeatureLength;

        /// <summary>
        /// The length of the feature in orthogonal direction from the feature cross section. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FeatureLength
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _FeatureLength;
            }
            set { this.SetModelValue(this, ref _FeatureLength, value, v => FeatureLength = v, "FeatureLength"); }
        }

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
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _FeatureLength = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
