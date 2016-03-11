#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRampFlights.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.SharedBldgElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRampFlights
    {
        private readonly IModel _model;

        public IfcRampFlights(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRampFlight> Items
        {
            get { return this._model.Instances.OfType<IfcRampFlight>(); }
        }
    }
}