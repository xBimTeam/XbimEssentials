#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcArbitraryClosedProfileDef.cs
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
    public class IfcArbitraryClosedProfileDef : IfcProfileDef
    {
        #region Fields

        private IfcCurve _outerCurve;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcCurve OuterCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _outerCurve;
            }
            set { this.SetModelValue(this, ref _outerCurve, value, v => OuterCurve = v, "OuterCurve"); }
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
                    _outerCurve = (IfcCurve) value.EntityVal;
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
            if (OuterCurve.Dim != 2)
                err +=
                    "WR1: ArbitraryClosedProfileDef:  The curve used for the outer curve definition shall have the dimensionality of 2.\n";
            if (OuterCurve is IfcLine)
                err +=
                    "WR2: ArbitraryClosedProfileDef:  The outer curve shall not be of type IfcLine as IfcLine is not a closed curve.\n";
            if (OuterCurve is IfcOffsetCurve2D)
                err +=
                    "WR2: ArbitraryClosedProfileDef:  The outer curve shall not be of type IfcOffsetCurve2D as it should not be defined as an offset of another curve.\n";
            return err;
        }

        #endregion
    }
}