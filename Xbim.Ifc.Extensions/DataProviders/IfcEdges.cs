#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEdges.cs
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
    public class IfcEdges
    {
        private readonly IModel _model;

        public IfcEdges(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcEdge> Items
        {
            get { return this._model.Instances.OfType<IfcEdge>(); }
        }

        public IfcOrientedEdges IfcOrientedEdges
        {
            get { return new IfcOrientedEdges(_model); }
        }

        public IfcSubedges IfcSubedges
        {
            get { return new IfcSubedges(_model); }
        }

        public IfcEdgeCurves IfcEdgeCurves
        {
            get { return new IfcEdgeCurves(_model); }
        }
    }
}