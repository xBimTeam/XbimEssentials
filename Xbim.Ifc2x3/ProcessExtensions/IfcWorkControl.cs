using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.ActorResource;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ProcessExtensions
{
    /// <summary>
    /// An IfcWorkControl is an abstract supertype which captures information that is common to both IfcWorkPlan and IfcWorkSchedule
    /// </summary>
    /// <remarks>
    /// Use Definitions
    /// A work control may have resources assigned to it, this is handled by the IfcRelAssignsToControl relationship. The assignment of tasks 
    /// to the work control is handled by the IfcRelAssignsTasks relationship.
    /// The inherited attributes have the following meaning:
    ///     IfcControl.Controls - references to the IfcRelAssignsTasks, that assign instances of IfcTask including time schedule controls.
    ///     IfcObject.HasAssignments - references to the IfcRelAssignsToResources, that assigns an instance of IfcResource to the IfcWorkControl.
    /// The attribute IfcWorkControl.Purpose is used to define the purpose of either a work schedule or a work plan. In the case of IfcWorkPlan, 
    /// the purpose attribute can be used to determine if the work plan is for cost estimating, task scheduling or some other defined purpose.
    /// </remarks>
    public class IfcWorkControl : IfcControl
    {
        public IfcWorkControl()
        {
            _creators = new XbimSet<IfcPerson>(this);
        }

        #region Fields

        	
        private IfcIdentifier _identifier;
        private IfcDateTimeSelect _creationDate;
        private XbimSet<IfcPerson> _creators;
        private IfcLabel? _purpose;
        private IfcTimeMeasure? _duration;
        private IfcTimeMeasure? _totalFloat;
        private IfcDateTimeSelect _startTime;
        private IfcDateTimeSelect _finishTime;
        private IfcWorkControlTypeEnum? _workControlType;
        private IfcLabel? _userDefinedControlType;


        #endregion

        /// <summary>
        ///   Identifier of the work plan, given by user.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier Identifier
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _identifier;
            }
            set
            {
                this.SetModelValue(this, ref _identifier, value, v => Identifier = v,
                                           "Identifier");
            }
        }

        /// <summary>
        ///   The date that the plan is created
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcDateTimeSelect CreationDate
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _creationDate;
            }
            set { this.SetModelValue(this, ref _creationDate, value, v => CreationDate = v, "CreationDate"); }
        }

        /// <summary>
        ///   The authors of the work plan.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional, IfcAttributeType.Set, 1)]
        public XbimSet<IfcPerson> Creators
        {
            // TODO: Review this has cardinality. 
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _creators;
            }
            set
            {
                this.SetModelValue(this, ref _creators, value, v => Creators = v,
                                           "Creators");
            }
        }

        /// <summary>
        ///   A description of the purpose of the work schedule.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel? Purpose
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _purpose;
            }
            set
            {
                this.SetModelValue(this, ref _purpose, value, v => Purpose = v,
                                           "Purpose");
            }
        }

        /// <summary>
        /// The total duration of the entire work schedule.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcTimeMeasure? Duration
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _duration;
            }
            set
            {
                this.SetModelValue(this, ref _duration, value, v => Duration = v,
                                           "Duration");
            }
        }

        /// <summary>
        /// The total time float of the entire work schedule.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcTimeMeasure? TotalFloat
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _totalFloat;
            }
            set
            {
                this.SetModelValue(this, ref _totalFloat, value, v => TotalFloat = v,
                                           "TotalFloat");
            }
        }

        /// <summary>
        /// The start time of the schedule.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcDateTimeSelect StartTime
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _startTime;
            }
            set
            {
                this.SetModelValue(this, ref _startTime, value, v => StartTime = v,
                                           "StartTime");
            }
        }

        /// <summary>
        /// The finish time of the schedule.
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcDateTimeSelect FinishTime
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _finishTime;
            }
            set
            {
                this.SetModelValue(this, ref _finishTime, value, v => FinishTime = v,
                                           "FinishTime");
            }
        }

        /// <summary>
        /// The finish time of the schedule.
        /// </summary>
        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcWorkControlTypeEnum? WorkControlType
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _workControlType;
            }
            set
            {
                this.SetModelValue(this, ref _workControlType, value, v => WorkControlType = v,
                                           "WorkControlType");
            }
        }

        /// <summary>
        /// The finish time of the schedule.
        /// </summary>
        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcLabel? UserDefinedControlType
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _userDefinedControlType;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedControlType, value, v => UserDefinedControlType = v,
                                           "UserDefinedControlType");
            }
        }

        #region ISupportIfcParser Members

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
                    _identifier = (IfcIdentifier)value.StringVal;
                    break;
                case 6:
                    _creationDate = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 7:
                    _creators.Add((IfcPerson)value.EntityVal);
                    break;
                case 8:
                    _purpose = value.StringVal;
                    break;
                case 9:
                    _duration = value.RealVal;
                    break;
                case 10:
                    _totalFloat = value.RealVal;
                    break;
                case 11:
                    _startTime = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 12:
                    _finishTime = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 13:
                    _workControlType = (IfcWorkControlTypeEnum)Enum.Parse(typeof(IfcWorkControlTypeEnum), value.EnumVal, true);
                    break;
                case 14:
                    _userDefinedControlType = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            // TODO:(WorkControlType <> IfcWorkControlTypeEnum.USERDEFINED) OR ((WorkControlType = IfcWorkControlTypeEnum.USERDEFINED) AND EXISTS(SELF\IfcWorkControl.UserDefinedControlType));
            string baseErr = base.WhereRule();
            return baseErr;
        }
    }

}
