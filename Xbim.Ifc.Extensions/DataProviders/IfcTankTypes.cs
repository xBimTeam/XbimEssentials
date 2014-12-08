#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTankTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.HVACDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcTankTypes
    {
        private readonly IModel _model;

        public IfcTankTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcTankType> Items
        {
            get { return this._model.Instances.OfType<IfcTankType>(); }
        }
    }
}