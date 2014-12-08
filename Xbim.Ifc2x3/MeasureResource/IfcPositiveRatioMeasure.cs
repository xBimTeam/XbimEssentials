#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPositiveRatioMeasure.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [Serializable]
    public struct IfcPositiveRatioMeasure : IPersistIfc, IfcMeasureValue, IfcSizeSelect
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

        #region ExpressType Members

        public string ToPart21
        {
            get { return IfcReal.AsPart21(_theValue); }
        }

        public object Value
        {
            get { return _theValue; }
        }

        #endregion

        private double _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public override string ToString()
        {
            return IfcReal.AsPart21(_theValue);
            //string str = _theValue.ToString();
            //if (str.IndexOfAny(new[] {'.', 'E', 'e'}) == -1) str += ".";
            //return str;
            
        }

        public IfcPositiveRatioMeasure(double val)
        {
            //if (val <= 0)
            //    throw new ArgumentOutOfRangeException("PositiveRatioMeasures canot be 0 or less");
            _theValue = val;
        }


        public static implicit operator IfcPositiveRatioMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcPositiveRatioMeasure((double) value);
            else
                return new IfcPositiveRatioMeasure();
        }

        public static implicit operator IfcPositiveRatioMeasure(double value)
        {
            return new IfcPositiveRatioMeasure(value);
        }

        public static implicit operator double(IfcPositiveRatioMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcPositiveRatioMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcPositiveRatioMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcPositiveRatioMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcPositiveRatioMeasure obj1, IfcPositiveRatioMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcPositiveRatioMeasure obj1, IfcPositiveRatioMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcPositiveRatioMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcPositiveRatioMeasure) value).ToString());
            else
                return new StepP21Token("$");
        }

        #region XbimType Members

        public double ToDouble
        {
            get { return _theValue; }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue <= 0)
                return "WR1 PositiveRatioMeasure : A positive measure shall be greater than zero.";
            else
                return "";
        }

        #endregion
    }
}