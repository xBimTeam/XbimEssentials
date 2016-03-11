#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralSurfaceConnections.cs
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
    public class IfcStructuralSurfaceConnections
    {
        private readonly IModel _model;

        public IfcStructuralSurfaceConnections(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralSurfaceConnection> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralSurfaceConnection>(); }
        }
    }
}