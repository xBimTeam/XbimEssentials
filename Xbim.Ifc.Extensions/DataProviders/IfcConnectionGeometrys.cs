#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionGeometrys.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.GeometricConstraintResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcConnectionGeometrys
    {
        private readonly IModel _model;

        public IfcConnectionGeometrys(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcConnectionGeometry> Items
        {
            get { return this._model.Instances.OfType<IfcConnectionGeometry>(); }
        }

        public IfcConnectionCurveGeometrys IfcConnectionCurveGeometrys
        {
            get { return new IfcConnectionCurveGeometrys(_model); }
        }

        public IfcConnectionSurfaceGeometrys IfcConnectionSurfaceGeometrys
        {
            get { return new IfcConnectionSurfaceGeometrys(_model); }
        }

        public IfcConnectionPointGeometrys IfcConnectionPointGeometrys
        {
            get { return new IfcConnectionPointGeometrys(_model); }
        }
    }
}