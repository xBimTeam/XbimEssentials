#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlacements.cs
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
    public class IfcPlacements
    {
        private readonly IModel _model;

        public IfcPlacements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPlacement> Items
        {
            get { return this._model.Instances.OfType<IfcPlacement>(); }
        }

        public IfcAxis2Placement2Ds IfcAxis2Placement2Ds
        {
            get { return new IfcAxis2Placement2Ds(_model); }
        }

        public IfcAxis2Placement3Ds IfcAxis2Placement3Ds
        {
            get { return new IfcAxis2Placement3Ds(_model); }
        }

        public IfcAxis1Placements IfcAxis1Placements
        {
            get { return new IfcAxis1Placements(_model); }
        }
    }
}