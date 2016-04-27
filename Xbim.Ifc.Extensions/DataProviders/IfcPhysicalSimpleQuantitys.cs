#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPhysicalSimpleQuantitys.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.QuantityResource;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcPhysicalSimpleQuantitys
    {
        private readonly IModel _model;

        public IfcPhysicalSimpleQuantitys(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPhysicalSimpleQuantity> Items
        {
            get { return this._model.Instances.OfType<IfcPhysicalSimpleQuantity>(); }
        }

        public IfcQuantityLengths IfcQuantityLengths
        {
            get { return new IfcQuantityLengths(_model); }
        }

        public IfcQuantityCounts IfcQuantityCounts
        {
            get { return new IfcQuantityCounts(_model); }
        }

        public IfcQuantityWeights IfcQuantityWeights
        {
            get { return new IfcQuantityWeights(_model); }
        }

        public IfcQuantityAreas IfcQuantityAreas
        {
            get { return new IfcQuantityAreas(_model); }
        }

        public IfcQuantityVolumes IfcQuantityVolumes
        {
            get { return new IfcQuantityVolumes(_model); }
        }

        public IfcQuantityTimes IfcQuantityTimes
        {
            get { return new IfcQuantityTimes(_model); }
        }
    }
}