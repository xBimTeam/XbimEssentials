#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionControlElementTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.SharedBldgServiceElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcDistributionControlElementTypes
    {
        private readonly IModel _model;

        public IfcDistributionControlElementTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcDistributionControlElementType> Items
        {
            get { return this._model.Instances.OfType<IfcDistributionControlElementType>(); }
        }

        public IfcControllerTypes IfcControllerTypes
        {
            get { return new IfcControllerTypes(_model); }
        }

        public IfcFlowInstrumentTypes IfcFlowInstrumentTypes
        {
            get { return new IfcFlowInstrumentTypes(_model); }
        }

        public IfcActuatorTypes IfcActuatorTypes
        {
            get { return new IfcActuatorTypes(_model); }
        }

        public IfcSensorTypes IfcSensorTypes
        {
            get { return new IfcSensorTypes(_model); }
        }

        public IfcAlarmTypes IfcAlarmTypes
        {
            get { return new IfcAlarmTypes(_model); }
        }
    }
}