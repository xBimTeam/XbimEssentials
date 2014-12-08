#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCompoundPlaneAngleMeasure.cs
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
    public struct IfcCompoundPlaneAngleMeasure : IPersistIfc, IfcDerivedMeasureValue, ExpressComplexType
    {
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (_count)
            {
                case 0:
                    _measure1 = (int) value.IntegerVal;
                    break;
                case 1:
                    _measure2 = (int) value.IntegerVal;
                    break;
                case 2:
                    _measure3 = (int) value.IntegerVal;
                    break;
                case 3:
                    _millionthsOfaSecond = (int) value.IntegerVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
            if (_count >= 3)
                _count = 0; //restart if reloading
            else
                _count++;
        }

        #endregion

        #region ExpressType Members

        public string ToPart21
        {
            get { return String.Format("({0}, {1}, {2})", _measure1, _measure2, _measure3); }
        }

        Type ExpressType.UnderlyingSystemType
        {
            get { return typeof (ExpressComplexType); }
        }

        #endregion

        private sbyte _count;

        public object Value
        {
            get { throw new Exception(GetType().Name + " - Express Complex Types do not support simple value operations"); }
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", _measure1, _measure2, _measure3);
        }

        public int Degrees
        {
            get { return _measure1; }
        }

        public int Minutes
        {
            get { return _measure2; }
        }

        public int Seconds
        {
            get { return _measure3; }
        }

        public int? MillionthsOfaSecond
        {
            get { return _millionthsOfaSecond; }
        }

        public double ToDouble()
        {
            double main = _measure1 + (_measure2/60.0) + (_measure3/3600.0);
            if (_millionthsOfaSecond.HasValue)
                main += (_millionthsOfaSecond.Value)/3600e6;
            return main;
        }

        #region Part 21 Step file Parse routines

        //Part 21 Private Variables
        private int _measure1;
        private int _measure2;
        private int _measure3;
        private int? _millionthsOfaSecond;


        //Read Part21 Tokens

        #endregion

        public IfcCompoundPlaneAngleMeasure(double degreeAngle)
        {
            int sign = (int) (Math.Abs(degreeAngle)/degreeAngle); //all parts should have the same sign [+/-]
            degreeAngle = Math.Abs(degreeAngle);

            _measure3 = (int) Math.Round(degreeAngle*3600);
            _measure1 = sign*_measure3/3600;
            _measure3 = _measure3%3600;
            _measure2 = sign*_measure3/60;
            _measure3 = sign*_measure3%60;
            double tempAngle = _measure1 + (_measure2/60.0) + (_measure3/3600.0);
            _millionthsOfaSecond = (int) ((degreeAngle - tempAngle)*3600e6);
            _count = 4;
        }

        public IfcCompoundPlaneAngleMeasure(int first, int second, int third, int millionthsOfaSecond)
        {
            _measure1 = first;
            _measure2 = second;
            _measure3 = third;
            _millionthsOfaSecond = millionthsOfaSecond;
            _count = 4;
        }

        public IfcCompoundPlaneAngleMeasure(int first, int second, int third)
        {
            _measure1 = first;
            _measure2 = second;
            _measure3 = third;
            _millionthsOfaSecond = null;
            _count = 3;
        }

        public long Count
        {
            get { return _count; }
        }

        internal IfcCompoundPlaneAngleMeasure Add(int value)
        {
            if (_count > 3) //roll over
                _count = 0;
            switch (_count)
            {
                case 0:
                    _measure1 = value;
                    break;
                case 1:
                    _measure2 = value;
                    break;
                case 2:
                    _measure3 = value;
                    break;
                case 3:
                    _millionthsOfaSecond = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Index out of bounds - CompoundPlaneAngleMeasure.book");
            }
            _count++;
            return this;
        }

        public int this[long index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _measure1;
                    case 1:
                        return _measure2;
                    case 2:
                        return _measure3;
                    case 3:
                        return _millionthsOfaSecond ?? 0;
                    default:
                        throw new ArgumentOutOfRangeException("Index out of bounds - CompoundPlaneAngleMeasure");
                }
            }
        }

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            string err = "";
            if ((-360 > _measure1) || (_measure1 >= 360))
                err += "WR1 CompoundPlaneAngleMeasure : The first angle must be between -360 and 359 degrees\n";
            if ((-60 > _measure2) || (_measure2 >= 60))
                err += "WR2 CompoundPlaneAngleMeasure : The second angle must be between -60 and 59 degrees\n";
            if ((-60 > _measure3) || (_measure3 >= 60))
                err += "WR3 CompoundPlaneAngleMeasure : The second angle must be between -60 and 59 degrees\n";
            if (((_measure1 >= 0) && (_measure2 >= 0) && (_measure3 >= 0)) ||
                ((_measure1 <= 0) && (_measure2 <= 0) && (_measure3 <= 0)))
                return err;
            else
                return
                    err +=
                    "WR3 CompoundPlaneAngleMeasure : The measure components shall have the same sign (positive or negative).\n";
        }

        #endregion

        #region ExpressComplexType Members

        public IEnumerable<object> Properties
        {
            get
            {
                // List<object> props = new List<object>(1);
                XbimListProxy<int> measures = new XbimListProxy<int>("list");
                measures.Add(_measure1);
                measures.Add(_measure2);
                measures.Add(_measure3);
                if (_millionthsOfaSecond.HasValue) measures.Add(_millionthsOfaSecond.Value);
                return measures.Cast<object>();
            }
        }

        #endregion

        #region ExpressComplexType Members

        public void Add(object o)
        {
            this.Add(Convert.ToInt32((long) o));
        }

        #endregion
    }
}