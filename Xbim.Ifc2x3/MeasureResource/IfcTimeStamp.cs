#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTimeStamp.cs
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
    public struct IfcTimeStamp : IPersistIfc, IfcDerivedMeasureValue
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

        public static DateTime ToDateTime(IfcTimeStamp timeStamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from 1970/1/1 00:00:00
            return (dt.AddSeconds(timeStamp));
        }

        public static IfcTimeStamp ToTimeStamp(DateTime dateTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //from 1970/1/1 00:00:00 to _lastModifiedDate
            TimeSpan result = dateTime.Subtract(dt);
            int seconds = Convert.ToInt32(result.TotalSeconds);
            return new IfcTimeStamp(seconds);
        }

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

        public IfcTimeStamp(long val)
        {
            _theValue = val;
        }

        public IfcTimeStamp(string val)
        {
            _theValue = Convert.ToInt64(val);
        }


        public static implicit operator IfcTimeStamp(long? value)
        {
            if (value.HasValue)
                return new IfcTimeStamp((long) value);
            else
                return new IfcTimeStamp();
        }

        public static implicit operator IfcTimeStamp(long value)
        {
            return new IfcTimeStamp(value);
        }

        public static implicit operator long(IfcTimeStamp obj)
        {
            return (obj._theValue);
        }

        public static explicit operator long(IfcTimeStamp? obj)
        {
            if (obj.HasValue)
                return ((IfcTimeStamp) obj)._theValue;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcTimeStamp) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcTimeStamp obj1, IfcTimeStamp obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcTimeStamp obj1, IfcTimeStamp obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcTimeStamp? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcTimeStamp) value).ToString());
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