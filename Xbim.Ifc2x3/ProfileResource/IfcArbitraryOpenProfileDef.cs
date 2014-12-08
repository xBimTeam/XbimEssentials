#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcArbitraryOpenProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcArbitraryOpenProfileDef : IfcProfileDef
    {
        #region Fields

        private IfcBoundedCurve _curve;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcBoundedCurve Curve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curve;
            }
            set { this.SetModelValue(this, ref _curve, value, v => Curve = v, "Curve"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _curve = (IfcBoundedCurve) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string err = "";
            if (Curve.Dim != 2)
                err +=
                    "WR12: ArbitraryOpenProfileDef:  The curve used for the curve definition shall have the dimensionality of 2\n";
            if (!(this is IfcCenterLineProfileDef) || ProfileType != IfcProfileTypeEnum.CURVE)
                err +=
                    "WR11 ArbitraryOpenProfileDef  :  The profile type is not of type .CURVE., an open profile can only be used to define a swept surface.\n";
            return err;
        }

        #endregion
    }
}