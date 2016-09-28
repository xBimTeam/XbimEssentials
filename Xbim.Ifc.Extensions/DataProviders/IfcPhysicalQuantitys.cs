#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPhysicalQuantitys.cs
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
    public class IfcPhysicalQuantitys
    {
        private readonly IModel _model;

        public IfcPhysicalQuantitys(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcPhysicalQuantity> Items
        {
            get { return this._model.Instances.OfType<IfcPhysicalQuantity>(); }
        }

        public IfcPhysicalSimpleQuantitys IfcPhysicalSimpleQuantitys
        {
            get { return new IfcPhysicalSimpleQuantitys(_model); }
        }

        public IfcPhysicalComplexQuantitys IfcPhysicalComplexQuantitys
        {
            get { return new IfcPhysicalComplexQuantitys(_model); }
        }
    }
}