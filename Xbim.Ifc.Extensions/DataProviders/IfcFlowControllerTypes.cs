#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFlowControllerTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.SharedBldgServiceElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcFlowControllerTypes
    {
        private readonly IModel _model;

        public IfcFlowControllerTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFlowControllerType> Items
        {
            get { return this._model.Instances.OfType<IfcFlowControllerType>(); }
        }

        public IfcAirTerminalBoxTypes IfcAirTerminalBoxTypes
        {
            get { return new IfcAirTerminalBoxTypes(_model); }
        }

        public IfcSwitchingDeviceTypes IfcSwitchingDeviceTypes
        {
            get { return new IfcSwitchingDeviceTypes(_model); }
        }

        public IfcProtectiveDeviceTypes IfcProtectiveDeviceTypes
        {
            get { return new IfcProtectiveDeviceTypes(_model); }
        }

        public IfcElectricTimeControlTypes IfcElectricTimeControlTypes
        {
            get { return new IfcElectricTimeControlTypes(_model); }
        }

        public IfcFlowMeterTypes IfcFlowMeterTypes
        {
            get { return new IfcFlowMeterTypes(_model); }
        }

        public IfcValveTypes IfcValveTypes
        {
            get { return new IfcValveTypes(_model); }
        }

        public IfcDamperTypes IfcDamperTypes
        {
            get { return new IfcDamperTypes(_model); }
        }
    }
}