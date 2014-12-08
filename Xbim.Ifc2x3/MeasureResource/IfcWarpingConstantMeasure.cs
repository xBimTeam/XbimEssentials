#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWarpingConstantMeasure.cs
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
    public struct IfcWarpingConstantMeasure : IPersistIfc, IfcDerivedMeasureValue
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.RealVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region ExpressType Members

        public string ToPart21
        {
            get { return IfcReal.AsPart21(_theValue); }
        }

        #endregion

        private double _theValue;

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
            return IfcReal.AsPart21(_theValue);
            //string str = _theValue.ToString();
            //if (str.IndexOfAny(new[] {'.', 'E', 'e'}) == -1) str += ".";
            //return str;
        }

        public IfcWarpingConstantMeasure(double val)
        {
            _theValue = val;
        }


        public IfcWarpingConstantMeasure(string val)
        {
            _theValue = IfcReal.ToDouble(val);
        }

        public static implicit operator IfcWarpingConstantMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcWarpingConstantMeasure((double) value);
            else
                return new IfcWarpingConstantMeasure();
        }

        public static implicit operator IfcWarpingConstantMeasure(double value)
        {
            return new IfcWarpingConstantMeasure(value);
        }

        public static implicit operator double(IfcWarpingConstantMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcWarpingConstantMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcWarpingConstantMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcWarpingConstantMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcWarpingConstantMeasure obj1, IfcWarpingConstantMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcWarpingConstantMeasure obj1, IfcWarpingConstantMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcWarpingConstantMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcWarpingConstantMeasure) value).ToString());
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