#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCurves.cs
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
    public class IfcCurves
    {
        private readonly IModel _model;

        public IfcCurves(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcCurve> Items
        {
            get { return this._model.Instances.OfType<IfcCurve>(); }
        }

        public IfcConics IfcConics
        {
            get { return new IfcConics(_model); }
        }

        public IfcLines IfcLines
        {
            get { return new IfcLines(_model); }
        }

        public IfcBoundedCurves IfcBoundedCurves
        {
            get { return new IfcBoundedCurves(_model); }
        }

        public IfcOffsetCurve2Ds IfcOffsetCurve2Ds
        {
            get { return new IfcOffsetCurve2Ds(_model); }
        }

        public IfcOffsetCurve3Ds IfcOffsetCurve3Ds
        {
            get { return new IfcOffsetCurve3Ds(_model); }
        }
    }
}