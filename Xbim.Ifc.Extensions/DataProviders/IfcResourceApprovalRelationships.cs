#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcResourceApprovalRelationships.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcResourceApprovalRelationships
    {
        private readonly IModel _model;

        public IfcResourceApprovalRelationships(IModel model)
        {
            this._model = model;
        }

        //TODO: MC: Need to review with SRL. Commented out during refactoring. This is from IFC4 so it shouldn't be in IFC2x3 specific code.
        //public IEnumerable<IfcResourceApprovalRelationship> Items
        //{
        //    get { return this._model.Instances.OfType<IfcResourceApprovalRelationship>(); }
        //}
    }
}