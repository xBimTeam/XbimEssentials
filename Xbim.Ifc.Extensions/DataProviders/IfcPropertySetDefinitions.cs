#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertySetDefinitions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPropertySetDefinitions
    {
        private readonly IModel _model;

        public IfcPropertySetDefinitions(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPropertySetDefinition> Items
        {
            get { return this._model.Instances.OfType<IfcPropertySetDefinition>(); }
        }

        public IfcPropertySets IfcPropertySets
        {
            get { return new IfcPropertySets(_model); }
        }

        public IfcEnergyPropertiess IfcEnergyPropertiess
        {
            get { return new IfcEnergyPropertiess(_model); }
        }

        public IfcWindowPanelPropertiess IfcWindowPanelPropertiess
        {
            get { return new IfcWindowPanelPropertiess(_model); }
        }

        public IfcWindowLiningPropertiess IfcWindowLiningPropertiess
        {
            get { return new IfcWindowLiningPropertiess(_model); }
        }

        public IfcDoorLiningPropertiess IfcDoorLiningPropertiess
        {
            get { return new IfcDoorLiningPropertiess(_model); }
        }

        public IfcSoundValues IfcSoundValues
        {
            get { return new IfcSoundValues(_model); }
        }

        public IfcReinforcementDefinitionPropertiess IfcReinforcementDefinitionPropertiess
        {
            get { return new IfcReinforcementDefinitionPropertiess(_model); }
        }

        public IfcSoundPropertiess IfcSoundPropertiess
        {
            get { return new IfcSoundPropertiess(_model); }
        }

        public IfcElementQuantitys IfcElementQuantitys
        {
            get { return new IfcElementQuantitys(_model); }
        }

        public IfcDoorPanelPropertiess IfcDoorPanelPropertiess
        {
            get { return new IfcDoorPanelPropertiess(_model); }
        }

        public IfcSpaceThermalLoadPropertiess IfcSpaceThermalLoadPropertiess
        {
            get { return new IfcSpaceThermalLoadPropertiess(_model); }
        }

        public IfcFluidFlowPropertiess IfcFluidFlowPropertiess
        {
            get { return new IfcFluidFlowPropertiess(_model); }
        }
    }
}