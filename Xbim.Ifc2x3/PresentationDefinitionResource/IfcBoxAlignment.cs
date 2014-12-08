#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoxAlignment.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.MeasureResource;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    [Serializable]
    public struct IfcBoxAlignment : IFormattable, IPersistIfc, IfcSimpleValue
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
        ///   Ensures BoxAlignment does not exceed 255 chars
        /// </summary>
        public IfcBoxAlignment(string label)
        {
            _theValue = label.ToLower();
        }


        public static implicit operator string(IfcBoxAlignment obj)
        {
            return obj == null ? null : (obj._theValue);
        }

        public static implicit operator IfcBoxAlignment(string str)
        {
            return new IfcBoxAlignment(str);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcBoxAlignment) obj)._theValue == _theValue;
        }


        public static bool operator ==(IfcBoxAlignment r1, IfcBoxAlignment r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcBoxAlignment r1, IfcBoxAlignment r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        #region ISupportIfcParser Members

        public static string[] ValidBoxAlignments = new[]
                                                        {
                                                            "top-left", "top-middle", "top-right", "middle-left",
                                                            "center",
                                                            "middle-right', 'bottom-left", "bottom-middle",
                                                            "bottom-right"
                                                        };

        public string WhereRule()
        {
            if (!ValidBoxAlignments.Contains(_theValue))
                return
                    string.Format(
                        @"WR1 BoxAlignment : Invalid token for BoxAlignment = '{0}', it must be one of 'top-left', 'top-middle', 'top-right', 'middle-left', 'center', 'middle-right', 'bottom-left', 'bottom-middle', 'bottom-right'",
                        _theValue);
            else
                return "";
        }

        #endregion
    }
}