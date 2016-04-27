#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator2Ds.cs
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
    public class IfcCartesianTransformationOperator2Ds
    {
        private readonly IModel _model;

        public IfcCartesianTransformationOperator2Ds(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcCartesianTransformationOperator2D> Items
        {
            get { return this._model.Instances.OfType<IfcCartesianTransformationOperator2D>(); }
        }

        public IfcCartesianTransformationOperator2DnonUniforms IfcCartesianTransformationOperator2DnonUniforms
        {
            get { return new IfcCartesianTransformationOperator2DnonUniforms(_model); }
        }
    }
}