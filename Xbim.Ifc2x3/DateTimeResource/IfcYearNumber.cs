#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcYearNumber.cs
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
    public struct IfcYearNumber : IPersistIfc, ExpressType
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

        #region ExpressType Members

        public string ToPart21
        {
            get { return _theValue.ToString(); }
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


        public IfcYearNumber(int val)
        {
            _theValue = val;
        }


        public IfcYearNumber(string val)
        {
            _theValue = Convert.ToInt32(val);
        }

        public static implicit operator IfcYearNumber(int? value)
        {
            if (value.HasValue)
                return new IfcYearNumber((int) value);
            else
                return new IfcYearNumber();
        }

        public static implicit operator IfcYearNumber(int value)
        {
            return new IfcYearNumber(value);
        }

        public static implicit operator long(IfcYearNumber obj)
        {
            return (obj._theValue);
        }

        public static implicit operator int(IfcYearNumber obj)
        {
            return obj._theValue;
        }

        public static explicit operator long(IfcYearNumber? obj)
        {
            if (obj.HasValue)
                return ((IfcYearNumber) obj)._theValue;
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcYearNumber) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcYearNumber obj1, IfcYearNumber obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcYearNumber obj1, IfcYearNumber obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcYearNumber? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcYearNumber) value).ToString());
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