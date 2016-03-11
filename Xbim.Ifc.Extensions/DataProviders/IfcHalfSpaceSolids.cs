#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcHalfSpaceSolids.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.GeometricModelResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcHalfSpaceSolids
    {
        private readonly IModel _model;

        public IfcHalfSpaceSolids(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcHalfSpaceSolid> Items
        {
            get { return this._model.Instances.OfType<IfcHalfSpaceSolid>(); }
        }

        public IfcPolygonalBoundedHalfSpaces IfcPolygonalBoundedHalfSpaces
        {
            get { return new IfcPolygonalBoundedHalfSpaces(_model); }
        }

        public IfcBoxedHalfSpaces IfcBoxedHalfSpaces
        {
            get { return new IfcBoxedHalfSpaces(_model); }
        }
    }
}