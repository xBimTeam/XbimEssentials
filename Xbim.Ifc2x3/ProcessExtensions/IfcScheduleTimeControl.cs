using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;

using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.ProcessExtensions
{
    /// <summary>
    /// The IfcScheduleTimeControl captures the time-related information about a process including the different types 
    /// (i.e. actual, or scheduled) of starting and ending times, duration, float times, etc.
    /// </summary>
    /// <remarks>
    /// Scheduled and actual durations of a task and all float times should be derived within an application from relevant start and 
    /// finish times that are also attributes of this class. Note that they are not directly derived within the IFC specification at 
    /// this stage due to the differences in data type between time measures date/time selections.
    ///
    /// The critical nature of an IfcScheduleTimeControl may also be derived within an application by comparing relevant start and 
    /// finish date/time selections but is not derived within the IFC specification at this stage.
    /// </remarks>
    public class IfcScheduleTimeControl : IfcControl
    {
        // TODO: NotImplemented

        #region Fields


        private IfcDateTimeSelect _actualStart;
        private IfcDateTimeSelect _earlyStart;
        private IfcDateTimeSelect _lateStart;
        private IfcDateTimeSelect _scheduleStart;
        private IfcDateTimeSelect _actualFinish;
        private IfcDateTimeSelect _earlyFinish;
        private IfcDateTimeSelect _lateFinish;
        private IfcDateTimeSelect _scheduleFinish;
        private IfcTimeMeasure? _scheduleDuration;
        private IfcTimeMeasure? _actualDuration;
        private IfcTimeMeasure? _remainingTime;
        private IfcTimeMeasure? _freeFloat;
        private IfcTimeMeasure? _totalFloat;
        private IfcBoolean? _isCritical;
        private IfcDateTimeSelect _statusTime;
        private IfcTimeMeasure? _startFloat;
        private IfcTimeMeasure? _finishFloat;
        private IfcPositiveRatioMeasure _completion;

        #endregion

        /// <summary>
        ///   The date on which a task is actually started. 
        /// </summary>
        /// <remarks>NOTE: The scheduled start date must be greater than or equal to the earliest start date. 
        /// No constraint is applied to the actual start date with respect to the scheduled start date since a task
        /// may be started earlier than had originally been scheduled if circumstances allow.</remarks>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ActualStart
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _actualStart;
            }
            set
            {
                this.SetModelValue(this, ref _actualStart, value, v => ActualStart = v,
                                           "ActualStart");
            }
        }

        /// <summary>
        /// The earliest date on which a task can be started.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcDateTimeSelect EarlyStart
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _earlyStart;
            }
            set
            {
                this.SetModelValue(this, ref _earlyStart, value, v => EarlyStart = v,
                                                 "EarlyStart");
            }
        }

        /// <summary>
        /// The latest date on which a task can be started.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcDateTimeSelect LateStart
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _lateStart;
            }
            set
            {
                this.SetModelValue(this, ref _lateStart, value, v => LateStart = v,
                                                 "LateStart");
            }
        }

        /// <summary>
        /// The date on which a task is scheduled to be started.
        /// </summary>
        /// <remarks>NOTE: The scheduled start date must be greater than or equal to the earliest start date.</remarks>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ScheduleStart
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _scheduleStart;
            }
            set
            {
                this.SetModelValue(this, ref _scheduleStart, value, v => ScheduleStart = v,
                                                 "ScheduleStart");
            }
        }

        /// <summary>
        /// The date on which a task is actually finished.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ActualFinish
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _actualFinish;
            }
            set
            {
                this.SetModelValue(this, ref _actualFinish, value, v => ActualFinish = v,
                                                 "ActualFinish");
            }
        }

        /// <summary>
        /// The earliest date on which a task can be finished.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcDateTimeSelect EarlyFinish
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _earlyFinish;
            }
            set
            {
                this.SetModelValue(this, ref _earlyFinish, value, v => EarlyFinish = v,
                                                 "EarlyFinish");
            }
        }

        /// <summary>
        /// The latest date on which a task can be finished.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcDateTimeSelect LateFinish
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _lateFinish;
            }
            set
            {
                this.SetModelValue(this, ref _lateFinish, value, v => LateFinish = v,
                                                 "LateFinish");
            }
        }

        /// <summary>
        /// The date on which a task is scheduled to be finished. 
        /// </summary>
        /// <remarks>NOTE: The scheduled finish date must be greater than or equal to the earliest finish date.</remarks>
        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcDateTimeSelect ScheduleFinish
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _scheduleFinish;
            }
            set
            {
                this.SetModelValue(this, ref _scheduleFinish, value, v => ScheduleFinish = v,
                                                 "ScheduleFinish");
            }
        }

        /// <summary>
        /// The amount of time which is scheduled for completion of a task. 
        /// </summary>
        /// <remarks>NOTE: Scheduled Duration may be calculated as the time from scheduled start date to scheduled finish date.</remarks>
        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcTimeMeasure? ScheduleDuration
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _scheduleDuration;
            }
            set
            {
                this.SetModelValue(this, ref _scheduleDuration, value, v => ScheduleDuration = v,
                                                 "ScheduleDuration");
            }
        }


        /// <summary>
        /// The actual duration of the task.
        /// </summary>
        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcTimeMeasure? ActualDuration
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _actualDuration;
            }
            set
            {
                this.SetModelValue(this, ref _actualDuration, value, v => ActualDuration = v,
                                                 "ActualDuration");
            }
        }

        /// <summary>
        /// The amount of time remaining to complete a task. 
        /// </summary>
        /// <remarks>NOTE: The time remaining in which to complete a task may be determined both for tasks which have 
        /// not yet started and those which have. Remaining time for a task not yet started has the same value as the 
        /// scheduled duration.  For a task already started, remaining time is calculated as the difference between 
        /// the scheduled finish and the point of analysis.</remarks>
        [IfcAttribute(16, IfcAttributeState.Optional)]
        public IfcTimeMeasure? RemainingTime
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _remainingTime;
            }
            set
            {
                this.SetModelValue(this, ref _remainingTime, value, v => RemainingTime = v,
                                                 "RemainingTime");
            }
        }

        /// <summary>
        /// The amount of time during which the start or finish of a task may be varied without any effect on the overall programme of work.
        /// </summary>
        [IfcAttribute(17, IfcAttributeState.Optional)]
        public IfcTimeMeasure? FreeFloat
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _freeFloat;
            }
            set
            {
                this.SetModelValue(this, ref _freeFloat, value, v => FreeFloat = v,
                                                 "FreeFloat");
            }
        }

        /// <summary>
        /// The difference between the duration available to carry out a task and the scheduled duration of the task. 
        /// </summary>
        /// <remarks>NOTE: Total Float time may be calculated as being the difference between the scheduled duration 
        /// of a task and the available duration from earliest start to latest finish. Float time may be either positive, 
        /// zero or negative. Where it is zero or negative, the task becomes critical.</remarks>
        [IfcAttribute(18, IfcAttributeState.Optional)]
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
        /// 	 A flag which identifies whether a scheduled task is a critical item within the programme. 
        /// </summary>
        /// <remarks>NOTE: A task becomes critical when the float time becomes zero or negative.</remarks>
        [IfcAttribute(19, IfcAttributeState.Optional)]
        public IfcBoolean? IsCritical
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _isCritical;
            }
            set
            {
                this.SetModelValue(this, ref _isCritical, value, v => IsCritical = v,
                                                 "IsCritical");
            }
        }

        /// <summary>
        /// The date or time at which the status of the tasks within the schedule is analyzed.
        /// </summary>
        [IfcAttribute(20, IfcAttributeState.Optional)]
        public IfcDateTimeSelect StatusTime
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _statusTime;
            }
            set
            {
                this.SetModelValue(this, ref _statusTime, value, v => StatusTime = v,
                                                 "StatusTime");
            }
        }

        /// <summary>
        /// The difference between the late start and early start of a task. Start float measures how long an task's start 
        /// can be delayed and still not have an impact on the overall duration of a schedule.
        /// </summary>
        [IfcAttribute(21, IfcAttributeState.Optional)]
        public IfcTimeMeasure? StartFloat
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _startFloat;
            }
            set
            {
                this.SetModelValue(this, ref _startFloat, value, v => StartFloat = v,
                                                 "StartFloat");
            }
        }

        /// <summary>
        /// The difference between the late finish and early finish of a task. Finish float measures how long a task's 
        /// finish can be delayed and still not have an impact on the overall duration of a schedule.
        /// </summary>
        [IfcAttribute(22, IfcAttributeState.Optional)]
        public IfcTimeMeasure? FinishFloat
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _finishFloat;
            }
            set
            {
                this.SetModelValue(this, ref _finishFloat, value, v => FinishFloat = v,
                                                 "FinishFloat");
            }
        }

        /// <summary>
        /// 	 The extent of completion expressed as a ratio or percentage.
        /// </summary>
        [IfcAttribute(23, IfcAttributeState.Optional)]
        public IfcPositiveRatioMeasure Completion
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _completion;
            }
            set
            {
                this.SetModelValue(this, ref _completion, value, v => Completion = v,
                                                 "Completion");
            }
        }


        #region Inverse Relationships

        /// <summary>
        ///   The assigned schedule time control in the relationship.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelAssignsTasks> ScheduleTimeControlAssigned 
        {
            get
            {
                return
                    this.ModelOf.Instances.Where<IfcRelAssignsTasks>(r => r.TimeForTask == this);
            }
        }

        #endregion
        	
