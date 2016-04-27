#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGroups.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcGroups
    {
        private readonly IModel _model;

        public IfcGroups(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcGroup> Items
        {
            get { return this._model.Instances.OfType<IfcGroup>(); }
        }

        public IfcZones IfcZones
        {
            get { return new IfcZones(_model); }
        }

        public IfcStructuralLoadGroups IfcStructuralLoadGroups
        {
            get { return new IfcStructuralLoadGroups(_model); }
        }

        public IfcSystems IfcSystems
        {
            get { return new IfcSystems(_model); }
        }

        public IfcStructuralResultGroups IfcStructuralResultGroups
        {
            get { return new IfcStructuralResultGroups(_model); }
        }
    }
}