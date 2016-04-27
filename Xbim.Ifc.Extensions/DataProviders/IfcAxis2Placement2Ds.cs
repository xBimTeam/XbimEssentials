#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAxis2Placement2Ds.cs
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
    public class IfcAxis2Placement2Ds
    {
        private readonly IModel _model;

        public IfcAxis2Placement2Ds(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcAxis2Placement2D> Items
        {
            get { return this._model.Instances.OfType<IfcAxis2Placement2D>(); }
        }
    }
}