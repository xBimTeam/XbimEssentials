#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDimensionCount.cs
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

namespace Xbim.Ifc2x3.GeometryResource
{
    [Serializable]
    public struct IfcDimensionCount : ExpressType, IPersistIfc
    {
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

        public IfcDimensionCount(int val)
        {
            _theValue = val;
        }


        public IfcDimensionCount(long val)
            : this((int) val)
        {
        }

        public IfcDimensionCount(string val)
            : this(Convert.ToInt32(val))
        {
        }


        public static implicit operator IfcDimensionCount(int value)
        {
            return new IfcDimensionCount(value);
        }

        public static implicit operator int(IfcDimensionCount obj)
        {
            return (obj._theValue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcDimensionCount) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcDimensionCount obj1, IfcDimensionCount obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator ==(IfcDimensionCount obj1, int obj2)
        {
            return Equals(obj1._theValue, obj2);
        }

        public static bool operator !=(IfcDimensionCount obj1, IfcDimensionCount obj2)
        {
            return !Equals(obj1, obj2);
        }

        public static bool operator !=(IfcDimensionCount obj1, int obj2)
        {
            return !Equals(obj1._theValue, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        #region ExpressType Members

        public string ToPart21
        {
            get { return _theValue.ToString(); }
        }

        #endregion

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = (int)value.IntegerVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            if (_theValue < 0 || _theValue > 3)
                return
                    "WR1 DimensionCount : The dimension count should be an integer between 1 and 3 NOTE: This is a further constraint by IFC, the upper limit does not exist in STEP./n";
            else
                return "";
        }

        #endregion
    }
}