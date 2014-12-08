#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundaryConditions.cs
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
    public class IfcBoundaryConditions
    {
        private readonly IModel _model;

        public IfcBoundaryConditions(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBoundaryCondition> Items
        {
            get { return this._model.Instances.OfType<IfcBoundaryCondition>(); }
        }

        public IfcBoundaryNodeConditions IfcBoundaryNodeConditions
        {
            get { return new IfcBoundaryNodeConditions(_model); }
        }

        public IfcBoundaryFaceConditions IfcBoundaryFaceConditions
        {
            get { return new IfcBoundaryFaceConditions(_model); }
        }

        public IfcBoundaryEdgeConditions IfcBoundaryEdgeConditions
        {
            get { return new IfcBoundaryEdgeConditions(_model); }
        }
    }
}