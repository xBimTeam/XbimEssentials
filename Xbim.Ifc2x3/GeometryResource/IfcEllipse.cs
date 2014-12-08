#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEllipse.cs
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

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcEllipse : IfcConic
    {
        #region Fields

        private IfcPositiveLengthMeasure _semiAxis1;
        private IfcPositiveLengthMeasure _semiAxis2;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The first radius of the ellipse which shall be positive. Placement.Axes[1] gives the direction of the SemiAxis1.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure SemiAxis1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _semiAxis1;
            }
            set { this.SetModelValue(this, ref _semiAxis1, value, v => SemiAxis1 = v, "SemiAxis1"); }
        }

        /// <summary>
        ///   The second radius of the ellipse which shall be positive.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure SemiAxis2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _semiAxis2;
            }
            set { this.SetModelValue(this, ref _semiAxis2, value, v => SemiAxis2 = v, "SemiAxis2"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _semiAxis1 = value.RealVal;
                    break;
                case 2:
                    _semiAxis2 = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}