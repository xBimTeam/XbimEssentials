#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLoops.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.TopologyResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcLoops
    {
        private readonly IModel _model;

        public IfcLoops(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcLoop> Items
        {
            get { return this._model.Instances.OfType<IfcLoop>(); }
        }

        public IfcEdgeLoops IfcEdgeLoops
        {
            get { return new IfcEdgeLoops(_model); }
        }

        public IfcVertexLoops IfcVertexLoops
        {
            get { return new IfcVertexLoops(_model); }
        }

        public IfcPolyLoops IfcPolyLoops
        {
            get { return new IfcPolyLoops(_model); }
        }
    }
}