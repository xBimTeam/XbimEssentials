#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSlippageConnectionCondition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSlippageConnectionCondition : IfcStructuralConnectionCondition
    {
        #region Fields

        private IfcLengthMeasure? _slippageX;
        private IfcLengthMeasure? _slippageY;
        private IfcLengthMeasure? _slippageZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Slippage of that connection. Defines the maximum displacement in x-direction without any loading applied.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLengthMeasure? SlippageX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _slippageX;
            }
            set { this.SetModelValue(this, ref _slippageX, value, v => SlippageX = v, "SlippageX"); }
        }

        /// <summary>
        ///   Slippage of that connection. Defines the maximum displacement in y-direction without any loading applied.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLengthMeasure? SlippageY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _slippageY;
            }
            set { this.SetModelValue(this, ref _slippageY, value, v => SlippageY = v, "SlippageY"); }
        }

        /// <summary>
        ///   Slippage of that connection. Defines the maximum displacement in z-direction without any loading applied.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLengthMeasure? SlippageZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _slippageZ;
            }
            set { this.SetModelValue(this, ref _slippageZ, value, v => SlippageZ = v, "SlippageZ"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _slippageX = value.RealVal;
                    break;
                case 2:
                    _slippageY = value.RealVal;
                    break;
                case 3:
                    _slippageZ = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}