#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPoints.cs
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
    public class IfcPoints
    {
        private readonly IModel _model;

        public IfcPoints(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPoint> Items
        {
            get { return this._model.Instances.OfType<IfcPoint>(); }
        }

        public IfcCartesianPoints IfcCartesianPoints
        {
            get { return new IfcCartesianPoints(_model); }
        }

        public IfcPointOnSurfaces IfcPointOnSurfaces
        {
            get { return new IfcPointOnSurfaces(_model); }
        }

        public IfcPointOnCurves IfcPointOnCurves
        {
            get { return new IfcPointOnCurves(_model); }
        }
    }
}