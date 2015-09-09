#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCurveStyle.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcCurveStyle : IfcPresentationStyle, IfcPresentationStyleSelect
    {


        #region Fields

        private IfcCurveFontOrScaledCurveFontSelect _curveFont;
        private IfcSizeSelect _curveWidth;
        private IfcColour _curveColour;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   A curve style font which is used to present a curve. It can either be a predefined curve font, or an explicitly defined curve font. 
        ///   Both may be scaled. If not given, then the curve font should be taken from the layer assignment with style, 
        ///   if that is not given either, then the default curve font applies.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcCurveFontOrScaledCurveFontSelect CurveFont
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curveFont;
            }
            set { this.SetModelValue(this, ref _curveFont, value, v => CurveFont = v, "CurveFont "); }
        }


        /// <summary>
        ///   A positive length measure in units of the presentation area for the width of a presented curve. 
        ///   If not given, then the style should be taken from the layer assignment with style, if that is not given either, then the default style applies.
        /// </summary>
        [TypeConverter(typeof (SizeSelectConverter))]
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcSizeSelect CurveWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curveWidth;
            }
            set
            {
                if (value is IfcRatioMeasure || value is IfcLengthMeasure || value is IfcDescriptiveMeasure ||
                    value is IfcPositiveLengthMeasure || value is IfcNormalisedRatioMeasure ||
                    value is IfcPositiveRatioMeasure)
                    this.SetModelValue(this, ref _curveWidth, value, v => CurveWidth = v, "CurveWidth");
                else
                    throw new ArgumentException("Invalid width, must be SizeSelect", "CurveWidth");
            }
        }


        /// <summary>
        ///   The colour of the visible part of the curve. 
        ///   If not given, then the colour should be taken from the layer assignment with style, if that is not given either, then the default colour applies.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcColour CurveColour
        {
            get { return _curveColour; }
            set
            {
                if (value is IfcDraughtingPreDefinedColour || value is IfcColourRgb)
                    this.SetModelValue(this, ref _curveColour, value, v => CurveColour = v, "CurveColour");
                else
                    throw new ArgumentException("Invalid colour, must be DraughtingPreDefinedColour or ColourRgb",
                                                "CurveColour");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _curveFont = (IfcCurveFontOrScaledCurveFontSelect) value.EntityVal;
                    break;
                case 2:
                    _curveWidth = (IfcSizeSelect) value.EntityVal;
                    break;
                case 3:
                    _curveColour = (IfcColour) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (_curveWidth != null)
                if (
                    !(_curveWidth.GetType() == typeof (IfcPositiveLengthMeasure) ||
                      (typeof (IfcDescriptiveMeasure) == _curveWidth.GetType() &&
                       ((string) ((IfcDescriptiveMeasure) _curveWidth)) == "by layer")))
                    return
                        "WR11 CurveStyle : The curve width, if provided, shall be given by an IfcPositiveLengthMeasure representing the curve width in the default measure unit, or by an IfcDescriptiveMeasure with the value 'by layer' representing the curve width by the default curve width at the associated layer.\n";
            return "";
        }
    }
}