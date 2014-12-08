#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcObjects.cs
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
    public class IfcObjects
    {
        private readonly IModel _model;

        public IfcObjects(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcObject> Items
        {
            get { return this._model.Instances.OfType<IfcObject>(); }
        }

        public IfcProducts IfcProducts
        {
            get { return new IfcProducts(_model); }
        }

        public IfcGroups IfcGroups
        {
            get { return new IfcGroups(_model); }
        }

        public IfcProjects IfcProjects
        {
            get { return new IfcProjects(_model); }
        }

        public IfcActors IfcActors
        {
            get { return new IfcActors(_model); }
        }
    }
}