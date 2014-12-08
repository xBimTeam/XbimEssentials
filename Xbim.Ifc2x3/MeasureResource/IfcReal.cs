#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReal.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using System.Globalization;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [Serializable]
    public struct IfcReal : IPersistIfc, IfcSimpleValue
    {
        private double _theValue;

        Type ExpressType.UnderlyingSystemType
        {
            get { return _theValue.GetType(); }
        }

        public IfcReal(double val)
        {
            _theValue = val;
        }


        public IfcReal(string val)
        {
            _theValue = ToDouble(val);
        }

        #region ExpressType Members

        public string ToPart21
        {
            get { return AsPart21(_theValue); }
        }

        #endregion

        public object Value
        {
            get { return _theValue; }
        }

        public override string ToString()
        {
            return AsPart21(_theValue);
        }

        public static string AsPart21(double real)
        {
            double dArg = (double)real;
            string result = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

            // if compiler flag, only then do the following 3 lines
            string rDoubleStr = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));
            double fixedDbl = double.Parse(rDoubleStr, CultureInfo.CreateSpecificCulture("en-US"));
            result = fixedDbl.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

            if (!result.Contains("."))
            {
                if (result.Contains("E"))
                    result = result.Replace("E", ".E");
                else
                    result += ".";
            }

            return result;
        }


        public static implicit operator IfcReal(double value)
        {
            return new IfcReal(value);
        }

        public static implicit operator double(IfcReal obj)
        {
            return (obj._theValue);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;
            return ((IfcReal) obj)._theValue == _theValue;
        }

        public static bool operator ==(IfcReal obj1, IfcReal obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcReal obj1, IfcReal obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return _theValue.GetHashCode();
        }

        public static explicit operator StepP21Token(IfcReal? value)
        {
            if (value.HasValue)
                return new StepP21Token(((IfcReal) value).ToString());
            else
                return new StepP21Token("$");
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _theValue = value.RealVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion

        public static double ToDouble(string val)
        {
            return Convert.ToDouble(val, CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}