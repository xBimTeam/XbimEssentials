#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcComplexNumber.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    [Serializable]
    public struct IfcComplexNumber : IPersistIfc, IfcMeasureValue, ExpressComplexType
    {
        private double _real;
        private double _imaginary;
        private bool _init;

        #region ExpressType Members

        public string ToPart21
        {
            get { return string.Format("{0}, {1}", IfcReal.AsPart21(_real), IfcReal.AsPart21(_imaginary)); }
        }

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof (ExpressComplexType); }
        }

        #endregion

        public object Value
        {
            get { throw new Exception(GetType().Name + " - Express Complex Types do not support simple value operations"); }
        }

        public IfcComplexNumber(double real, double imaginary)
        {
            _real = real;
            _imaginary = imaginary;
            _init = true;
        }

        public override string ToString()
        {
            return (String.Format("{0} + {1}i", _real, _imaginary));
        }

        public static IfcComplexNumber operator +(IfcComplexNumber c1, IfcComplexNumber c2)
        {
            return new IfcComplexNumber(c1._real + c2._real, c1._imaginary + c2._imaginary);
        }

        public double Real
        {
            get { return _real; }
        }

        public double Imaginary
        {
            get { return _imaginary; }
        }


        public override bool Equals(object obj)
        {
            return (((IfcComplexNumber) obj).Real == Real && ((IfcComplexNumber) obj).Imaginary == Imaginary);
        }

        public static bool operator ==(IfcComplexNumber obj1, IfcComplexNumber obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(IfcComplexNumber obj1, IfcComplexNumber obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            // first time this is called InitProperties values for future loading
            if (!_init)
            {
                _real = double.NaN;
                _imaginary = double.NaN;
                _init = true;
            }
            switch (propIndex)
            {
                case 0:

                    if (double.IsNaN(_real))
                        _real = value.RealVal;
                    else if (double.IsNaN(_imaginary))
                        _imaginary = value.RealVal;
                    else
                        throw new ArgumentOutOfRangeException(string.Format("P21 index value out of range in {0}",
                                                                            this.GetType().Name));
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion

        #region ExpressComplexType Members

        public IEnumerable<object> Properties
        {
            get
            {
                // List<object> props = new List<object>(1);
                XbimListProxy<double> vals = new XbimListProxy<double>("array");
                vals.Add(_real);
                vals.Add(_imaginary);
                //  props.Add(vals);
                return vals.Cast<object>();
            }
        }

        #endregion

        #region ExpressComplexType Members

        public void Add(object o)
        {
            if (double.IsNaN(_real))
                _real = (double) o;
            else if (double.IsNaN(_imaginary))
                _imaginary = (double) o;
            else
                throw new ArgumentOutOfRangeException(string.Format("P21 index value out of range in {0}",
                                                                    this.GetType().Name));
        }

        #endregion
    }
}