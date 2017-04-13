#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadStatics.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.StructuralLoadResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcStructuralLoadStatics
    {
        private readonly IModel _model;

        public IfcStructuralLoadStatics(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcStructuralLoadStatic> Items
        {
            get { return this._model.Instances.OfType<IfcStructuralLoadStatic>(); }
        }

        public IfcStructuralLoadSingleDisplacements IfcStructuralLoadSingleDisplacements
        {
            get { return new IfcStructuralLoadSingleDisplacements(_model); }
        }

        public IfcStructuralLoadTemperatures IfcStructuralLoadTemperatures
        {
            get { return new IfcStructuralLoadTemperatures(_model); }
        }

        public IfcStructuralLoadSingleForces IfcStructuralLoadSingleForces
        {
            get { return new IfcStructuralLoadSingleForces(_model); }
        }

        public IfcStructuralLoadPlanarForces IfcStructuralLoadPlanarForces
        {
            get { return new IfcStructuralLoadPlanarForces(_model); }
        }

        public IfcStructuralLoadLinearForces IfcStructuralLoadLinearForces
        {
            get { return new IfcStructuralLoadLinearForces(_model); }
        }
    }
}