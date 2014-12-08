#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcInteger.cs
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
    public struct IfcInteger : IPersistIfc, IfcSimpleValue
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

        public IfcInteger(long val)
        {
            _theValue = val;
        }

        public IfcInteger(string sval)
        {
            _theValue = Convert.ToInt32(sval);
        }

        public static implicit operator IfcInteger(long value)
        {
            return new IfcInteger(value);
        }

        public static implicit operator long(IfcInteger obj)
        {
            return obj._theValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcInteger) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcInteger obj1, IfcInteger obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcInteger obj1, IfcInteger obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcInteger? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcInteger) value).ToString());
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