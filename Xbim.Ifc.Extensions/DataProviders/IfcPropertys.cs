#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertys.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.PropertyResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPropertys
    {
        private readonly IModel _model;

        public IfcPropertys(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcProperty> Items
        {
            get { return this._model.Instances.OfType<IfcProperty>(); }
        }

        public IfcSimplePropertys IfcSimplePropertys
        {
            get { return new IfcSimplePropertys(_model); }
        }

        public IfcComplexPropertys IfcComplexPropertys
        {
            get { return new IfcComplexPropertys(_model); }
        }
    }
}