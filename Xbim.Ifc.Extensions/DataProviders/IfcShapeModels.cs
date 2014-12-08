#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcShapeModels.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.RepresentationResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcShapeModels
    {
        private readonly IModel _model;

        public IfcShapeModels(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcShapeModel> Items
        {
            get { return this._model.Instances.OfType<IfcShapeModel>(); }
        }

        public IfcTopologyRepresentations IfcTopologyRepresentations
        {
            get { return new IfcTopologyRepresentations(_model); }
        }

        public IfcShapeRepresentations IfcShapeRepresentations
        {
            get { return new IfcShapeRepresentations(_model); }
        }
    }
}