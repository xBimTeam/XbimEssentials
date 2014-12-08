#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTextAlignment.cs
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

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [Serializable]
    public struct IfcTextAlignment : IFormattable, IPersistIfc, ExpressType
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

        public static implicit operator IfcTextAlignment(string str)
        {
            return new IfcTextAlignment(str);
        }

        /// <summary>
        ///   Ensures only string type is used
        /// </summary>
        public IfcTextAlignment(string txt)
        {
            _theValue = txt;
        }


        public static implicit operator string(IfcTextAlignment obj)
        {
            return (obj._theValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcTextAlignment) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcTextAlignment r1, IfcTextAlignment r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcTextAlignment r1, IfcTextAlignment r2)
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

        private static readonly string[] validNames = {"left", "right", "center", "justify"};

        public string WhereRule()
        {
            if (!validNames.Contains(_theValue.ToLower()))
                return @"WR1 TextAlignment : Allowable values for font style are 'left', 'right', 'center', 'justify'";
            else
                return "";
        }

        #endregion
    }
}