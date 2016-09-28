#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFeatureElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcFeatureElements
    {
        private readonly IModel _model;

        public IfcFeatureElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFeatureElement> Items
        {
            get { return this._model.Instances.OfType<IfcFeatureElement>(); }
        }

        public IfcFeatureElementSubtractions IfcFeatureElementSubtractions
        {
            get { return new IfcFeatureElementSubtractions(_model); }
        }

        public IfcFeatureElementAdditions IfcFeatureElementAdditions
        {
            get { return new IfcFeatureElementAdditions(_model); }
        }
    }
}