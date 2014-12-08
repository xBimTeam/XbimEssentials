#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpatialStructureElements.cs
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
    public class IfcSpatialStructureElements
    {
        private readonly IModel _model;

        public IfcSpatialStructureElements(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcSpatialStructureElement> Items
        {
            get { return this._model.Instances.OfType<IfcSpatialStructureElement>(); }
        }

        public IfcBuildingStoreys IfcBuildingStoreys
        {
            get { return new IfcBuildingStoreys(_model); }
        }

        public IfcSpaces IfcSpaces
        {
            get { return new IfcSpaces(_model); }
        }

        public IfcSites IfcSites
        {
            get { return new IfcSites(_model); }
        }

        public IfcBuildings IfcBuildings
        {
            get { return new IfcBuildings(_model); }
        }
    }
}