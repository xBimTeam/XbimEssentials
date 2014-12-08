#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEllipseProfileDef.cs
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
    public class IfcEllipseProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _semiAxis1;
        private IfcPositiveLengthMeasure _semiAxis2;

        #endregion

        #region Properties

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure SemiAxis1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _semiAxis1;
            }
            set { this.SetModelValue(this, ref _semiAxis1, value, v => SemiAxis1 = v, "SemiAxis1"); }
        }

        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure SemiAxis2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _semiAxis2;
            }
            set { this.SetModelValue(this, ref _semiAxis2, value, v => SemiAxis2 = v, "SemiAxis2"); }
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
                    _semiAxis1 = value.RealVal;
                    break;
                case 4:
                    _semiAxis2 = value.RealVal;
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