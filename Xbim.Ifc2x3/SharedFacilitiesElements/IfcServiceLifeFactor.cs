using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    /// <summary>
    /// An IfcServiceLifeFactor captures the various factors that impact upon the expected service life of an artefact.
    /// 
    ///     HISTORY: New entity in IFC 2x2
    /// 
    /// Use Definitions
    /// 
    /// Note that each instance of IfcServiceLifeFactor may have a name that describes the form of impact that the factor has on the service life. Because there is a significant list of such potential impacts, they are not explicitly collected together into an enumeration. In order to name an instance of IfcServiceLifeFactor, the inherited Name attribute should be used.
    /// 
    /// Within the IFC specification, any number of service life factors may be allowed to impact upon the service life of an artefact. In many cases, it is probable that the ISO standard that specifies good practice for service life consideration will be applied.
    /// 
    /// Within the ISO standard, there are seven defined (named) service life factors that may be applied to an IfcServiceLife. These are captured in the IfcServiceLifeFactorEnum (together with a user defined capability). Each factor can have three values that define an upper, lower and most used (or median) value.
    /// 
    /// One or more instances of IfcServiceLifeFactor can be related to an IfcServiceLife through the IfcRelDefinesByProperties relationship class.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcServiceLifeFactor : IfcPropertySetDefinition
    {
        private IfcServiceLifeFactorTypeEnum _PredefinedType;

        /// <summary>
        /// Predefined service life factor types from which that required may be set. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcServiceLifeFactorTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PredefinedType;
            }
            set { this.SetModelValue(this, ref _PredefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        private IfcMeasureValue _UpperValue;

        /// <summary>
        /// Upper of the three values assigned to the service life factor. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcMeasureValue UpperValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _UpperValue;
            }
            set { this.SetModelValue(this, ref _UpperValue, value, v => UpperValue = v, "UpperValue"); }
        }

        private IfcMeasureValue _MostUsedValue;

        /// <summary>
        /// Most used of the three values assigned to the service life factor. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcMeasureValue MostUsedValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _MostUsedValue;
            }
            set { this.SetModelValue(this, ref _MostUsedValue, value, v => MostUsedValue = v, "MostUsedValue"); }
        }

        private IfcMeasureValue _LowerValue;

        /// <summary>
        /// Lower of the three values assigned to the service life factor. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcMeasureValue LowerValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LowerValue;
            }
            set { this.SetModelValue(this, ref _LowerValue, value, v => LowerValue = v, "LowerValue"); }
        }

        public override string WhereRule()
        {
            var result = "";

            // TODO: This rule seems like a nonsense as this object doesn't inherit from IfcObject
            // NOT(PredefinedType = IfcServiceLifeFactorTypeEnum.USERDEFINED) OR EXISTS(SELF\IfcObject.ObjectType); 
            //if (false)
            //    result += "WR31: The attribute UserDefinedFactor must be asserted when the value of the IfcServiceLifeFactorEnum is set to USERDEFINED. \n";

            return result;
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _PredefinedType = (IfcServiceLifeFactorTypeEnum)Enum.Parse(typeof(IfcServiceLifeFactorTypeEnum), value.EnumVal);
                    break;
                case 5:
                    _UpperValue = (IfcMeasureValue)value.EntityVal;
                    break;
                case 6:
                    _MostUsedValue = (IfcMeasureValue)value.EntityVal;
                    break;
                case 7:
                    _LowerValue = (IfcMeasureValue)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
