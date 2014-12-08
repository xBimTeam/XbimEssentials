#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClassifications.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ExternalReferenceResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcClassifications
    {
        private readonly IModel _model;

        public IfcClassifications(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcClassification> Items
        {
            get { return this._model.Instances.OfType<IfcClassification>(); }
        }
    }
}