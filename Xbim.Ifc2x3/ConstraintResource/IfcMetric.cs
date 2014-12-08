#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMetric.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ConstraintResource
{
    public class IfcMetric : IfcConstraint
    {
        #region Fields

        private IfcBenchmarkEnum _benchmark;
        private IfcLabel _valueSource;
        private IfcMetricValueSelect _dataValue;

        #endregion

        /// <summary>
        ///   Enumeration that identifies the type of benchmark data.
        /// </summary>
        [Ifc(8, IfcAttributeState.Mandatory)]
        public IfcBenchmarkEnum Benchmark

        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _benchmark;
            }
            set { this.SetModelValue(this, ref _benchmark, value, v => Benchmark = v, " Benchmark"); }
        }

        /// <summary>
        ///   Reference source for data values.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel ValueSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _valueSource;
            }
            set { this.SetModelValue(this, ref _valueSource, value, v => ValueSource = v, "ValueSource"); }
        }

        /// <summary>
        ///   Value with data type defined by the DataType enumeration
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcMetricValueSelect DataValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dataValue;
            }
            set { this.SetModelValue(this, ref _dataValue, value, v => DataValue = v, "DataValue"); }
        }

        #region ISupportIfcParser Members

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _benchmark = (IfcBenchmarkEnum) Enum.Parse(typeof (IfcBenchmarkEnum), value.EnumVal);
                    break;
                case 8:
                    _valueSource = value.StringVal;
                    break;
                case 9:
                    _dataValue = (IfcMetricValueSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}