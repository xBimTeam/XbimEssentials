#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralMembers.cs
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
    public class IfcStructuralMembers
    {
        private readonly IModel _model;

        public IfcStructuralMembers(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralMember> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralMember>(); }
        }

        public IfcStructuralCurveMembers IfcStructuralCurveMembers
        {
            get { return new IfcStructuralCurveMembers(_model); }
        }

        public IfcStructuralSurfaceMembers IfcStructuralSurfaceMembers
        {
            get { return new IfcStructuralSurfaceMembers(_model); }
        }
    }
}