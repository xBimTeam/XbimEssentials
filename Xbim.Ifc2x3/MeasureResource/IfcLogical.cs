#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLogical.cs
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
    public struct IfcLogical : IPersistIfc, IfcSimpleValue
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
            get
            {
                if (_theValue.HasValue)
                    return string.Format(@".{0}.", _theValue.Value ? "T" : "F");
                else
                    return ".U.";
            }
        }

        #endregion

        private bool? _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof(bool?); }
        }

        public bool IsUnknown
        {
            get { return !_theValue.HasValue; }
        }

        public bool IsTrue
        {
            get { return _theValue.HasValue && _theValue.Value; }
        }

        public bool IsFalse
        {
            get { return _theValue.HasValue && !_theValue.Value; }
        }

        public object Value
        {
            get { return _theValue; }
        }

        public override string ToString()
        {
            if (_theValue.HasValue)
                return _theValue.Value ? "true" : "false";
            else
                return "unknown";
        }

        public IfcLogical(bool val)
        {
            _theValue = val;
        }

        public IfcLogical(string val)
        {
            if (string.Compare(val, "true", true) == 0)
                _theValue = true;
            else if (string.Compare(val, "false", true) == 0)
                _theValue = false;
            else
                _theValue = null;
        }


        public static implicit operator IfcLogical(bool value)
        {
            return new IfcLogical(value);
        }

        public static implicit operator bool?(IfcLogical obj)
        {
            return obj._theValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcLogical) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcLogical obj1, IfcLogical obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcLogical obj1, IfcLogical obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}
