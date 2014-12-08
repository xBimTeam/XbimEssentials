#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRepresentationItems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRepresentationItems
    {
        private readonly IModel _model;

        public IfcRepresentationItems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRepresentationItem> Items
        {
            get { return this._model.Instances.OfType<IfcRepresentationItem>(); }
        }

        public IfcGeometricRepresentationItems IfcGeometricRepresentationItems
        {
            get { return new IfcGeometricRepresentationItems(_model); }
        }

        public IfcStyledItems IfcStyledItems
        {
            get { return new IfcStyledItems(_model); }
        }

        public IfcTopologicalRepresentationItems IfcTopologicalRepresentationItems
        {
            get { return new IfcTopologicalRepresentationItems(_model); }
        }

        public IfcAnnotationFillAreas IfcAnnotationFillAreas
        {
            get { return new IfcAnnotationFillAreas(_model); }
        }

        public IfcMappedItems IfcMappedItems
        {
            get { return new IfcMappedItems(_model); }
        }
    }
}