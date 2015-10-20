#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionFlowElementTypes.cs
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
    public class IfcDistributionFlowElementTypes
    {
        private readonly IModel _model;

        public IfcDistributionFlowElementTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcDistributionFlowElementType> Items
        {
            get { return this._model.Instances.OfType<IfcDistributionFlowElementType>(); }
        }

        public IfcEnergyConversionDeviceTypes IfcEnergyConversionDeviceTypes
        {
            get { return new IfcEnergyConversionDeviceTypes(_model); }
        }

        public IfcFlowControllerTypes IfcFlowControllerTypes
        {
            get { return new IfcFlowControllerTypes(_model); }
        }

        public IfcFlowFittingTypes IfcFlowFittingTypes
        {
            get { return new IfcFlowFittingTypes(_model); }
        }

        public IfcDistributionChamberElements IfcDistributionChamberElements
        {
            get { return new IfcDistributionChamberElements(_model); }
        }

        public IfcFlowTerminalTypes IfcFlowTerminalTypes
        {
            get { return new IfcFlowTerminalTypes(_model); }
        }

        public IfcFlowTreatmentDeviceTypes IfcFlowTreatmentDeviceTypes
        {
            get { return new IfcFlowTreatmentDeviceTypes(_model); }
        }

        public IfcFlowStorageDeviceTypes IfcFlowStorageDeviceTypes
        {
            get { return new IfcFlowStorageDeviceTypes(_model); }
        }

        public IfcFlowSegmentTypes IfcFlowSegmentTypes
        {
            get { return new IfcFlowSegmentTypes(_model); }
        }

        public IfcFlowMovingDeviceTypes IfcFlowMovingDeviceTypes
        {
            get { return new IfcFlowMovingDeviceTypes(_model); }
        }

        public IfcDistributionChamberElementTypes IfcDistributionChamberElementTypes
        {
            get { return new IfcDistributionChamberElementTypes(_model); }
        }
    }
}