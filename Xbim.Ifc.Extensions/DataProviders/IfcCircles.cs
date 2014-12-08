#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCircles.cs
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
    public class IfcCircles
    {
        private readonly IModel _model;

        public IfcCircles(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcCircle> Items
        {
            get { return this._model.Instances.OfType<IfcCircle>(); }
        }
    }
}