#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralConnections.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralAnalysisDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralConnections
    {
        private readonly IModel _model;

        public IfcStructuralConnections(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralConnection> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralConnection>(); }
        }

        public IfcStructuralSurfaceConnections IfcStructuralSurfaceConnections
        {
            get { return new IfcStructuralSurfaceConnections(_model); }
        }

        public IfcStructuralPointConnections IfcStructuralPointConnections
        {
            get { return new IfcStructuralPointConnections(_model); }
        }

        public IfcStructuralCurveConnections IfcStructuralCurveConnections
        {
            get { return new IfcStructuralCurveConnections(_model); }
        }
    }
}