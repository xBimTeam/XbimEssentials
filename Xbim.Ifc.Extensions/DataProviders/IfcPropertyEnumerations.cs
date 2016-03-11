#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyEnumerations.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.PropertyResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPropertyEnumerations
    {
        private readonly IModel _model;

        public IfcPropertyEnumerations(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPropertyEnumeration> Items
        {
            get { return this._model.Instances.OfType<IfcPropertyEnumeration>(); }
        }
    }
}