#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPreDefinedItems.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.PresentationResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPreDefinedItems
    {
        private readonly IModel _model;

        public IfcPreDefinedItems(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPreDefinedItem> Items
        {
            get { return this._model.Instances.OfType<IfcPreDefinedItem>(); }
        }

        public IfcPreDefinedColours IfcPreDefinedColours
        {
            get { return new IfcPreDefinedColours(_model); }
        }

        public IfcPreDefinedTextFonts IfcPreDefinedTextFonts
        {
            get { return new IfcPreDefinedTextFonts(_model); }
        }

        public IfcPreDefinedCurveFonts IfcPreDefinedCurveFonts
        {
            get { return new IfcPreDefinedCurveFonts(_model); }
        }

        public IfcPreDefinedSymbols IfcPreDefinedSymbols
        {
            get { return new IfcPreDefinedSymbols(_model); }
        }
    }
}