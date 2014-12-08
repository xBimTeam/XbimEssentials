#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLabel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcPersistedEntityAttribute]
    public class LabelCollection : XbimList<IfcLabel>, IFormattable
    {
        internal LabelCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        #region Properties

        public string Summary
        {
            get { return this.ToString(); }
        }

        #endregion

        #region IFormattable Members

        public override string ToString()
        {
            if (this.Count > 0)
                return string.Join("; ", this.OfType<string>().Cast<string>().ToArray());
            else
                return null;
        }

        /// <summary>
        ///   Special format method for the properties of a Label collection
        /// </summary>
        /// <remarks>
        ///   Format strings as a delimited list, use {D} followed by any sequence of characters to act as the delimiter
        /// </remarks>
        /// <param name = "format">
        ///   use {D} followed by any sequence of characters to act as the delimiter
        /// </param>
        /// <param name = "formatProvider">
        /// </param>
        /// <returns>String with the formatted result.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return ToString();
            if (format[0] == 'D') //delimited list
            {
                string delim = format.Substring(1);
                return string.Join(delim, this.Select<IfcLabel,String>(lbl=>lbl.ToString()));
            }
            else
                throw new FormatException(String.Format("Invalid format string: '{0}'.", format));
        }

        #endregion
    }


    [Serializable]
    public struct IfcLabel : IFormattable, IPersistIfc, IfcSimpleValue
    {
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

        public string ToPart21
        {
            get { return _theValue != null ? string.Format(@"'{0}'", IfcText.Escape(_theValue)) : "$"; }
        }

        #endregion

        /// <summary>
        ///   Ensures label does not exceed 255 chars
        /// </summary>
        public IfcLabel(string label)
        {
            _theValue = label;
        }


        public static implicit operator string(IfcLabel obj)
        {
            return obj == null ? null : (obj._theValue);
        }


        public static implicit operator IfcLabel(string str)
        {
            return new IfcLabel(str);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcLabel) obj)._theValue == _theValue;
        }


        public static bool operator ==(IfcLabel r1, IfcLabel r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcLabel r1, IfcLabel r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcLabel value)
        {
            if (value._theValue != null)
                return new StepP21Token(string.Format(@"'{0}'", value._theValue));
            else
                return new StepP21Token("$");
        }

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (!string.IsNullOrEmpty(_theValue) && _theValue.Length > 255)
                return "WR1 Label : Max length for a label should not exceed 255 characters";
            else
                return "";
        }

        #endregion
    }
}