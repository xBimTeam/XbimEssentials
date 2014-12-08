#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyleShadings.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.PresentationAppearanceResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSurfaceStyleShadings
    {
        private readonly IModel _model;

        public IfcSurfaceStyleShadings(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSurfaceStyleShading> Items
        {
            get { return this._model.Instances.OfType<IfcSurfaceStyleShading>(); }
        }

        public IfcSurfaceStyleRenderings IfcSurfaceStyleRenderings
        {
            get { return new IfcSurfaceStyleRenderings(_model); }
        }
    }
}