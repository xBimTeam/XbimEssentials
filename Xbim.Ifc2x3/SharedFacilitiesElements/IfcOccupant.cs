using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    /// <summary>
    /// AnIfcOccupant is a type of actor that defines the form of occupancy of a property.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcOccupant : IfcActor
    {
        private IfcOccupantTypeEnum _predefinedType;

        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcOccupantTypeEnum PredefinedType
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                    base.IfcParse(propIndex, value); //fall through and call base
                    break;
                case 6:
                    _predefinedType = (IfcOccupantTypeEnum)Enum.Parse(typeof(IfcOccupantTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (PredefinedType == IfcOccupantTypeEnum.USERDEFINED && !ObjectType.HasValue)
                baseErr +=
                    "WR31 OccupantType :The attribute ObjectType shall be given, when the value of the IfcOccupantTypeEnum is set to USERDEFINED.\n";
            return baseErr;
        }
    }
}
