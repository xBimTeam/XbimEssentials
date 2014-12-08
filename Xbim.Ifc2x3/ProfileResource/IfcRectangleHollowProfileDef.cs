#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRectangleHollowProfileDef.cs
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
    public class IfcRectangleHollowProfileDef : IfcRectangleProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _wallThickness;
        private IfcPositiveLengthMeasure? _innerFilletRadius;
        private IfcPositiveLengthMeasure? _outerFilletRadius;

        #endregion

        #region Properties

        /// <summary>
        ///   Radius of the material.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure WallThickness
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _wallThickness;
            }
            set { this.SetModelValue(this, ref _wallThickness, value, v => WallThickness = v, "WallThickness"); }
        }

        /// <summary>
        ///   Radius of the circular arcs, by which all four corners of the outer contour of rectangle are equally rounded. 
        ///   If not given, zero (= no rounding arcs) applies.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure? InnerFilletRadius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _innerFilletRadius;
            }
            set
            {
                this.SetModelValue(this, ref _innerFilletRadius, value, v => InnerFilletRadius = v,
                                           "InnerFilletRadius");
            }
        }

        /// <summary>
        ///   Radius of the circular arcs, by which all four corners of the outer contour of rectangle are equally rounded. 
        ///   If not given, zero (= no rounding arcs) applies.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure? OuterFilletRadius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _outerFilletRadius;
            }
            set
            {
                this.SetModelValue(this, ref _outerFilletRadius, value, v => OuterFilletRadius = v,
                                           "OuterFilletRadius");
            }
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
                    _wallThickness = value.RealVal;
                    break;
                case 6:
                    _innerFilletRadius = value.RealVal;
                    break;
                case 7:
                    _outerFilletRadius = value.RealVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_wallThickness >= XDim / 2 || _wallThickness >= YDim / 2)
                baseErr +=
                    "WR31 RectangleHollowProfileDef : The wall thickness shall be smaller than half the value of the X or Y dimension of the rectangle.\n";
            if (_outerFilletRadius.HasValue &&
                (_outerFilletRadius.Value >= XDim / 2 || _outerFilletRadius.Value >= YDim / 2))
                baseErr +=
                    "WR32 RectangleHollowProfileDef : The outer fillet radius (if given) shall be smaller than or equal to half the value of the Xdim and the YDim attribute\n";
            if (_innerFilletRadius.HasValue &&
                (_innerFilletRadius.Value >= XDim / 2 || _innerFilletRadius.Value >= YDim / 2))
                baseErr +=
                    "WR33 RectangleHollowProfileDef : The inner fillet radius (if given) shall be smaller than or equal to half the value of the Xdim and the YDim attribute.\n";
            return baseErr;
        }
    }
}
