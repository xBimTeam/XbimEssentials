#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    CurveFontConverter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    public class CurveFontConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                return value.ToString();
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str != null) str = str.Trim();
            if (!string.IsNullOrEmpty(str))
            {
                string[] tokens = str.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                double scale = 1.0;
                if (tokens.Length > 1) //it is a scaled font
                {
                    str = tokens[0].Trim();
                    scale = double.Parse(tokens[1], CultureInfo.InvariantCulture);
                }

                if (char.IsLetter(str, 0)) //it is a predefined font 
                {
                    if (scale == 1.0) //use CurveStyleFont
                        return new IfcDraughtingPreDefinedCurveFont {Name = str};
                    else //CurveFontStyleAndScale
                        return new IfcCurveStyleFontAndScaling
                                   {
                                       CurveFont = new IfcDraughtingPreDefinedCurveFont {Name = str},
                                       CurveFontScaling = scale
                                   };
                }
                else //it should be CurveFontStyle or a CurveFontStyleAndScale
                {
                    DoubleCollection dc = DoubleCollection.Parse(str);
                    int rem;
                    int patterns = Math.DivRem(dc.Count, 2, out rem);
                    if (rem == 0 && patterns > 0) //we have an even number of stroke patterns
                    {
                        IfcCurveStyleFont font = new IfcCurveStyleFont();
                        for (int i = 0; i < dc.Count; i += 2)
                        {
                            font.PatternList.Add_Reversible(new IfcCurveStyleFontPattern
                                                                {
                                                                    VisibleSegmentLength = dc[i],
                                                                    InvisibleSegmentLength = dc[i + 1]
                                                                });
                        }
                        if (scale == 1.0) //use CurveStyleFont
                            return font;
                        else //CurveFontStyleAndScale
                            return new IfcCurveStyleFontAndScaling {CurveFont = font, CurveFontScaling = scale};
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            StandardValuesCollection svc =
                new StandardValuesCollection(IfcDraughtingPreDefinedCurveFont.PrefinedCurveFonts);
            return svc;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}