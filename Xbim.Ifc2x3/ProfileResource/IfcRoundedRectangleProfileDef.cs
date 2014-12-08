#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRoundedRectangleProfileDef.cs
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
    public class IfcRoundedRectangleProfileDef : IfcRectangleProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _roundingRadius;

        #endregion

        #region Properties

        public IfcPositiveLengthMeasure RoundingRadius
        {
            get { return _roundingRadius; }
            set { this.SetModelValue(this, ref _roundingRadius, value, v => RoundingRadius = v, "RoundingRadius"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _roundingRadius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_roundingRadius > XDim/2 || _roundingRadius > YDim/2)
                baseErr +=
                    "WR31 RoundedRectangleProfileDef : The value of the attribute RoundingRadius shall be lower or equal than either of both, half the value of the Xdim and the YDim attribute.\n";

            return baseErr;
        }
    }
}