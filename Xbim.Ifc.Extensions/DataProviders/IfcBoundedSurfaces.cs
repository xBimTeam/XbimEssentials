#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundedSurfaces.cs
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
    public class IfcBoundedSurfaces
    {
        private readonly IModel _model;

        public IfcBoundedSurfaces(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBoundedSurface> Items
        {
            get { return this._model.Instances.OfType<IfcBoundedSurface>(); }
        }

        public IfcRectangularTrimmedSurfaces IfcRectangularTrimmedSurfaces
        {
            get { return new IfcRectangularTrimmedSurfaces(_model); }
        }

        public IfcCurveBoundedPlanes IfcCurveBoundedPlanes
        {
            get { return new IfcCurveBoundedPlanes(_model); }
        }
    }
}