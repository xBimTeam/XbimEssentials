#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCircleHollowProfileDef.cs
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
    public class IfcCircleHollowProfileDef : IfcCircleProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _wallThickness;

        #endregion

        #region Properties

        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure WallThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _wallThickness;
            }
            set { this.SetModelValue(this, ref _wallThickness, value, v => WallThickness = v, "WallThickness"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _wallThickness = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_wallThickness >= Radius)
                baseErr += "WR1 CircleHollowProfileDef : The wall thickness shall be smaller then the radius.\n";
            return baseErr;
        }
    }
}