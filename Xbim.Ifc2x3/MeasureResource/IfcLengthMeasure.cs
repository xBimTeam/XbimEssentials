#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLengthMeasure.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using System.Globalization;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [IfcEntity(IfcEntityType.MeasureValue)]
    [Serializable]
    public struct IfcLengthMeasure : IPersistIfc, IfcMeasureValue, IfcSizeSelect
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.RealVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        private double _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public object Value
        {
            get { return _theValue; }
        }

        public override string ToString()
        {
            return IfcReal.AsPart21(_theValue);
            //string str = _theValue.ToString();
            //if (str.IndexOfAny(new[] {'.', 'E', 'e'}) == -1) str += ".";
            //return str;
        }

        public string ToPart21
        {
            get { return IfcReal.AsPart21(_theValue); }
        }


        public IfcLengthMeasure(double val)
        {
            _theValue = val;
        }


        public IfcLengthMeasure(string val)
        {
            _theValue = IfcReal.ToDouble(val);
        }

        public static implicit operator IfcLengthMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcLengthMeasure((double) value);
            else
                return new IfcLengthMeasure();
        }

        public static implicit operator IfcLengthMeasure(double value)
        {
            return new IfcLengthMeasure(value);
        }

        public static implicit operator double(IfcLengthMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcLengthMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcLengthMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcLengthMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcLengthMeasure obj1, IfcLengthMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcLengthMeasure obj1, IfcLengthMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcLengthMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcLengthMeasure) value).ToString());
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