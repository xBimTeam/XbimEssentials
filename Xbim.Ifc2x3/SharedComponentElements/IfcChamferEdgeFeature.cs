using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedComponentElements
{
    /// <summary>
    /// An edge feature with a chamfered cross section shape. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcChamferEdgeFeature : IfcEdgeFeature
    {
        private IfcPositiveLengthMeasure? _Width;

        /// <summary>
        /// The width of the feature chamfer cross section. 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Width
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Width;
            }
            set { this.SetModelValue(this, ref _Width, value, v => Width = v, "Width"); }
        }

        private IfcPositiveLengthMeasure? _Height;

        /// <summary>
        /// The height of the feature chamfer cross section. 
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Height
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Height;
            }
            set { this.SetModelValue(this, ref _Height, value, v => Height = v, "Height"); }
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
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _Width = value.RealVal;
                    break;
                case 10:
                    _Height = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
