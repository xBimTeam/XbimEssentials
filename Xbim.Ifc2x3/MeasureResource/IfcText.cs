#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcText.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using System.Text;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [Serializable]
    public struct IfcText : IFormattable, IPersistIfc, IfcSimpleValue, IfcMetricValueSelect
    {
        private enum WriteState
        {
            Normal,
            TwoBytes,
            FourBytes
        }

        public static string Escape(string source)
        {
            WriteState state = WriteState.Normal;
            StringBuilder sb = new StringBuilder(source.Length * 2);
            
                        
            for (int i = 0; i < source.Length; i++)
            {
                int c;
                try
                {
                    c = Char.ConvertToUtf32(source, i);
                }
                catch (Exception)
                {
                    c = '?';
                }
                if (c > 0xFFFF)
                {
                    state = setMode(WriteState.FourBytes, state, sb);
                    sb.AppendFormat(@"{0:X8}", c);
                    i++; // to skip the next surrogate
                }
                else if (c > 0xFF)
                {
                    state = setMode(WriteState.TwoBytes, state, sb);
                    sb.AppendFormat(@"{0:X4}", c);
                }
                else
                {
                    state = setMode(WriteState.Normal, state, sb);
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
            state = setMode(WriteState.Normal, state, sb);
            return sb.ToString();
        }

        private static WriteState setMode(WriteState newState, WriteState fromState, StringBuilder sb)
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

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.StringVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        private string _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof (string); }
        }

        public object Value
        {
            get { return _theValue; }
        }


        public string ToPart21
        {
            get 
            {
                return _theValue != null ? 
                    string.Format(@"'{0}'", Escape(_theValue)) : // if not null escape
                    "$"; 
            }
        }

        public static implicit operator IfcText(string str)
        {
            return new IfcText(str);
        }

        /// <summary>
        ///   Ensures only string type is used
        /// </summary>
        public IfcText(string txt)
        {
            _theValue = txt;
        }


        public static implicit operator string(IfcText obj)
        {
            return (obj._theValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcText) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcText r1, IfcText r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcText r1, IfcText r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcText value)
        {
            if (value._theValue != null)
                return new StepP21Token(string.Format(@"'{0}'", value._theValue));
            else
                return new StepP21Token("$");
        }

        #region IFormattable Members

        public override string ToString()
        {
            return _theValue;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return _theValue;
            if (format == "P21") return ToPart21;
            else
                throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Invalid format string: '{0}'.",
                                                        format));
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}