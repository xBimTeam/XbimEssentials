#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTimeSeriess.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.TimeSeriesResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcTimeSeriess
    {
        private readonly IModel _model;

        public IfcTimeSeriess(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcTimeSeries> Items
        {
            get { return this._model.Instances.OfType<IfcTimeSeries>(); }
        }

        public IfcRegularTimeSeriess IfcRegularTimeSeriess
        {
            get { return new IfcRegularTimeSeriess(_model); }
        }

        public IfcIrregularTimeSeriess IfcIrregularTimeSeriess
        {
            get { return new IfcIrregularTimeSeriess(_model); }
        }
    }
}