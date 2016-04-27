#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignsToResources.cs
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
    public class IfcRelAssignsToResources
    {
        private readonly IModel _model;

        public IfcRelAssignsToResources(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelAssignsToResource> Items
        {
            get { return this._model.Instances.OfType<IfcRelAssignsToResource>(); }
        }
    }
}