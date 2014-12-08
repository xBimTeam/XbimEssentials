#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSweptSurfaces.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSweptSurfaces
    {
        private readonly IModel _model;

        public IfcSweptSurfaces(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSweptSurface> Items
        {
            get { return this._model.Instances.OfType<IfcSweptSurface>(); }
        }

        public IfcSurfaceOfRevolutions IfcSurfaceOfRevolutions
        {
            get { return new IfcSurfaceOfRevolutions(_model); }
        }

        public IfcSurfaceOfLinearExtrusions IfcSurfaceOfLinearExtrusions
        {
            get { return new IfcSurfaceOfLinearExtrusions(_model); }
        }
    }
}