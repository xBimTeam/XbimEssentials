#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEnergyConversionDeviceTypes.cs
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
    public class IfcEnergyConversionDeviceTypes
    {
        private readonly IModel _model;

        public IfcEnergyConversionDeviceTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcEnergyConversionDeviceType> Items
        {
            get { return this._model.Instances.OfType<IfcEnergyConversionDeviceType>(); }
        }

        public IfcEvaporativeCoolerTypes IfcEvaporativeCoolerTypes
        {
            get { return new IfcEvaporativeCoolerTypes(_model); }
        }

        public IfcCoolingTowerTypes IfcCoolingTowerTypes
        {
            get { return new IfcCoolingTowerTypes(_model); }
        }

        public IfcHeatExchangerTypes IfcHeatExchangerTypes
        {
            get { return new IfcHeatExchangerTypes(_model); }
        }

        public IfcCooledBeamTypes IfcCooledBeamTypes
        {
            get { return new IfcCooledBeamTypes(_model); }
        }

        public IfcBoilerTypes IfcBoilerTypes
        {
            get { return new IfcBoilerTypes(_model); }
        }

        public IfcMotorConnectionTypes IfcMotorConnectionTypes
        {
            get { return new IfcMotorConnectionTypes(_model); }
        }

        public IfcElectricGeneratorTypes IfcElectricGeneratorTypes
        {
            get { return new IfcElectricGeneratorTypes(_model); }
        }

        public IfcCondenserTypes IfcCondenserTypes
        {
            get { return new IfcCondenserTypes(_model); }
        }

        public IfcCoilTypes IfcCoilTypes
        {
            get { return new IfcCoilTypes(_model); }
        }

        public IfcAirToAirHeatRecoveryTypes IfcAirToAirHeatRecoveryTypes
        {
            get { return new IfcAirToAirHeatRecoveryTypes(_model); }
        }

        public IfcEvaporatorTypes IfcEvaporatorTypes
        {
            get { return new IfcEvaporatorTypes(_model); }
        }

        public IfcElectricMotorTypes IfcElectricMotorTypes
        {
            get { return new IfcElectricMotorTypes(_model); }
        }

        public IfcUnitaryEquipmentTypes IfcUnitaryEquipmentTypes
        {
            get { return new IfcUnitaryEquipmentTypes(_model); }
        }

        public IfcSpaceHeaterTypes IfcSpaceHeaterTypes
        {
            get { return new IfcSpaceHeaterTypes(_model); }
        }

        public IfcHumidifierTypes IfcHumidifierTypes
        {
            get { return new IfcHumidifierTypes(_model); }
        }

        public IfcTubeBundleTypes IfcTubeBundleTypes
        {
            get { return new IfcTubeBundleTypes(_model); }
        }

        public IfcTransformerTypes IfcTransformerTypes
        {
            get { return new IfcTransformerTypes(_model); }
        }

        public IfcChillerTypes IfcChillerTypes
        {
            get { return new IfcChillerTypes(_model); }
        }
    }
}