using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ProcessExtensions
{
    public class IfcTask : IfcProcess
    {
        #region Fields

        private IfcIdentifier _taskId;
        private IfcLabel? _status;
        private IfcLabel? _workMethod;
        private bool _isMilestone;
        private int? _priority;

        #endregion

        /// <summary>
        ///   An identifying designation given to a task.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier TaskId
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _taskId;
            }
            set
            {
                this.SetModelValue(this, ref _taskId, value, v => TaskId = v,
                                           "TaskId");
            }
        }

        /// <summary>
        ///   Current status of the task.
        ///   NOTE: Particular values for status are not specified, these should be 
        ///   determined and agreed by local usage. Examples of possible status 
        ///   values include 'Not Yet Started', 'Started', 'Completed'.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLabel? Status
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _status;
            }
            set { this.SetModelValue(this, ref _status, value, v => Status = v, "Status"); }
        }

        /// <summary>
        ///   The method of work used in carrying out a task.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel? WorkMethod
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _workMethod;
            }
            set
            {
                this.SetModelValue(this, ref _workMethod, value, v => WorkMethod = v,
                                           "WorkMethod");
            }
        }

        /// <summary>
        ///   Identifies whether a task is a milestone task (=TRUE) or not (= FALSE).
        ///   NOTE: In small project planning applications, a milestone task may be 
        ///   understood to be a task having no duration. As such, it represents a 
        ///   singular point in time
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public bool IsMilestone
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _isMilestone;
            }
            set
            {
                this.SetModelValue(this, ref _isMilestone, value, v => IsMilestone = v,
                                           "IsMilestone");
            }
        }

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public int? Priority
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _priority;
            }
            set
            {
                this.SetModelValue(this, ref _priority, value, v => Priority = v,
                                           "Priority");
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
                    _taskId = (IfcIdentifier)value.StringVal;
                    break;
                case 6:
                    _status = (IfcLabel)value.StringVal;
                    break;
                case 7:
                    _workMethod = (IfcLabel)value.StringVal;
                    break;
                case 8:
                    _isMilestone = value.BooleanVal;
                    break;
                case 9:
                    _priority = (int)value.IntegerVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            return baseErr;
        }
    }

}
