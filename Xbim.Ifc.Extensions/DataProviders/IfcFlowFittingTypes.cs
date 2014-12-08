#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFlowFittingTypes.cs
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
    public class IfcFlowFittingTypes
    {
        private readonly IModel _model;

        public IfcFlowFittingTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFlowFittingType> Items
        {
            get { return this._model.Instances.OfType<IfcFlowFittingType>(); }
        }

        public IfcCableCarrierFittingTypes IfcCableCarrierFittingTypes
        {
            get { return new IfcCableCarrierFittingTypes(_model); }
        }

        public IfcPipeFittingTypes IfcPipeFittingTypes
        {
            get { return new IfcPipeFittingTypes(_model); }
        }

        public IfcDuctFittingTypes IfcDuctFittingTypes
        {
            get { return new IfcDuctFittingTypes(_model); }
        }

        public IfcJunctionBoxTypes IfcJunctionBoxTypes
        {
            get { return new IfcJunctionBoxTypes(_model); }
        }
    }
}