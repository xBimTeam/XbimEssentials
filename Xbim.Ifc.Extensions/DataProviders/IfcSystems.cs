#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSystems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSystems
    {
        private readonly IModel _model;

        public IfcSystems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSystem> Items
        {
            get { return this._model.Instances.OfType<IfcSystem>(); }
        }

        public IfcStructuralAnalysisModels IfcStructuralAnalysisModels
        {
            get { return new IfcStructuralAnalysisModels(_model); }
        }

        public IfcElectricalCircuits IfcElectricalCircuits
        {
            get { return new IfcElectricalCircuits(_model); }
        }
    }
}