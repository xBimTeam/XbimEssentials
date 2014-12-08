#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpaces.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSpaces
    {
        private readonly IModel _model;

        public IfcSpaces(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSpace> Items
        {
            get { return this._model.Instances.OfType<IfcSpace>(); }
        }
    }
}