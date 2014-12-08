#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcIntegerCountRateMeasure.cs
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
    public struct IfcIntegerCountRateMeasure : IPersistIfc, IfcDerivedMeasureValue
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.IntegerVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        private long _theValue;

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
            return _theValue.ToString();
        }

        public string ToPart21
        {
            get { return _theValue.ToString(); }
        }

        public IfcIntegerCountRateMeasure(long val)
        {
            _theValue = val;
        }


        public static implicit operator IfcIntegerCountRateMeasure(long? value)
        {
            if (value.HasValue)
                return new IfcIntegerCountRateMeasure((long) value);
            else
                return new IfcIntegerCountRateMeasure();
        }

        public static implicit operator IfcIntegerCountRateMeasure(long value)
        {
            return new IfcIntegerCountRateMeasure(value);
        }

        public static implicit operator long(IfcIntegerCountRateMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator long(IfcIntegerCountRateMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcIntegerCountRateMeasure) obj)._theValue;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcIntegerCountRateMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcIntegerCountRateMeasure obj1, IfcIntegerCountRateMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcIntegerCountRateMeasure obj1, IfcIntegerCountRateMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcIntegerCountRateMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcIntegerCountRateMeasure) value).ToString());
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