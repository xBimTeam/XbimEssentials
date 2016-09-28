#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcArbitraryClosedProfileDefs.cs
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
    public class IfcArbitraryClosedProfileDefs
    {
        private readonly IModel _model;

        public IfcArbitraryClosedProfileDefs(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcArbitraryClosedProfileDef> Items
        {
            get { return this._model.Instances.OfType<IfcArbitraryClosedProfileDef>(); }
        }

        public IfcArbitraryProfileDefWithVoidss IfcArbitraryProfileDefWithVoidss
        {
            get { return new IfcArbitraryProfileDefWithVoidss(_model); }
        }
    }
}