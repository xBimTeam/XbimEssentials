#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDescriptiveMeasure.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcEntity(IfcEntityType.MeasureValue)]
    [Serializable]
    public struct IfcDescriptiveMeasure : IFormattable, IPersistIfc, IfcMeasureValue, IfcSizeSelect
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
            return ToPart21;
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
        public IfcDescriptiveMeasure(string label)
        {
            if (label != null && label.Length > 255)
                throw new ArgumentOutOfRangeException("Label", "Max length for a label is 255 characters");
            _theValue = label;
        }

        public static implicit operator string(IfcDescriptiveMeasure obj)
        {
            return obj == null ? null : (obj._theValue);
        }

        public static implicit operator IfcDescriptiveMeasure(string str)
        {
            return new IfcDescriptiveMeasure(str);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcDescriptiveMeasure) obj)._theValue == _theValue;
        }


        public static bool operator ==(IfcDescriptiveMeasure r1, IfcDescriptiveMeasure r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(IfcDescriptiveMeasure r1, IfcDescriptiveMeasure r2)
        {
            return !Equals(r1, r2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcDescriptiveMeasure value)
        {
            if (value._theValue != null)
                return new StepP21Token(string.Format(@"'{0}'", value._theValue));
            else
                return new StepP21Token("$");
        }

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}