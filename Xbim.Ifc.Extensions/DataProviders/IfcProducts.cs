#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProducts.cs
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
    public class IfcProducts
    {
        private readonly IModel _model;

        public IfcProducts(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcProduct> Items
        {
            get { return this._model.Instances.OfType<IfcProduct>(); }
        }

        public IfcElements IfcElements
        {
            get { return new IfcElements(_model); }
        }

        public IfcSpatialStructureElements IfcSpatialStructureElements
        {
            get { return new IfcSpatialStructureElements(_model); }
        }

        public IfcStructuralActivitys IfcStructuralActivitys
        {
            get { return new IfcStructuralActivitys(_model); }
        }

        public IfcProxys IfcProxys
        {
            get { return new IfcProxys(_model); }
        }

        public IfcStructuralItems IfcStructuralItems
        {
            get { return new IfcStructuralItems(_model); }
        }

        public IfcAnnotations IfcAnnotations
        {
            get { return new IfcAnnotations(_model); }
        }

        public IfcPorts IfcPorts
        {
            get { return new IfcPorts(_model); }
        }

        public IfcGrids IfcGrids
        {
            get { return new IfcGrids(_model); }
        }
    }
}