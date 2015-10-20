#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelDefiness.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRelDefiness
    {
        private readonly IModel _model;

        public IfcRelDefiness(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelDefines> Items
        {
            get { return this._model.Instances.OfType<IfcRelDefines>(); }
        }

        public IfcRelDefinesByPropertiess IfcRelDefinesByPropertiess
        {
            get { return new IfcRelDefinesByPropertiess(_model); }
        }

        public IfcRelDefinesByTypes IfcRelDefinesByTypes
        {
            get { return new IfcRelDefinesByTypes(_model); }
        }
    }
}