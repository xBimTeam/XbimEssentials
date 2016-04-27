#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralItems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralAnalysisDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralItems
    {
        private readonly IModel _model;

        public IfcStructuralItems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralItem> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralItem>(); }
        }

        public IfcStructuralMembers IfcStructuralMembers
        {
            get { return new IfcStructuralMembers(_model); }
        }

        public IfcStructuralConnections IfcStructuralConnections
        {
            get { return new IfcStructuralConnections(_model); }
        }
    }
}