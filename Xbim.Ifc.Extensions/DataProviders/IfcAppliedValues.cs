#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAppliedValues.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.CostResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcAppliedValues
    {
        private readonly IModel _model;

        public IfcAppliedValues(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcAppliedValue> Items
        {
            get { return this._model.Instances.OfType<IfcAppliedValue>(); }
        }

        public IfcEnvironmentalImpactValues IfcEnvironmentalImpactValues
        {
            get { return new IfcEnvironmentalImpactValues(_model); }
        }

        public IfcCostValues IfcCostValues
        {
            get { return new IfcCostValues(_model); }
        }
    }
}