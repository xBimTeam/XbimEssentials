using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    [IfcPersistedEntity]
    public class IfcServiceLife : IfcControl
    {
        private IfcServiceLifeTypeEnum _ServiceLifeType;

        /// <summary>
        /// Predefined service life types from which that required may be set. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcServiceLifeTypeEnum ServiceLifeType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ServiceLifeType;
            }
            set { this.SetModelValue(this, ref _ServiceLifeType, value, v => ServiceLifeType = v, "ServiceLifeType"); }
        }

        private IfcTimeMeasure _ServiceLifeDuration;

        /// <summary>
        /// The length or duration of a service life. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcTimeMeasure ServiceLifeDuration
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ServiceLifeDuration;
            }
            set { this.SetModelValue(this, ref _ServiceLifeDuration, value, v => ServiceLifeDuration = v, "ServiceLifeDuration"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _ServiceLifeType = (IfcServiceLifeTypeEnum)Enum.Parse(typeof(IfcServiceLifeTypeEnum), value.EnumVal);
                    break;
                case 6:
                    _ServiceLifeDuration = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return base.WhereRule();
        }
    }
}
