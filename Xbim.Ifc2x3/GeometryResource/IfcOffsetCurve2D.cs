#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOffsetCurve2D.cs
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
    public class IfcOffsetCurve2D : IfcCurve
    {
        #region Fields

        private IfcCurve _basisCurve;
        private IfcLengthMeasure _distance;
        private IfcLogical _selfIntersect;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The curve that is being offset.
        /// </summary>
        [Ifc(1, IfcAttributeState.Mandatory)]
        public IfcCurve BasisCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _basisCurve;
            }
            set { this.SetModelValue(this, ref _basisCurve, value, v => BasisCurve = v, "BasisCurve"); }
        }

        /// <summary>
        ///   The distance of the offset curve from the basis curve. distance may be positive, negative or zero. 
        ///   A positive value of distance defnes an offset in the direction which is normal to the curve in the sense of an 
        ///   anti-clockwise rotation through 90 degrees from the tangent vector T at the given point. 
        ///   (This is in the direction of orthogonal complement(T).)
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLengthMeasure Distance
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _distance;
            }
            set { this.SetModelValue(this, ref _distance, value, v => Distance = v, "Distance"); }
        }

        /// <summary>
        ///   An indication of whether the offset curve self-intersects; this is for information only.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcLogical SelfIntersect
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _selfIntersect;
            }
            set { this.SetModelValue(this, ref _selfIntersect, value, v => SelfIntersect = v, "SelfIntersect"); }
        }

        public override IfcDimensionCount Dim
        {
            get { return _basisCurve.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _basisCurve = (IfcCurve) value.EntityVal;
                    break;
                case 1:
                    _distance = value.RealVal;
                    break;
                case 2:
                    _selfIntersect = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (_basisCurve.Dim != 2)
                return "WR1 OffsetCurve2D: The underlying curve shall be defined in two-dimensional space.";
            else
                return "";
        }
    }
}