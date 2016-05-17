using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Xbim.Ifc4.DateTimeResource
{
    public partial struct IfcDuration
    {
        public TimeSpan ToTimeSpan()
        {
            return XmlConvert.ToTimeSpan(Value.ToString());
        }

        public static implicit operator IfcDuration(TimeSpan value)
        {
            //https://www.w3.org/TR/xmlschema-2/#duration
            return new IfcDuration(string.Format("P{0}DT{1}H{2}M{3:F3}S", value.Days, value.Hours, value.Minutes, value.Seconds+value.Milliseconds/1000.0));
        }

        public static implicit operator TimeSpan(IfcDuration obj)
        {
            string value = obj;
            if(string.IsNullOrWhiteSpace(value) || value[0] != 'P')
                return new TimeSpan();

            //https://www.w3.org/TR/xmlschema-2/#duration
            //PnYnMnDTnHnMnS

            var daysTotal = 0;
            var yReg = new Regex("(?<Y>[0-9]+)Y", RegexOptions.Compiled);
            var yMatch = yReg.Match(value);
            if (yMatch.Success)
                daysTotal += (int) (int.Parse(yMatch.Groups["Y"].Value)*365.25);
            var mReg = new Regex("^[^T]+(?<M>[0-9]+)M", RegexOptions.Compiled);
            var mMatch = mReg.Match(value);
            if (mMatch.Success)
                daysTotal += (int)(int.Parse(mMatch.Groups["M"].Value) * 30.4166780729);
            var dReg = new Regex("(?<D>[0-9]+)D", RegexOptions.Compiled);
            var dMatch = dReg.Match(value);
            if (dMatch.Success)
                daysTotal += int.Parse(dMatch.Groups["D"].Value);

            var hours = 0;
            var hReg = new Regex("(?<H>[0-9]+)H", RegexOptions.Compiled);
            var hMatch = hReg.Match(value);
            if (hMatch.Success)
                hours = int.Parse(hMatch.Groups["H"].Value);

            var minutes = 0;
            var miReg = new Regex("T[0-9]*H?(?<M>[0-9]+)M", RegexOptions.Compiled);
            var miMatch = miReg.Match(value);
            if (miMatch.Success)
                minutes = int.Parse(miMatch.Groups["M"].Value);

            var seconds = 0.0;
            var sReg = new Regex("(?<S>[0-9]+\\.?[0-9]*)S", RegexOptions.Compiled);
            var sMatch = sReg.Match(value);
            if (sMatch.Success)
                seconds = float.Parse(sMatch.Groups["S"].Value, NumberStyles.Any,CultureInfo.InvariantCulture);

            return new TimeSpan(daysTotal, hours, minutes, (int)seconds, (int)((seconds - (int)seconds)*1000));

        }
    }
}
