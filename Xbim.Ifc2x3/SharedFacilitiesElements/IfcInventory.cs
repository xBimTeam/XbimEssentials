using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.CostResource;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    [IfcPersistedEntity]
    public class IfcInventory : IfcGroup
    {
        public IfcInventory()
        {
            _ResponsiblePersons = new XbimSet<IfcPerson>(this);
        }

        private IfcInventoryTypeEnum _InventoryType;

        /// <summary>
        ///  	A list of the types of inventories from which that required may be selected.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcInventoryTypeEnum InventoryType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _InventoryType;
            }
            set { this.SetModelValue(this, ref _InventoryType, value, v => InventoryType = v, "InventoryType"); }
        }

        private IfcActorSelect _Jurisdiction;

        /// <summary>
        /// The organizational unit to which the inventory is applicable. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcActorSelect Jurisdiction
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Jurisdiction;
            }
            set { this.SetModelValue(this, ref _Jurisdiction, value, v => Jurisdiction = v, "Jurisdiction"); }
        }

        private XbimSet<IfcPerson> _ResponsiblePersons;

        /// <summary>
        /// Persons who are responsible for the inventory. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcPerson> ResponsiblePersons
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ResponsiblePersons;
            }
            set { this.SetModelValue(this, ref _ResponsiblePersons, value, v => ResponsiblePersons = v, "ResponsiblePersons"); }
        }

        private IfcCalendarDate _LastUpdateDate;

        /// <summary>
        /// The date on which the last update of the inventory was carried out. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcCalendarDate LastUpdateDate
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LastUpdateDate;
            }
            set { this.SetModelValue(this, ref _LastUpdateDate, value, v => LastUpdateDate = v, "LastUpdateDate"); }
        }

        private IfcCostValue _CurrentValue;

        /// <summary>
        /// An estimate of the current cost value of the inventory. 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcCostValue CurrentValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _CurrentValue;
            }
            set { this.SetModelValue(this, ref _CurrentValue, value, v => CurrentValue = v, "CurrentValue"); }
        }

        private IfcCostValue _OriginalValue;

        /// <summary>
        /// An estimate of the original cost value of the inventory. 
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcCostValue OriginalValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _OriginalValue;
            }
            set { this.SetModelValue(this, ref _OriginalValue, value, v => OriginalValue = v, "OriginalValue"); }
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
                    _InventoryType = (IfcInventoryTypeEnum)Enum.Parse(typeof(IfcInventoryTypeEnum), value.EnumVal);
                    break;
                case 6:
                    _Jurisdiction = (IfcActorSelect)value.EntityVal;
                    break;
                case 7:
                    _ResponsiblePersons.Add((IfcPerson)value.EntityVal);
                    break;
                case 8:
                    _LastUpdateDate = (IfcCalendarDate)value.EntityVal;
                    break;
                case 9:
                    _CurrentValue = (IfcCostValue)value.EntityVal;
                    break;
                case 10:
                    _OriginalValue = (IfcCostValue)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (IsGroupedBy != null && IsGroupedBy.RelatedObjects.Any(o => !(o is IfcSpace || o is IfcFurnishingElement || o is IfcAsset)))
                result += "WR41: Constrains the type of objects that can be contained within an IfcInventory to IfcSpace, IfcFurnishingElement and IfcAsset. \n";

            return result;
        }
    }
}
