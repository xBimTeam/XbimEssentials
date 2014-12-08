using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.CostResource;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    [IfcPersistedEntity]
    public class IfcAsset : IfcGroup
    {
        private IfcIdentifier _AssetID;

        /// <summary>
        /// A unique identification assigned to an asset that enables its differentiation from other assets.
        /// NOTE: The asset identifier is unique within the asset register. It differs from the globally unique
        /// id assigned to the instance of an entity 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier AssetID
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _AssetID;
            }
            set { this.SetModelValue(this, ref _AssetID, value, v => AssetID = v, "AssetID"); }
        }

        private IfcCostValue _OriginalValue;

        /// <summary>
        /// The cost value of the asset at the time of purchase. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcCostValue OriginalValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _OriginalValue;
            }
            set { this.SetModelValue(this, ref _OriginalValue, value, v => OriginalValue = v, "OriginalValue"); }
        }

        private IfcCostValue _CurrentValue;

        /// <summary>
        /// The current cost value of the asset. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcCostValue CurrentValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _CurrentValue;
            }
            set { this.SetModelValue(this, ref _CurrentValue, value, v => CurrentValue = v, "CurrentValue"); }
        }

        private IfcCostValue _TotalReplacementCost;

        /// <summary>
        /// The total cost of replacement of the asset. 
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcCostValue TotalReplacementCost
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TotalReplacementCost;
            }
            set { this.SetModelValue(this, ref _TotalReplacementCost, value, v => TotalReplacementCost = v, "TotalReplacementCost"); }
        }

        private IfcActorSelect _Owner;

        /// <summary>
        /// The name of the person or organization that 'owns' the asset. 
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcActorSelect Owner
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Owner;
            }
            set { this.SetModelValue(this, ref _Owner, value, v => Owner = v, "Owner"); }
        }

        private IfcActorSelect _User;

        /// <summary>
        /// The name of the person or organization that 'uses' the asset. 
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcActorSelect User
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _User;
            }
            set { this.SetModelValue(this, ref _User, value, v => User = v, "User"); }
        }

        private IfcPerson _ResponsiblePerson;

        /// <summary>
        /// The person designated to be responsible for the asset.
        /// NOTE: In (e.g.) UK Law (Health and Safety at Work Act, Electricity at Work 
        /// Regulations, and others), management of assets must have a person identified as 
        /// being responsible and to whom regulatory, insurance and other organizations communicate. 
        /// In places where there is not a legal requirement, the responsible person would be the asset 
        /// manager but would not have a legal status. 
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcPerson ResponsiblePerson
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ResponsiblePerson;
            }
            set { this.SetModelValue(this, ref _ResponsiblePerson, value, v => ResponsiblePerson = v, "ResponsiblePerson"); }
        }

        private IfcCalendarDate _IncorporationDate;

        /// <summary>
        /// The date on which an asset was incorporated into the works, installed, constructed, erected or completed.
        /// NOTE: This is the date on which an asset is considered to start depreciating. 
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcCalendarDate IncorporationDate
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _IncorporationDate;
            }
            set { this.SetModelValue(this, ref _IncorporationDate, value, v => IncorporationDate = v, "IncorporationDate"); }
        }

        private IfcCostValue _DepreciatedValue;

        /// <summary>
        /// The current value of an asset within the accounting rules and procedures of an organization. 
        /// </summary>
        [IfcAttribute(14, IfcAttributeState.Mandatory)]
        public IfcCostValue DepreciatedValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _DepreciatedValue;
            }
            set { this.SetModelValue(this, ref _DepreciatedValue, value, v => DepreciatedValue = v, "DepreciatedValue"); }
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
                    _AssetID = value.StringVal;
                    break;
                case 6:
                    _OriginalValue = (IfcCostValue)value.EntityVal;
                    break;
                case 7:
                    _CurrentValue = (IfcCostValue)value.EntityVal;
                    break;
                case 8:
                    _TotalReplacementCost = (IfcCostValue)value.EntityVal;
                    break;
                case 9:
                    _Owner = (IfcActorSelect)value.EntityVal;
                    break;
                case 10:
                    _User = (IfcActorSelect)value.EntityVal;
                    break;
                case 11:
                    _ResponsiblePerson = (IfcPerson)value.EntityVal;
                    break;
                case 12:
                    _IncorporationDate = (IfcCalendarDate)value.EntityVal;
                    break;
                case 13:
                    _DepreciatedValue = (IfcCostValue)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }

        }

        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (IsGroupedBy != null && IsGroupedBy.RelatedObjects.Any(o => !(o is IfcElement)))
                result += "WR1: Constrains the contents of the group forming the IfcAsset to be instances of IfcElement. This allows for both spatial structures and physical elements to participate in an asset. \n";

            return result;
        }
    }
}
