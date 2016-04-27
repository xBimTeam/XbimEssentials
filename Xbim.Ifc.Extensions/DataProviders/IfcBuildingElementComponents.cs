#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingElementComponents.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralElementsDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcBuildingElementComponents
    {
        private readonly IModel _model;

        public IfcBuildingElementComponents(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBuildingElementComponent> Items
        {
            get { return this._model.Instances.OfType<IfcBuildingElementComponent>(); }
        }

        public IfcReinforcingElements IfcReinforcingElements
        {
            get { return new IfcReinforcingElements(_model); }
        }

        public IfcBuildingElementParts IfcBuildingElementParts
        {
            get { return new IfcBuildingElementParts(_model); }
        }
    }
}