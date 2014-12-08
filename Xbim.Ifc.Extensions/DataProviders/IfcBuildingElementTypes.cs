#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingElementTypes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ProductExtension;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcBuildingElementTypes
    {
        private readonly IModel _model;

        public IfcBuildingElementTypes(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcBuildingElementType> Items
        {
            get { return this._model.Instances.OfType<IfcBuildingElementType>(); }
        }

        public IfcBuildingElementProxyTypes IfcBuildingElementProxyTypes
        {
            get { return new IfcBuildingElementProxyTypes(_model); }
        }

        public IfcRampFlightTypes IfcRampFlightTypes
        {
            get { return new IfcRampFlightTypes(_model); }
        }

        public IfcRailingTypes IfcRailingTypes
        {
            get { return new IfcRailingTypes(_model); }
        }

        public IfcBeamTypes IfcBeamTypes
        {
            get { return new IfcBeamTypes(_model); }
        }

        public IfcCoveringTypes IfcCoveringTypes
        {
            get { return new IfcCoveringTypes(_model); }
        }

        public IfcPlateTypes IfcPlateTypes
        {
            get { return new IfcPlateTypes(_model); }
        }

        public IfcMemberTypes IfcMemberTypes
        {
            get { return new IfcMemberTypes(_model); }
        }

        public IfcWallTypes IfcWallTypes
        {
            get { return new IfcWallTypes(_model); }
        }

        public IfcColumnTypes IfcColumnTypes
        {
            get { return new IfcColumnTypes(_model); }
        }

        public IfcCurtainWallTypes IfcCurtainWallTypes
        {
            get { return new IfcCurtainWallTypes(_model); }
        }

        public IfcStairFlightTypes IfcStairFlightTypes
        {
            get { return new IfcStairFlightTypes(_model); }
        }

        public IfcSlabTypes IfcSlabTypes
        {
            get { return new IfcSlabTypes(_model); }
        }
    }
}