#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFillAreaStyleHatching.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcFillAreaStyleHatching : IfcGeometricRepresentationItem, IfcFillStyleSelect
    {
        #region Fields

        private IfcCurveStyle _hatchLineAppearance;
        private IfcHatchLineDistanceSelect _startOfNextHatchLine;
        private IfcCartesianPoint _pointOfReferenceHatchLine;
        private IfcCartesianPoint _patternStart;
        private IfcPlaneAngleMeasure _hatchLineAngle;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The curve style of the hatching lines. Any curve style pattern shall start at the origin of each hatch line.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCurveStyle HatchLineAppearance
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hatchLineAppearance;
            }
            set
            {
                this.SetModelValue(this, ref _hatchLineAppearance, value, v => HatchLineAppearance = v,
                                           "HatchLineAppearance");
            }
        }

        /// <summary>
        ///   A repetition factor that determines the distance between adjacent hatch lines.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcHatchLineDistanceSelect StartOfNextHatchLine
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _startOfNextHatchLine;
            }
            set
            {
                this.SetModelValue(this, ref _startOfNextHatchLine, value, v => StartOfNextHatchLine = v,
                                           "StartOfNextHatchLine");
            }
        }

        /// <summary>
        ///   A Cartesian point which defines the offset of the reference hatch line from the origin of the (virtual) hatching coordinate system. 
        ///   The origin is used for mapping the fill area style hatching onto an annotation fill area or surface. 
        ///   The reference hatch line would then appear with this offset from the fill style target point.
        ///   If not given the reference hatch lines goes through the origin of the (virtual) hatching coordinate system.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcCartesianPoint PointOfReferenceHatchLine
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pointOfReferenceHatchLine;
            }
            set
            {
                this.SetModelValue(this, ref _pointOfReferenceHatchLine, value,
                                           v => PointOfReferenceHatchLine = v, "PointOfReferenceHatchLine");
            }
        }

        /// <summary>
        ///   A distance along the reference hatch line which is the start point for the curve style font pattern of the reference hatch line.
        ///   If not given, the start point of the curve style font pattern is at the (virtual) hatching coordinate system.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcCartesianPoint PatternStart
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _patternStart;
            }
            set { this.SetModelValue(this, ref _patternStart, value, v => PatternStart = v, "PatternStart"); }
        }

        /// <summary>
        ///   A plane angle measure determining the direction of the parallel hatching lines.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPlaneAngleMeasure HatchLineAngle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hatchLineAngle;
            }
            set { this.SetModelValue(this, ref _hatchLineAngle, value, v => HatchLineAngle = v, "HatchLineAngle"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _hatchLineAppearance = (IfcCurveStyle) value.EntityVal;
                    break;
                case 1:
                    _startOfNextHatchLine = (IfcHatchLineDistanceSelect) value.EntityVal;
                    break;
                case 2:
                    _pointOfReferenceHatchLine = (IfcCartesianPoint) value.EntityVal;
                    break;
                case 3:
                    _patternStart = (IfcCartesianPoint) value.EntityVal;
                    break;
                case 4:
                    _hatchLineAngle = value.RealVal;
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
            if (StartOfNextHatchLine is IfcTwoDirectionRepeatFactor)
                err +=
                    "WR21 FillAreaStyleHatching: The subtype IfcTwoDirectionRepeatFactor should not be used to define the start of the next hatch line.\n";
            if (PatternStart != null && PatternStart.Dim != 2)
                err +=
                    "WR22 FillAreaStyleHatching: The CartesianPoint, if given as value to PatternStart shall have the dimensionality of 2.\n";
            if (PointOfReferenceHatchLine != null && PointOfReferenceHatchLine.Dim != 2)
                err +=
                    "WR23 FillAreaStyleHatching: The CartesianPoint, if given as value to PointOfReferenceHatchLine shall have the dimensionality of 2.\n";
            return err;
        }

        #endregion
    }
}