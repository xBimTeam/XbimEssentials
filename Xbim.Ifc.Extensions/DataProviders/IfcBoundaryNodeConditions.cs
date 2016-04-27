#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundaryNodeConditions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralLoadResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcBoundaryNodeConditions
    {
        private readonly IModel _model;

        public IfcBoundaryNodeConditions(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBoundaryNodeCondition> Items
        {
            get { return this._model.Instances.OfType<IfcBoundaryNodeCondition>(); }
        }

        public IfcBoundaryNodeConditionWarpings IfcBoundaryNodeConditionWarpings
        {
            get { return new IfcBoundaryNodeConditionWarpings(_model); }
        }
    }
}