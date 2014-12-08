#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTopologicalRepresentationItems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.TopologyResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcTopologicalRepresentationItems
    {
        private readonly IModel _model;

        public IfcTopologicalRepresentationItems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcTopologicalRepresentationItem> Items
        {
            get { return this._model.Instances.OfType<IfcTopologicalRepresentationItem>(); }
        }

        public IfcLoops IfcLoops
        {
            get { return new IfcLoops(_model); }
        }

        public IfcVertexs IfcVertexs
        {
            get { return new IfcVertexs(_model); }
        }

        public IfcConnectedFaceSets IfcConnectedFaceSets
        {
            get { return new IfcConnectedFaceSets(_model); }
        }

        public IfcEdges IfcEdges
        {
            get { return new IfcEdges(_model); }
        }

        public IfcFaceBounds IfcFaceBounds
        {
            get { return new IfcFaceBounds(_model); }
        }

        public IfcFaces IfcFaces
        {
            get { return new IfcFaces(_model); }
        }
    }
}