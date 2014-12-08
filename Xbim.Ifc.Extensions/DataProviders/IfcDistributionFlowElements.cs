#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionFlowElements.cs
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
    public class IfcDistributionFlowElements
    {
        private readonly IModel _model;

        public IfcDistributionFlowElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcDistributionFlowElement> Items
        {
            get { return this._model.Instances.OfType<IfcDistributionFlowElement>(); }
        }

        public IfcFlowStorageDevices IfcFlowStorageDevices
        {
            get { return new IfcFlowStorageDevices(_model); }
        }

        public IfcFlowFittings IfcFlowFittings
        {
            get { return new IfcFlowFittings(_model); }
        }

        public IfcFlowTerminals IfcFlowTerminals
        {
            get { return new IfcFlowTerminals(_model); }
        }

        public IfcFlowControllers IfcFlowControllers
        {
            get { return new IfcFlowControllers(_model); }
        }

        public IfcFlowSegments IfcFlowSegments
        {
            get { return new IfcFlowSegments(_model); }
        }

        public IfcFlowMovingDevices IfcFlowMovingDevices
        {
            get { return new IfcFlowMovingDevices(_model); }
        }

        public IfcEnergyConversionDevices IfcEnergyConversionDevices
        {
            get { return new IfcEnergyConversionDevices(_model); }
        }

        public IfcFlowTreatmentDevices IfcFlowTreatmentDevices
        {
            get { return new IfcFlowTreatmentDevices(_model); }
        }
    }
}