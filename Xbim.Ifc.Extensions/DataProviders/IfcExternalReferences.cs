#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcExternalReferences.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ExternalReferenceResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcExternalReferences
    {
        private readonly IModel _model;

        public IfcExternalReferences(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcExternalReference> Items
        {
            get { return this._model.Instances.OfType<IfcExternalReference>(); }
        }

        public IfcExternallyDefinedSymbols IfcExternallyDefinedSymbols
        {
            get { return new IfcExternallyDefinedSymbols(_model); }
        }

        public IfcExternallyDefinedTextFonts IfcExternallyDefinedTextFonts
        {
            get { return new IfcExternallyDefinedTextFonts(_model); }
        }

        public IfcLibraryReferences IfcLibraryReferences
        {
            get { return new IfcLibraryReferences(_model); }
        }

        public IfcExternallyDefinedSurfaceStyles IfcExternallyDefinedSurfaceStyles
        {
            get { return new IfcExternallyDefinedSurfaceStyles(_model); }
        }

        public IfcClassificationReferences IfcClassificationReferences
        {
            get { return new IfcClassificationReferences(_model); }
        }

        public IfcDocumentReferences IfcDocumentReferences
        {
            get { return new IfcDocumentReferences(_model); }
        }
    }
}