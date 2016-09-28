#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFlowStorageDevices.cs
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
    public class IfcFlowStorageDevices
    {
        private readonly IModel _model;

        public IfcFlowStorageDevices(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFlowStorageDevice> Items
        {
            get { return this._model.Instances.OfType<IfcFlowStorageDevice>(); }
        }

        public IfcElectricFlowStorageDeviceTypes IfcElectricFlowStorageDeviceTypes
        {
            get { return new IfcElectricFlowStorageDeviceTypes(_model); }
        }
    }
}