#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcParameterizedProfileDefs.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.ProfileResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcParameterizedProfileDefs
    {
        private readonly IModel _model;

        public IfcParameterizedProfileDefs(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcParameterizedProfileDef> Items
        {
            get { return this._model.Instances.OfType<IfcParameterizedProfileDef>(); }
        }

        public IfcCraneRailFShapeProfileDefs IfcCraneRailFShapeProfileDefs
        {
            get { return new IfcCraneRailFShapeProfileDefs(_model); }
        }

        public IfcUShapeProfileDefs IfcUShapeProfileDefs
        {
            get { return new IfcUShapeProfileDefs(_model); }
        }

        public IfcTShapeProfileDefs IfcTShapeProfileDefs
        {
            get { return new IfcTShapeProfileDefs(_model); }
        }

        public IfcEllipseProfileDefs IfcEllipseProfileDefs
        {
            get { return new IfcEllipseProfileDefs(_model); }
        }

        public IfcCircleProfileDefs IfcCircleProfileDefs
        {
            get { return new IfcCircleProfileDefs(_model); }
        }

        public IfcCraneRailAShapeProfileDefs IfcCraneRailAShapeProfileDefs
        {
            get { return new IfcCraneRailAShapeProfileDefs(_model); }
        }

        public IfcTrapeziumProfileDefs IfcTrapeziumProfileDefs
        {
            get { return new IfcTrapeziumProfileDefs(_model); }
        }

        public IfcRectangleProfileDefs IfcRectangleProfileDefs
        {
            get { return new IfcRectangleProfileDefs(_model); }
        }

        public IfcLShapeProfileDefs IfcLShapeProfileDefs
        {
            get { return new IfcLShapeProfileDefs(_model); }
        }

        public IfcIShapeProfileDefs IfcIShapeProfileDefs
        {
            get { return new IfcIShapeProfileDefs(_model); }
        }

        public IfcCShapeProfileDefs IfcCShapeProfileDefs
        {
            get { return new IfcCShapeProfileDefs(_model); }
        }

        public IfcZShapeProfileDefs IfcZShapeProfileDefs
        {
            get { return new IfcZShapeProfileDefs(_model); }
        }
    }
}