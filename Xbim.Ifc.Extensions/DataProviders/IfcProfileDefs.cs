#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProfileDefs.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ProfileResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcProfileDefs
    {
        private readonly IModel _model;

        public IfcProfileDefs(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcProfileDef> Items
        {
            get { return this._model.Instances.OfType<IfcProfileDef>(); }
        }

        public IfcParameterizedProfileDefs IfcParameterizedProfileDefs
        {
            get { return new IfcParameterizedProfileDefs(_model); }
        }

        public IfcArbitraryClosedProfileDefs IfcArbitraryClosedProfileDefs
        {
            get { return new IfcArbitraryClosedProfileDefs(_model); }
        }

        public IfcDerivedProfileDefs IfcDerivedProfileDefs
        {
            get { return new IfcDerivedProfileDefs(_model); }
        }

        public IfcArbitraryOpenProfileDefs IfcArbitraryOpenProfileDefs
        {
            get { return new IfcArbitraryOpenProfileDefs(_model); }
        }

        public IfcCompositeProfileDefs IfcCompositeProfileDefs
        {
            get { return new IfcCompositeProfileDefs(_model); }
        }
    }
}