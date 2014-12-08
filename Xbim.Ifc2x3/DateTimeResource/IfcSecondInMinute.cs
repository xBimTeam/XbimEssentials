#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSecondInMinute.cs
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
    public struct IfcSecondInMinute : IPersistIfc, ExpressType
    {
        private double _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public IfcSecondInMinute(double val)
        {
            if (val >= 60 || val < 0)
                throw new ArgumentOutOfRangeException("Seconds in a minute must have a value between 0 and 59");
            _theValue = val;
        }

        public object Value
        {
            get { return _theValue; }
        }

        public static implicit operator double(IfcSecondInMinute obj)
        {
            return (obj._theValue);
        }

        public override string ToString()
        {
            return _theValue.ToString();
        }

        public static implicit operator IfcSecondInMinute(double val)
        {
            return new IfcSecondInMinute(val);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcSecondInMinute) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcSecondInMinute obj1, IfcSecondInMinute obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcSecondInMinute obj1, IfcSecondInMinute obj2)
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
                _theValue = value.RealVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }


        public string WhereRule()
        {
            if (_theValue < 0 || _theValue >= 60)
                return "WR1 SecondInMinute : The value of the real number shall be between 0 to 59.";
            else
                return "";
        }

        #endregion

        #region ExpressType Members

        public string ToPart21
        {
            get { return _theValue.ToString(); }
        }

        #endregion
    }
}