#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMinuteInHour.cs
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
    public struct IfcMinuteInHour : IPersistIfc, ExpressType
    {
        private int _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public IfcMinuteInHour(int val)
        {
            _theValue = val;
        }

        public object Value
        {
            get { return _theValue; }
        }

        public IfcMinuteInHour(string val)
        {
            _theValue = Convert.ToInt32(val);
        }

        public override string ToString()
        {
            return _theValue.ToString();
        }

        public static implicit operator int(IfcMinuteInHour obj)
        {
            return (obj._theValue);
        }

        public static implicit operator IfcMinuteInHour(int val)
        {
            return new IfcMinuteInHour(val);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcMinuteInHour) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcMinuteInHour obj1, IfcMinuteInHour obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcMinuteInHour obj1, IfcMinuteInHour obj2)
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
            if (_theValue > 59 || _theValue < 0)
                return "WR1 MinuteInHour: Minutes in the hour must have a value between 0 and 59";
            else
                return "";
        }

        #endregion
    }
}