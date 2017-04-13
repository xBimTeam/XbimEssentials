#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementComponentTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.SharedComponentElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcElementComponentTypes
    {
        private readonly IModel _model;

        public IfcElementComponentTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcElementComponentType> Items
        {
            get { return this._model.Instances.OfType<IfcElementComponentType>(); }
        }

        public IfcDiscreteAccessoryTypes IfcDiscreteAccessoryTypes
        {
            get { return new IfcDiscreteAccessoryTypes(_model); }
        }
    }
}