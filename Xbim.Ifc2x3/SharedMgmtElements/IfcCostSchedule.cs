using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.MeasureResource;


namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcCostSchedule brings together instances of IfcCostItem either for the purpose of identifying purely cost information as in an estimate for constructions costs, bill of quantities etc. or for including cost information within another presentation form such as an order (of whatever type)
    /// </summary>
    [IfcPersistedEntity]
    public class IfcCostSchedule : IfcControl
    {
        public IfcCostSchedule()
            : base()
        {
            _TargetUsers = new XbimListUnique<IfcActorSelect>(this);
        }

        #region Fields

        private IfcActorSelect _SubmittedBy;
        private IfcActorSelect _PreparedBy;
        private IfcDateTimeSelect _SubmittedOn;
        private IfcLabel _Status;
        private XbimListUnique<IfcActorSelect> _TargetUsers;
        private IfcDateTimeSelect _UpdateDate; 
        private IfcIdentifier _ID;
        private IfcCostScheduleTypeEnum _PredefinedType; //	 

        #endregion

        /// <summary>
        /// The identity of the person or organization submitting the cost schedule.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcActorSelect SubmittedBy
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SubmittedBy;
            }
            set { this.SetModelValue(this, ref _SubmittedBy, value, v => SubmittedBy = v, "SubmittedBy"); }
        }

        /// <summary>
        /// The identity of the person or organization preparing the cost schedule.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcActorSelect PreparedBy
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PreparedBy;
            }
            set { this.SetModelValue(this, ref _PreparedBy, value, v => PreparedBy = v, "PreparedBy"); }
        }

        /// <summary>
        /// The date on which the cost schedule was submitted.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcDateTimeSelect SubmittedOn
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SubmittedOn;
            }
            set { this.SetModelValue(this, ref _SubmittedOn, value, v => SubmittedOn = v, "SubmittedOn"); }
        }

        /// <summary>
        /// The current status of a cost schedule. Examples of status values that might be used for a cost schedule status include: - PLANNED - APPROVED - AGREED - ISSUED - STARTED 
        /// /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel Status
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Status;
            }
            set { this.SetModelValue(this, ref _Status, value, v => Status = v, "Status"); }
        }

        /// <summary>
        /// The actors for whom the cost schedule was prepared.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1, -1)]
        public XbimListUnique<IfcActorSelect> TargetUsers
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TargetUsers;
            }
            set { this.SetModelValue(this, ref _TargetUsers, value, v => TargetUsers = v, "TargetUsers"); }
        }

        /// <summary>
        /// The date that this cost schedule is updated; this allows tracking the schedule history.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcDateTimeSelect UpdateDate
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _UpdateDate;
            }
            set { this.SetModelValue(this, ref _UpdateDate, value, v => UpdateDate = v, "UpdateDate"); }
        }

        /// <summary>
        /// A unique identification assigned to a cost schedule that enables its differentiation from other cost schedules.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcIdentifier ID
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ID;
            }
            set { this.SetModelValue(this, ref _ID, value, v => ID = v, "ID"); }
        }

        /// <summary>
        /// Predefined types of cost schedule from which that required may be selected.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcCostScheduleTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PredefinedType;
            }
            set { this.SetModelValue(this, ref _PredefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                    _SubmittedBy = (IfcActorSelect)value.EntityVal;
                    break;
                case 6:
                    _PreparedBy = (IfcActorSelect)value.EntityVal;
                    break;
                case 7:
                    _SubmittedOn = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 8:
                    _Status = (IfcLabel)value.StringVal;
                    break;
                case 10:
                      _TargetUsers.Add((IfcActorSelect)value.EntityVal);
                      break;
                case 11:
                     _UpdateDate = (IfcDateTimeSelect)value.EntityVal;
                     break;
                case 12:
                     _ID = (IfcIdentifier)value.EntityVal;
                     break;
                case 13:
                     _PredefinedType = (IfcCostScheduleTypeEnum)Enum.Parse(typeof(IfcCostScheduleTypeEnum), value.EnumVal);
                     break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

    }
}
