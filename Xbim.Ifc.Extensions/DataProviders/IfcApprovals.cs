#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcApprovals.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ApprovalResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcApprovals
    {
        private readonly IModel _model;

        public IfcApprovals(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcApproval> Items
        {
            get { return this._model.Instances.OfType<IfcApproval>(); }
        }
    }
}