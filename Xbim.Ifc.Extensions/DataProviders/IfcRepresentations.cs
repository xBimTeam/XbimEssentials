#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRepresentations.cs
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
    public class IfcRepresentations
    {
        private readonly IModel _model;

        public IfcRepresentations(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRepresentation> Items
        {
            get { return this._model.Instances.OfType<IfcRepresentation>(); }
        }

        public IfcShapeModels IfcShapeModels
        {
            get { return new IfcShapeModels(_model); }
        }

        public IfcStyleModels IfcStyleModels
        {
            get { return new IfcStyleModels(_model); }
        }
    }
}