//SUBTYPE OF (	IfcControl);
//ActualStart	 : 	OPTIONAL IfcDateTimeSelect;
//EarlyStart	 : 	OPTIONAL IfcDateTimeSelect;
//LateStart	 : 	OPTIONAL IfcDateTimeSelect;
//ScheduleStart	 : 	OPTIONAL IfcDateTimeSelect;
//ActualFinish	 : 	OPTIONAL IfcDateTimeSelect;
//EarlyFinish	 : 	OPTIONAL IfcDateTimeSelect;
//LateFinish	 : 	OPTIONAL IfcDateTimeSelect;

//ScheduleFinish	 : 	OPTIONAL IfcDateTimeSelect;
//ScheduleDuration	 : 	OPTIONAL IfcTimeMeasure;
//ActualDuration	 : 	OPTIONAL IfcTimeMeasure;
//RemainingTime	 : 	OPTIONAL IfcTimeMeasure;
//FreeFloat	 : 	OPTIONAL IfcTimeMeasure;
//TotalFloat	 : 	OPTIONAL IfcTimeMeasure;
//IsCritical	 : 	OPTIONAL BOOLEAN;
//StatusTime	 : 	OPTIONAL IfcDateTimeSelect;
//StartFloat	 : 	OPTIONAL IfcTimeMeasure;
//FinishFloat	 : 	OPTIONAL IfcTimeMeasure;
//Completion	 : 	OPTIONAL IfcPositiveRatioMeasure;
//INVERSE
//ScheduleTimeControlAssigned	 : 	IfcRelAssignsTasks FOR TimeForTask;
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
                    _actualStart = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 6:
                    _earlyStart = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 7:
                    _lateStart = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 8:
                    _scheduleStart = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 9:
                    _actualFinish = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 10:
                    _earlyFinish = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 11:
                    _lateFinish = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 12:
                    _scheduleFinish = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 13:
                    _scheduleDuration = (IfcTimeMeasure)value.RealVal;
                    break;
                case 14:
                    _actualDuration = (IfcTimeMeasure)value.RealVal;
                    break;
                case 15:
                    _remainingTime = (IfcTimeMeasure)value.RealVal;
                    break;
                case 16:
                    _freeFloat = (IfcTimeMeasure)value.RealVal;
                    break;
                case 17:
                    _totalFloat = (IfcTimeMeasure)value.RealVal;
                    break;
                case 18:
                    _isCritical = (IfcBoolean)value.BooleanVal;
                    break;
                case 19:
                    _statusTime = (IfcDateTimeSelect)value.EntityVal;
                    break;
                case 20:
                    _startFloat = (IfcTimeMeasure)value.RealVal;
                    break;
                case 21:
                    _finishFloat = (IfcTimeMeasure)value.RealVal;
                    break;
                case 22:
                    _completion = (IfcPositiveRatioMeasure)value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
