#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcingElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralElementsDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcReinforcingElements
    {
        private readonly IModel _model;

        public IfcReinforcingElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcReinforcingElement> Items
        {
            get { return this._model.Instances.OfType<IfcReinforcingElement>(); }
        }

        public IfcTendonAnchors IfcTendonAnchors
        {
            get { return new IfcTendonAnchors(_model); }
        }

        public IfcReinforcingBars IfcReinforcingBars
        {
            get { return new IfcReinforcingBars(_model); }
        }

        public IfcTendons IfcTendons
        {
            get { return new IfcTendons(_model); }
        }

        public IfcReinforcingMeshs IfcReinforcingMeshs
        {
            get { return new IfcReinforcingMeshs(_model); }
        }
    }
}