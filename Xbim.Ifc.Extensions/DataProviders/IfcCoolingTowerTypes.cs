#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCoolingTowerTypes.cs
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
    public class IfcCoolingTowerTypes
    {
        private readonly IModel _model;

        public IfcCoolingTowerTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcCoolingTowerType> Items
        {
            get { return this._model.Instances.OfType<IfcCoolingTowerType>(); }
        }
    }
}