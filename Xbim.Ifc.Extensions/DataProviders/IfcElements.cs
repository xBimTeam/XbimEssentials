#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcElements
    {
        private readonly IModel _model;

        public IfcElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcElement> Items
        {
            get { return this._model.Instances.OfType<IfcElement>(); }
        }

        public IfcBuildingElements IfcBuildingElements
        {
            get { return new IfcBuildingElements(_model); }
        }

        public IfcEquipmentElements IfcEquipmentElements
        {
            get { return new IfcEquipmentElements(_model); }
        }

        public IfcDistributionElements IfcDistributionElements
        {
            get { return new IfcDistributionElements(_model); }
        }

        public IfcFeatureElements IfcFeatureElements
        {
            get { return new IfcFeatureElements(_model); }
        }

        public IfcElementAssemblys IfcElementAssemblys
        {
            get { return new IfcElementAssemblys(_model); }
        }

        public IfcElectricalElements IfcElectricalElements
        {
            get { return new IfcElectricalElements(_model); }
        }

        public IfcTransportElements IfcTransportElements
        {
            get { return new IfcTransportElements(_model); }
        }

        public IfcFurnishingElements IfcFurnishingElements
        {
            get { return new IfcFurnishingElements(_model); }
        }

        public IfcVirtualElements IfcVirtualElements
        {
            get { return new IfcVirtualElements(_model); }
        }
    }
}