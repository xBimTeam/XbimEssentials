using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ControlExtension
{
    [IfcPersistedEntity]
    public class IfcPerformanceHistory: IfcControl
    {
        private IfcLabel _LifeCyclePhase;

        /// <summary>
        /// Describes the applicable building life-cycle phase. Typical values should be DESIGNDEVELOPMENT, SCHEMATICDEVELOPMENT, CONSTRUCTIONDOCUMENT, CONSTRUCTION, ASBUILT, COMMISSIONING, OPERATION, etc. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcLabel LifeCyclePhase
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LifeCyclePhase;
            }
            set { this.SetModelValue(this, ref _LifeCyclePhase, value, v => LifeCyclePhase = v, "LifeCyclePhase"); }
        }
    }
}
