#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTemperatureGradientMeasure.cs
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
    public struct IfcTemperatureGradientMeasure : IPersistIfc, IfcDerivedMeasureValue
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

        public IfcTemperatureGradientMeasure(double val)
        {
            _theValue = val;
        }


        public IfcTemperatureGradientMeasure(string val)
        {
            _theValue = IfcReal.ToDouble(val);
        }

        public static implicit operator IfcTemperatureGradientMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcTemperatureGradientMeasure((double) value);
            else
                return new IfcTemperatureGradientMeasure();
        }

        public static implicit operator IfcTemperatureGradientMeasure(double value)
        {
            return new IfcTemperatureGradientMeasure(value);
        }

        public static implicit operator double(IfcTemperatureGradientMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcTemperatureGradientMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcTemperatureGradientMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcTemperatureGradientMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcTemperatureGradientMeasure obj1, IfcTemperatureGradientMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcTemperatureGradientMeasure obj1, IfcTemperatureGradientMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcTemperatureGradientMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcTemperatureGradientMeasure) value).ToString());
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