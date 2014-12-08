#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionElements.cs
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
    public class IfcDistributionElements
    {
        private readonly IModel _model;

        public IfcDistributionElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcDistributionElement> Items
        {
            get { return this._model.Instances.OfType<IfcDistributionElement>(); }
        }

        public IfcDistributionFlowElements IfcDistributionFlowElements
        {
            get { return new IfcDistributionFlowElements(_model); }
        }

        public IfcDistributionControlElements IfcDistributionControlElements
        {
            get { return new IfcDistributionControlElements(_model); }
        }
    }
}