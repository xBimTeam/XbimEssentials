#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignss.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRelAssignss
    {
        private readonly IModel _model;

        public IfcRelAssignss(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelAssigns> Items
        {
            get { return this._model.Instances.OfType<IfcRelAssigns>(); }
        }

        public IfcRelAssignsToProducts IfcRelAssignsToProducts
        {
            get { return new IfcRelAssignsToProducts(_model); }
        }

        public IfcRelAssignsToGroups IfcRelAssignsToGroups
        {
            get { return new IfcRelAssignsToGroups(_model); }
        }

        public IfcRelAssignsToResources IfcRelAssignsToResources
        {
            get { return new IfcRelAssignsToResources(_model); }
        }

        public IfcRelAssignsToControls IfcRelAssignsToControls
        {
            get { return new IfcRelAssignsToControls(_model); }
        }

        public IfcRelAssignsToActors IfcRelAssignsToActors
        {
            get { return new IfcRelAssignsToActors(_model); }
        }

        public IfcRelAssignsToProcesss IfcRelAssignsToProcesss
        {
            get { return new IfcRelAssignsToProcesss(_model); }
        }
    }
}