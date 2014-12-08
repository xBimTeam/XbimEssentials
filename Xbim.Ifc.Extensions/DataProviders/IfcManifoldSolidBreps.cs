#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcManifoldSolidBreps.cs
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
    public class IfcManifoldSolidBreps
    {
        private readonly IModel _model;

        public IfcManifoldSolidBreps(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcManifoldSolidBrep> Items
        {
            get { return this._model.Instances.OfType<IfcManifoldSolidBrep>(); }
        }

        public IfcFacetedBreps IfcFacetedBreps
        {
            get { return new IfcFacetedBreps(_model); }
        }

        public IfcFacetedBrepWithVoidss IfcFacetedBrepWithVoidss
        {
            get { return new IfcFacetedBrepWithVoidss(_model); }
        }
    }
}