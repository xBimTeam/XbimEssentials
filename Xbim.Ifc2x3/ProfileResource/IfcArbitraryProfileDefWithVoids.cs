#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcArbitraryProfileDefWithVoids.cs
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
    public class IfcArbitraryProfileDefWithVoids : IfcArbitraryClosedProfileDef
    {
        public IfcArbitraryProfileDefWithVoids()
        {
            _innerCurves = new CurveSet(this);
        }

        #region Fields

        private CurveSet _innerCurves;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public CurveSet InnerCurves
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _innerCurves;
            }
            set { this.SetModelValue(this, ref _innerCurves, value, v => InnerCurves = v, "InnerCurves"); }
        }


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
                    _innerCurves.Add((IfcCurve) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (ProfileType != IfcProfileTypeEnum.AREA)
                baseErr +=
                    "WR1: ArbitraryProfileDefWithVoids: The type of the profile shall be AREA, as it can only be involved in the definition of a swept area.";
            foreach (IfcCurve curve in InnerCurves)
            {
                if (curve.Dim != 2)
                    baseErr += "WR2: ArbitraryProfileDefWithVoids: All inner curves shall have the dimensionality of 2.";
                if (curve is IfcLine)
                    baseErr +=
                        "WR3: ArbitraryProfileDefWithVoids: None of the inner curves shall by of type IfcLine, as an IfcLine can not be a closed curve.";
            }
            return baseErr;
        }
    }
}