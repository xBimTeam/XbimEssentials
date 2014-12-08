#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDayInMonthNumber.cs
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

namespace Xbim.Ifc2x3.DateTimeResource
{
    [Serializable]
    public struct IfcDayInMonthNumber : IPersistIfc, ExpressType
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = (int) value.IntegerVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        private int _theValue;

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


        public IfcDayInMonthNumber(int val)
        {
            _theValue = val;
        }


        public IfcDayInMonthNumber(string val)
        {
            _theValue = Convert.ToInt32(val);
        }


        public static implicit operator IfcDayInMonthNumber(int? value)
        {
            if (value.HasValue)
                return new IfcDayInMonthNumber((int) value);
            else
                return new IfcDayInMonthNumber();
        }

        public static implicit operator IfcDayInMonthNumber(int value)
        {
            return new IfcDayInMonthNumber(value);
        }

        public static implicit operator long(IfcDayInMonthNumber obj)
        {
            return (obj._theValue);
        }

        public static implicit operator int(IfcDayInMonthNumber obj)
        {
            return obj._theValue;
        }

        public static explicit operator long(IfcDayInMonthNumber? obj)
        {
            if (obj.HasValue)
                return ((IfcDayInMonthNumber) obj)._theValue;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcDayInMonthNumber) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcDayInMonthNumber obj1, IfcDayInMonthNumber obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcDayInMonthNumber obj1, IfcDayInMonthNumber obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcDayInMonthNumber? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcDayInMonthNumber) value).ToString());
            else
                return new StepP21Token("$");
        }

        #region ExpressType Members

        public string ToPart21
        {
            get { return _theValue.ToString(); }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue < 1 || _theValue > 31)
                return "WR1 DayInMonthNumber: Day in Month must be between 1 and 31";
            else
                return "";
        }

        #endregion
    }
}