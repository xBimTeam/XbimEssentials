#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcHourInDay.cs
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
    public struct IfcHourInDay : IPersistIfc, ExpressType
    {
        private int _theValue;

        public object Value
        {
            get { return _theValue; }
        }

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public IfcHourInDay(int val)
        {
            _theValue = val;
        }


        public IfcHourInDay(string val)
        {
            _theValue = Convert.ToInt32(val);
        }

        public override string ToString()
        {
            return _theValue.ToString();
        }

        public static implicit operator int(IfcHourInDay obj)
        {
            return (obj._theValue);
        }

        public static implicit operator IfcHourInDay(int val)
        {
            return new IfcHourInDay(val);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcHourInDay) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcHourInDay obj1, IfcHourInDay obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcHourInDay obj1, IfcHourInDay obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = (int) value.IntegerVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region ExpressType Members

        public string ToPart21
        {
            get { return _theValue.ToString(); }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue > 23 || _theValue < 0)
                return "HourInDay : Hours in the day must be between 0 and 23";
            else
                return "";
        }

        #endregion
    }
}