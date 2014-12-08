#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDaylightSavingHour.cs
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
    public struct IfcDaylightSavingHour : IPersistIfc, ExpressType
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

        public IfcDaylightSavingHour(int val)
        {
            _theValue = val;
        }


        public IfcDaylightSavingHour(string val)
        {
            _theValue = Convert.ToInt32(val);
        }

        public static implicit operator IfcDaylightSavingHour(int value)
        {
            return new IfcDaylightSavingHour(value);
        }

        public static implicit operator long(IfcDaylightSavingHour obj)
        {
            return (obj._theValue);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcDaylightSavingHour) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcDaylightSavingHour obj1, IfcDaylightSavingHour obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcDaylightSavingHour obj1, IfcDaylightSavingHour obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcDaylightSavingHour? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcDaylightSavingHour) value).ToString());
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
            if (_theValue < 0 || _theValue > 2)
                return
                    "WR1 DaylightSavingHour : Daylight saving number is always positive and can take the maximum value of 2 (hours) ahead of local time. Depending on the locality and the time of year, the value may be 0, 1 or 2.\n";
            else
                return "";
        }

        #endregion
    }
}