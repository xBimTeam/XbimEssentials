#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClassificationNotations.cs
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
    public class IfcClassificationNotations
    {
        private readonly IModel _model;

        public IfcClassificationNotations(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcClassificationNotation> Items
        {
            get { return this._model.Instances.OfType<IfcClassificationNotation>(); }
        }
    }
}