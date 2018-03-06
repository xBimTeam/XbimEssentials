using System;
using System.Globalization;
using System.Text;

namespace Xbim.IO.Step21
{
    public static class StepText
    {
        internal static readonly CultureInfo DoubleCulture = new CultureInfo("en-US");
        public static double ToDouble(this string val)
        {
            switch (val)
            {
                case "-1.#INF":
                    return double.NegativeInfinity;
                case "1.#INF":
                    return double.PositiveInfinity;
                case "-1.#IND":
                    return double.NaN;
            }
            return Convert.ToDouble(val, DoubleCulture);
        }

        public static string ToPart21(this string source)
        {
            if (string.IsNullOrEmpty(source)) return "";
            var state = WriteState.Normal;
            var sb = new StringBuilder(source.Length * 2);


            for (var i = 0; i < source.Length; i++)
            {
                int c;
                try
                {
                    c = char.ConvertToUtf32(source, i);
                }
                catch (Exception)
                {
                    c = '?';
                }
                if (c > 0xFFFF)
                {
                    state = SetMode(WriteState.FourBytes, state, sb);
                    sb.AppendFormat(@"{0:X8}", c);
                    i++; // to skip the next surrogate
                }
                else if (c > 0xFF)
                {
                    state = SetMode(WriteState.TwoBytes, state, sb);
                    sb.AppendFormat(@"{0:X4}", c);
                }
                else
                {
                    state = SetMode(WriteState.Normal, state, sb);
                    // boundaries according to specs from http://www.buildingsmart-tech.org/downloads/accompanying-documents/guidelines/IFC2x%20Model%20Implementation%20Guide%20V2-0b.pdf
                    if (c > 126 || c < 32)
                        sb.AppendFormat(@"\X\{0:X2}", c);
                    //needs un-escaping as this is converting SIZE: 2'x2'x3/4" to SIZE: 2''x2''x3/4" and Manufacturer's to Manufacturer''s 
                    else if ((char)c == '\'')
                        sb.Append("''");
                    else if ((char)c == '\\')
                        sb.Append("\\\\");
                    else
                        sb.Append((char)c);
                }
            }
            SetMode(WriteState.Normal, state, sb);
            return sb.ToString();
        }

        private enum WriteState
        {
            Normal,
            TwoBytes,
            FourBytes
        }

        private static WriteState SetMode(WriteState newState, WriteState fromState, StringBuilder sb)
        {
            if (newState == fromState)
                return newState;
            if (fromState != WriteState.Normal)
            {
                // needs to close the old state
                sb.Append(@"\X0\");
            }
            if (newState == WriteState.TwoBytes)
            {
                sb.Append(@"\X2\");
            }
            else if (newState == WriteState.FourBytes)
            {
                sb.Append(@"\X4\");
            }
            return newState;
        }
    }
}
