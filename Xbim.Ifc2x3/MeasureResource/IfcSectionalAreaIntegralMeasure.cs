#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSectionalAreaIntegralMeasure.cs
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
    public struct IfcSectionalAreaIntegralMeasure : IPersistIfc, IfcDerivedMeasureValue
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

        public IfcSectionalAreaIntegralMeasure(double val)
        {
            _theValue = val;
        }


        public IfcSectionalAreaIntegralMeasure(string val)
        {
            _theValue = IfcReal.ToDouble(val);
        }

        public static implicit operator IfcSectionalAreaIntegralMeasure(double? value)
        {
            if (value.HasValue)
                return new IfcSectionalAreaIntegralMeasure((double) value);
            else
                return new IfcSectionalAreaIntegralMeasure();
        }

        public static implicit operator IfcSectionalAreaIntegralMeasure(double value)
        {
            return new IfcSectionalAreaIntegralMeasure(value);
        }

        public static implicit operator double(IfcSectionalAreaIntegralMeasure obj)
        {
            return (obj._theValue);
        }

        public static explicit operator double(IfcSectionalAreaIntegralMeasure? obj)
        {
            if (obj.HasValue)
                return ((IfcSectionalAreaIntegralMeasure) obj)._theValue;
            else
                return 0.0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcSectionalAreaIntegralMeasure) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcSectionalAreaIntegralMeasure obj1, IfcSectionalAreaIntegralMeasure obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcSectionalAreaIntegralMeasure obj1, IfcSectionalAreaIntegralMeasure obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcSectionalAreaIntegralMeasure? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcSectionalAreaIntegralMeasure) value).ToString());
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