#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStairFlights.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.SharedBldgElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStairFlights
    {
        private readonly IModel _model;

        public IfcStairFlights(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStairFlight> Items
        {
            get { return this._model.Instances.OfType<IfcStairFlight>(); }
        }
    }
}