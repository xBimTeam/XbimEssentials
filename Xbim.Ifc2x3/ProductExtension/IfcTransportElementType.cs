using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;



namespace Xbim.Ifc2x3.ProductExtension
{
    [IfcPersistedEntity]
    public class IfcTransportElementType : IfcElementType
    {
        private IfcTransportElementTypeEnum _predefinedType;

        /// <summary>
        ///   Predefined types to define the particular type of the transport element. 
        ///   There may be property set definitions available for each predefined type
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcTransportElementTypeEnum PredefinedType
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
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _predefinedType =
                        (IfcTransportElementTypeEnum)
                        Enum.Parse(typeof(IfcTransportElementTypeEnum), value.EnumVal, true);
                    break;
              
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
