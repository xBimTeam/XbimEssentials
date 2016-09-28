#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaces.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSurfaces
    {
        private readonly IModel _model;

        public IfcSurfaces(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSurface> Items
        {
            get { return this._model.Instances.OfType<IfcSurface>(); }
        }

        public IfcElementarySurfaces IfcElementarySurfaces
        {
            get { return new IfcElementarySurfaces(_model); }
        }

        public IfcSweptSurfaces IfcSweptSurfaces
        {
            get { return new IfcSweptSurfaces(_model); }
        }

        public IfcBoundedSurfaces IfcBoundedSurfaces
        {
            get { return new IfcBoundedSurfaces(_model); }
        }
    }
}