#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyDefinitions.cs
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
    public class IfcPropertyDefinitions
    {
        private readonly IModel _model;

        public IfcPropertyDefinitions(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPropertyDefinition> Items
        {
            get { return this._model.Instances.OfType<IfcPropertyDefinition>(); }
        }

        public IfcPropertySetDefinitions IfcPropertySetDefinitions
        {
            get { return new IfcPropertySetDefinitions(_model); }
        }
    }
}