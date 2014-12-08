#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralConnectionConditions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.StructuralLoadResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralConnectionConditions
    {
        private readonly IModel _model;

        public IfcStructuralConnectionConditions(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralConnectionCondition> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralConnectionCondition>(); }
        }

        public IfcFailureConnectionConditions IfcFailureConnectionConditions
        {
            get { return new IfcFailureConnectionConditions(_model); }
        }

        public IfcSlippageConnectionConditions IfcSlippageConnectionConditions
        {
            get { return new IfcSlippageConnectionConditions(_model); }
        }
    }
}