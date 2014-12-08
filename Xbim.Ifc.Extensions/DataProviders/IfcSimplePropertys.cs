#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSimplePropertys.cs
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
    public class IfcSimplePropertys
    {
        private readonly IModel _model;

        public IfcSimplePropertys(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSimpleProperty> Items
        {
            get { return this._model.Instances.OfType<IfcSimpleProperty>(); }
        }

        public IfcPropertySingleValues IfcPropertySingleValues
        {
            get { return new IfcPropertySingleValues(_model); }
        }

        public IfcPropertyTableValues IfcPropertyTableValues
        {
            get { return new IfcPropertyTableValues(_model); }
        }

        public IfcPropertyBoundedValues IfcPropertyBoundedValues
        {
            get { return new IfcPropertyBoundedValues(_model); }
        }

        public IfcPropertyListValues IfcPropertyListValues
        {
            get { return new IfcPropertyListValues(_model); }
        }

        public IfcPropertyReferenceValues IfcPropertyReferenceValues
        {
            get { return new IfcPropertyReferenceValues(_model); }
        }

        public IfcPropertyEnumeratedValues IfcPropertyEnumeratedValues
        {
            get { return new IfcPropertyEnumeratedValues(_model); }
        }
    }
}