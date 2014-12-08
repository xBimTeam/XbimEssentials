#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTextStyleFontModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcTextStyleFontModel : IfcPreDefinedTextFont
    {
        #region Fields

        private XbimList<IfcTextFontName> _fontFamily;
        private IfcFontStyle _fontStyle;
        private IfcFontVariant _fontVariant;
        private IfcFontWeight _fontWeight;
        private IfcSizeSelect _fontSize;

        #endregion

        #region Properties

        /// <summary>
        ///   The value is a prioritized list of font family names and/or generic family names. 
        ///   The first list entry has the highest priority, if this font fails, the next list item shall be used. 
        ///   The last list item should (if possible) be a generic family.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional, IfcAttributeType.List)]
        public XbimList<IfcTextFontName> FontFamily
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fontFamily;
            }
            set { this.SetModelValue(this, ref _fontFamily, value, v => FontFamily = v, "FontFamily"); }
        }

        /// <summary>
        ///   The font style property selects between normal (sometimes referred to as "roman" or "upright"), italic and oblique faces within a font family.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcFontStyle FontStyle
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fontStyle;
            }
            set { this.SetModelValue(this, ref _fontStyle, value, v => FontStyle = v, "FontStyle"); }
        }

        /// <summary>
        ///   The font variant property selects between normal and small-caps
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcFontVariant FontVariant
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fontVariant;
            }
            set { this.SetModelValue(this, ref _fontVariant, value, v => FontVariant = v, "FontVariant"); }
        }

        /// <summary>
        ///   The font weight property selects the weight of the font.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcFontWeight FontWeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fontWeight;
            }
            set { this.SetModelValue(this, ref _fontWeight, value, v => FontWeight = v, "FontWeight"); }
        }

        /// <summary>
        ///   The font size provides the size or height of the text font
        ///   NOTE  The following values are allowed, LengthMeasure, with positive values, the length unit is globally defined at IfcUnitAssignment
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcSizeSelect FontSize
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fontSize;
            }
            set { this.SetModelValue(this, ref _fontSize, value, v => FontSize = v, "FontSize"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    if (_fontFamily == null) _fontFamily = new XbimList<IfcTextFontName>(this);
                    if (value.Type == IfcParserType.String)
                        _fontFamily.Add(new IfcTextFontName(value.StringVal));
                    else

                        _fontFamily.Add((IfcTextFontName)value.EntityVal);
                    break;
                case 2:
                    _fontStyle = value.StringVal;
                    break;
                case 3:
                    _fontVariant = value.StringVal;
                    break;
                case 4:
                    _fontWeight = value.StringVal;
                    break;
                case 5:
                    _fontSize = (IfcSizeSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_fontSize == null || typeof (IfcLengthMeasure) != _fontSize.GetType() ||
                ((IfcLengthMeasure) _fontSize) <= 0)
                return "WR31 TextStyleFontModel : The size should be given by a positive length measure\n";
            else
                return "";
        }
    }
}