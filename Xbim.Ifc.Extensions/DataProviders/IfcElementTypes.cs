#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementTypes.cs
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
    public class IfcElementTypes
    {
        private readonly IModel _model;

        public IfcElementTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcElementType> Items
        {
            get { return this._model.Instances.OfType<IfcElementType>(); }
        }

        public IfcFurnishingElementTypes IfcFurnishingElementTypes
        {
            get { return new IfcFurnishingElementTypes(_model); }
        }

        public IfcElementComponentTypes IfcElementComponentTypes
        {
            get { return new IfcElementComponentTypes(_model); }
        }

        public IfcDistributionElementTypes IfcDistributionElementTypes
        {
            get { return new IfcDistributionElementTypes(_model); }
        }

        public IfcBuildingElementTypes IfcBuildingElementTypes
        {
            get { return new IfcBuildingElementTypes(_model); }
        }

        public IfcSpatialStructureElementTypes IfcSpatialStructureElementTypes
        {
            get { return new IfcSpatialStructureElementTypes(_model); }
        }
    }
}