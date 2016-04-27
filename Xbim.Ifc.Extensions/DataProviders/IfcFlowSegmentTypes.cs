#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFlowSegmentTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.SharedBldgServiceElements;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcFlowSegmentTypes
    {
        private readonly IModel _model;

        public IfcFlowSegmentTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcFlowSegmentType> Items
        {
            get { return this._model.Instances.OfType<IfcFlowSegmentType>(); }
        }

        public IfcCableCarrierSegmentTypes IfcCableCarrierSegmentTypes
        {
            get { return new IfcCableCarrierSegmentTypes(_model); }
        }

        public IfcPipeSegmentTypes IfcPipeSegmentTypes
        {
            get { return new IfcPipeSegmentTypes(_model); }
        }

        public IfcDuctSegmentTypes IfcDuctSegmentTypes
        {
            get { return new IfcDuctSegmentTypes(_model); }
        }
    }
}