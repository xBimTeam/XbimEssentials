#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAddresss.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ActorResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcAddresss
    {
        private readonly IModel _model;

        public IfcAddresss(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcAddress> Items
        {
            get { return this._model.Instances.OfType<IfcAddress>(); }
        }

        public IfcTelecomAddresss IfcTelecomAddresss
        {
            get { return new IfcTelecomAddresss(_model); }
        }

        public IfcPostalAddresss IfcPostalAddresss
        {
            get { return new IfcPostalAddresss(_model); }
        }
    }
}