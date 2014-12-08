#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTrapeziumProfileDef.cs
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

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcTrapeziumProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _bottomXDim;
        private IfcPositiveLengthMeasure _topXDim;
        private IfcPositiveLengthMeasure _yDim;
        private IfcLengthMeasure _topXOffset;

        #endregion

        #region Properties

        /// <summary>
        ///   The extent of the bottom line measured along the implicit x-axis.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure BottomXDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _bottomXDim;
            }
            set { this.SetModelValue(this, ref _bottomXDim, value, v => BottomXDim = v, "BottomXDim"); }
        }

        /// <summary>
        ///   The extent of the top line measured along the implicit x-axis.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure TopXDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _topXDim;
            }
            set { this.SetModelValue(this, ref _topXDim, value, v => TopXDim = v, "TopXDim"); }
        }

        /// <summary>
        ///   The extent of the distance between the parallel bottom and top lines measured along the implicit y-axis.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure YDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _yDim;
            }
            set { this.SetModelValue(this, ref _yDim, value, v => YDim = v, "YDim"); }
        }

        /// <summary>
        ///   Offset from the beginning of the top line to the bottom line, measured along the implicit x-axis.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcLengthMeasure TopXOffset
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _topXOffset;
            }
            set { this.SetModelValue(this, ref _topXOffset, value, v => TopXOffset = v, "TopXOffset"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _bottomXDim = value.RealVal;
                    break;
                case 4:
                    _topXDim = value.RealVal;
                    break;
                case 5:
                    _yDim = value.RealVal;
                    break;
                case 6:
                    _topXOffset = value.RealVal;
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