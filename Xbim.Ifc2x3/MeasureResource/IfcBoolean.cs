#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoolean.cs
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
    public struct IfcBoolean : IPersistIfc, IfcSimpleValue
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.BooleanVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region ExpressType Members

        public string ToPart21
        {
            get { return string.Format(@".{0}.", _theValue ? "T" : "F"); }
        }

        #endregion

        private bool _theValue;

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
            return _theValue ? "true" : "false";
        }

        public IfcBoolean(bool val)
        {
            _theValue = val;
        }

        public IfcBoolean(string val)
        {
            if (string.Compare(val, "true", true) == 0)
                _theValue = true;
            else
                _theValue = false;
        }


        public static implicit operator IfcBoolean(bool value)
        {
            return new IfcBoolean(value);
        }

        public static implicit operator bool(IfcBoolean obj)
        {
            return obj._theValue;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcBoolean) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcBoolean obj1, IfcBoolean obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcBoolean obj1, IfcBoolean obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcBoolean? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcBoolean) value).ToString());
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