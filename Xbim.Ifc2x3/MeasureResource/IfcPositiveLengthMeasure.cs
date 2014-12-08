#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPositiveLengthMeasure.cs
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
    [Serializable]
    public struct IfcPositiveLengthMeasure : IPersistIfc, IfcMeasureValue, IfcHatchLineDistanceSelect, IfcSizeSelect
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

        public override string ToString()
        {
            return IfcReal.AsPart21(_theValue);
            //string str = String.Format("{0:F16}", _theValue);
            //string str = _theValue.ToString();
            //if (str.IndexOfAny(new[] {'.', 'E', 'e'}) == -1) str += ".";
            //return str;
        }

        public object Value
        {
            get { return _theValue; }
        }

       
            
       

        public IfcPositiveLengthMeasure(double val)
        {
            _theValue = val;
        }


        public IfcPositiveLengthMeasure(string val)
        {
            _theValue = IfcReal.ToDouble(val);
        }


        public static implicit operator IfcPositiveLengthMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcPositiveLengthMeasure((double) value);
            else
                return new IfcPositiveLengthMeasure();
        }

        public static implicit operator IfcPositiveLengthMeasure?(double? value)
        {
            if (value.HasValue && value.Value > 0)
                return new IfcPositiveLengthMeasure((double) value);
            else
                return null;
        }

        public static implicit operator IfcPositiveLengthMeasure(double value)
        {
            return new IfcPositiveLengthMeasure(value);
        }

        public static implicit operator double(IfcPositiveLengthMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcPositiveLengthMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcPositiveLengthMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcPositiveLengthMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcPositiveLengthMeasure obj1, IfcPositiveLengthMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcPositiveLengthMeasure obj1, IfcPositiveLengthMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        #region ExpressType Members

        public string ToPart21
        {
            get { return IfcReal.AsPart21(_theValue); }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue > 0)
                return null;
            else
                return string.Format("PositiveLengthMeasure = {0}, it must have a value greater than 0", _theValue);
        }

        #endregion
    }
}