#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSolidModels.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.GeometricModelResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcSolidModels
    {
        private readonly IModel _model;

        public IfcSolidModels(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSolidModel> Items
        {
            get { return this._model.Instances.OfType<IfcSolidModel>(); }
        }

        public IfcSweptDiskSolids IfcSweptDiskSolids
        {
            get { return new IfcSweptDiskSolids(_model); }
        }

        public IfcCsgSolids IfcCsgSolids
        {
            get { return new IfcCsgSolids(_model); }
        }

        public IfcSweptAreaSolids IfcSweptAreaSolids
        {
            get { return new IfcSweptAreaSolids(_model); }
        }

        public IfcManifoldSolidBreps IfcManifoldSolidBreps
        {
            get { return new IfcManifoldSolidBreps(_model); }
        }
    }
}