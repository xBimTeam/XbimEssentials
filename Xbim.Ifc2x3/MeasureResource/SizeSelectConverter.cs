#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    SizeSelectConverter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    public class SizeSelectConverter : TypeConverter
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
                if (str.Contains('%') && !char.IsLetter(str, 0)) //it is a ratio measure
                {
                    str = str.TrimEnd(new[] {'%'});
                    double val = double.Parse(str, CultureInfo.InvariantCulture);
                    if (val > 0)
                        return new IfcPositiveRatioMeasure(val/100);
                    else
                        return new IfcRatioMeasure(val/100);
                }
                else if (str.Contains('*') && !char.IsLetter(str, 0)) //it is a normalized ratio measure
                {
                    str = str.TrimEnd(new[] {'*'});
                    double val = double.Parse(str, CultureInfo.InvariantCulture);
                    if (val >= 0 || val <= 1.0)
                        return new IfcNormalisedRatioMeasure(val);
                }
                else if (char.IsLetter(str, 0)) //it is a descriptive measure
                {
                    return new IfcDescriptiveMeasure(str);
                }
                else //it should be a length measure
                {
                    double val = double.Parse(str, CultureInfo.InvariantCulture);
                    if (val > 0)
                        return new IfcPositiveLengthMeasure(val);
                    else
                        return new IfcLengthMeasure(val);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}