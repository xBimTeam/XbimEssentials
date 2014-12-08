#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionChamberElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.SharedBldgServiceElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcDistributionChamberElements
    {
        private readonly IModel _model;

        public IfcDistributionChamberElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcDistributionChamberElement> Items
        {
            get { return this._model.Instances.OfType<IfcDistributionChamberElement>(); }
        }
    }
}