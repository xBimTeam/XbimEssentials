#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperators.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcCartesianTransformationOperators
    {
        private readonly IModel _model;

        public IfcCartesianTransformationOperators(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcCartesianTransformationOperator> Items
        {
            get { return this._model.Instances.OfType<IfcCartesianTransformationOperator>(); }
        }

        public IfcCartesianTransformationOperator2Ds IfcCartesianTransformationOperator2Ds
        {
            get { return new IfcCartesianTransformationOperator2Ds(_model); }
        }

        public IfcCartesianTransformationOperator3Ds IfcCartesianTransformationOperator3Ds
        {
            get { return new IfcCartesianTransformationOperator3Ds(_model); }
        }
    }
}