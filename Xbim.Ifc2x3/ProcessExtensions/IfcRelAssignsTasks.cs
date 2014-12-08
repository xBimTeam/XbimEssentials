using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.ProcessExtensions
{
    public class IfcRelAssignsTasks : IfcRelAssignsToControl
    {
        #region Fields

        private IfcScheduleTimeControl _timeForTask;
       

        #endregion

        /// <summary>
        ///   An identifying designation given to a task.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcScheduleTimeControl TimeForTask
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _timeForTask;
            }
            set
            {
                this.SetModelValue(this, ref _timeForTask, value, v => TimeForTask = v,
                                           "TimeForTask");
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
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _timeForTask = (IfcScheduleTimeControl)value.EntityVal;
                    break;
                
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            // TODO: 
//WR1	 : 	HIINDEX(SELF\IfcRelAssigns.RelatedObjects) = 1;
//WR2	 : 	'IFCPROCESSEXTENSION.IFCTASK' IN TYPEOF(SELF\IfcRelAssigns.RelatedObjects[1]);
//WR3	 : 	'IFCPROCESSEXTENSION.IFCWORKCONTROL' IN TYPEOF(SELF\IfcRelAssignsToControl.RelatingControl)
            string baseErr = base.WhereRule();
            return baseErr;
        }
    }
}
