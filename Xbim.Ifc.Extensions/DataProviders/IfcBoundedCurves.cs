#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundedCurves.cs
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
    public class IfcBoundedCurves
    {
        private readonly IModel _model;

        public IfcBoundedCurves(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBoundedCurve> Items
        {
            get { return this._model.Instances.OfType<IfcBoundedCurve>(); }
        }

        public IfcPolylines IfcPolylines
        {
            get { return new IfcPolylines(_model); }
        }

        public IfcCompositeCurves IfcCompositeCurves
        {
            get { return new IfcCompositeCurves(_model); }
        }

        public IfcTrimmedCurves IfcTrimmedCurves
        {
            get { return new IfcTrimmedCurves(_model); }
        }

        public IfcBSplineCurves IfcBSplineCurves
        {
            get { return new IfcBSplineCurves(_model); }
        }
    }
}