#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFontStyle.cs
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
using Xbim.Ifc2x3.MeasureResource;

#endregion

namespace Xbim.Ifc2x3.PresentationResource
{
    [Serializable]
    public struct IfcFontStyle : IFormattable, IPersistIfc, ExpressType
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


        public string ToPart21
        {
            get { return _theValue != null ? string.Format(@"'{0}'", IfcText.Escape(_theValue)) : "$"; }
        }

        public static implicit operator IfcFontStyle(string str)
        {
            return new IfcFontStyle(str);
        }

        /// <summary>
        ///   Ensures only string type is used
        /// </summary>
        public IfcFontStyle(string txt)
        {
            _theValue = txt;
        }


        public static implicit operator string(IfcFontStyle obj)
        {
            return (obj._theValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcFontStyle) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcFontStyle r1, IfcFontStyle r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcFontStyle r1, IfcFontStyle r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
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

        private static readonly string[] validNames = {"normal", "italic", "oblique"};

        public string WhereRule()
        {
            if (_theValue == null || !validNames.Contains(_theValue.ToLower()))
                return @"WR1 FontStyle : Allowable values for font style are 'normal','italic','oblique'";
            else
                return "";
        }

        #endregion
    }
}