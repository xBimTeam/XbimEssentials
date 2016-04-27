#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSlabTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.SharedBldgElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSlabTypes
    {
        private readonly IModel _model;

        public IfcSlabTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSlabType> Items
        {
            get { return this._model.Instances.OfType<IfcSlabType>(); }
        }
    }
}