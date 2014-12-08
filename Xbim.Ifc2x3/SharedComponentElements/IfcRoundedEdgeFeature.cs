using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedComponentElements
{
    [IfcPersistedEntity]
    public class IfcRoundedEdgeFeature : IfcEdgeFeature
    {
        private IfcPositiveLengthMeasure? _Radius;

        /// <summary>
        ///  The radius of the feature cross section. 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Radius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Radius;
            }
            set { this.SetModelValue(this, ref _Radius, value, v => Radius = v, "Radius"); }
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
                    _Radius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
