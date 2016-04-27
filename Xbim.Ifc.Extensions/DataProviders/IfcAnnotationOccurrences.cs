#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotationOccurrences.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.PresentationDefinitionResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcAnnotationOccurrences
    {
        private readonly IModel _model;

        public IfcAnnotationOccurrences(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcAnnotationOccurrence> Items
        {
            get { return this._model.Instances.OfType<IfcAnnotationOccurrence>(); }
        }

        public IfcAnnotationCurveOccurrences IfcAnnotationCurveOccurrences
        {
            get { return new IfcAnnotationCurveOccurrences(_model); }
        }

        public IfcAnnotationSymbolOccurrences IfcAnnotationSymbolOccurrences
        {
            get { return new IfcAnnotationSymbolOccurrences(_model); }
        }

        public IfcAnnotationTextOccurrences IfcAnnotationTextOccurrences
        {
            get { return new IfcAnnotationTextOccurrences(_model); }
        }

        public IfcAnnotationFillAreaOccurrences IfcAnnotationFillAreaOccurrences
        {
            get { return new IfcAnnotationFillAreaOccurrences(_model); }
        }
    }
}