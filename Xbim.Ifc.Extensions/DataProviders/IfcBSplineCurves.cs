#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBSplineCurves.cs
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
    public class IfcBSplineCurves
    {
        private readonly IModel _model;

        public IfcBSplineCurves(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBSplineCurve> Items
        {
            get { return this._model.Instances.OfType<IfcBSplineCurve>(); }
        }

        public IfcBezierCurves IfcBezierCurves
        {
            get { return new IfcBezierCurves(_model); }
        }
    }
}