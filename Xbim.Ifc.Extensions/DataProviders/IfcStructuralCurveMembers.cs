#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralCurveMembers.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralAnalysisDomain;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralCurveMembers
    {
        private readonly IModel _model;

        public IfcStructuralCurveMembers(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralCurveMember> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralCurveMember>(); }
        }

        public IfcStructuralCurveMemberVaryings IfcStructuralCurveMemberVaryings
        {
            get { return new IfcStructuralCurveMemberVaryings(_model); }
        }
    }
}