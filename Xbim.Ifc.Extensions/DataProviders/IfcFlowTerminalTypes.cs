#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFlowTerminalTypes.cs
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
    public class IfcFlowTerminalTypes
    {
        private readonly IModel _model;

        public IfcFlowTerminalTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFlowTerminalType> Items
        {
            get { return this._model.Instances.OfType<IfcFlowTerminalType>(); }
        }

        public IfcElectricApplianceTypes IfcElectricApplianceTypes
        {
            get { return new IfcElectricApplianceTypes(_model); }
        }

        public IfcSanitaryTerminalTypes IfcSanitaryTerminalTypes
        {
            get { return new IfcSanitaryTerminalTypes(_model); }
        }

        public IfcWasteTerminalTypes IfcWasteTerminalTypes
        {
            get { return new IfcWasteTerminalTypes(_model); }
        }

        public IfcLightFixtureTypes IfcLightFixtureTypes
        {
            get { return new IfcLightFixtureTypes(_model); }
        }

        public IfcLampTypes IfcLampTypes
        {
            get { return new IfcLampTypes(_model); }
        }

        public IfcFireSuppressionTerminalTypes IfcFireSuppressionTerminalTypes
        {
            get { return new IfcFireSuppressionTerminalTypes(_model); }
        }

        public IfcOutletTypes IfcOutletTypes
        {
            get { return new IfcOutletTypes(_model); }
        }

        public IfcElectricHeaterTypes IfcElectricHeaterTypes
        {
            get { return new IfcElectricHeaterTypes(_model); }
        }

        public IfcStackTerminalTypes IfcStackTerminalTypes
        {
            get { return new IfcStackTerminalTypes(_model); }
        }

        public IfcAirTerminalTypes IfcAirTerminalTypes
        {
            get { return new IfcAirTerminalTypes(_model); }
        }
    }
}