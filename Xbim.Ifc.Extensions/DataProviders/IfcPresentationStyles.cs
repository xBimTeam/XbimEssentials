#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPresentationStyles.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.PresentationAppearanceResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPresentationStyles
    {
        private readonly IModel _model;

        public IfcPresentationStyles(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPresentationStyle> Items
        {
            get { return this._model.Instances.OfType<IfcPresentationStyle>(); }
        }

        public IfcCurveStyles IfcCurveStyles
        {
            get { return new IfcCurveStyles(_model); }
        }

        public IfcFillAreaStyles IfcFillAreaStyles
        {
            get { return new IfcFillAreaStyles(_model); }
        }

        public IfcSurfaceStyles IfcSurfaceStyles
        {
            get { return new IfcSurfaceStyles(_model); }
        }

        public IfcTextStyles IfcTextStyles
        {
            get { return new IfcTextStyles(_model); }
        }
    }
}