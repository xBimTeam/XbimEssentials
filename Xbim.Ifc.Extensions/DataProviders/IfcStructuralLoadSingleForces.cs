#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadSingleForces.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.StructuralLoadResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralLoadSingleForces
    {
        private readonly IModel _model;

        public IfcStructuralLoadSingleForces(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralLoadSingleForce> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralLoadSingleForce>(); }
        }

        public IfcStructuralLoadSingleForceWarpings IfcStructuralLoadSingleForceWarpings
        {
            get { return new IfcStructuralLoadSingleForceWarpings(_model); }
        }
    }
}