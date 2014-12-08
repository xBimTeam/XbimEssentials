using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.ControlExtension
{
    /// <summary>
    /// Definition from IAI: The IfcTimeSeriesSchedule defines a time-series that is applicable to to one or more calendar dates. It typically contains a periodically repetitive time series used to define the schedule, facilitating the capture of hours of operation, occupancy loads, etc.
    /// 
    ///     HISTORY: New entity in Release IFC2x Edition 2.
    /// 
    /// Informal proposition:
    /// 
    ///     If Intent : IfcConstraintIntentEnum is set to value UserDefined, then the intent shall be given by using inherited attribute IfcObject.ObjectType.
    /// 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTimeSeriesSchedule: IfcControl
    {
        public IfcTimeSeriesSchedule()
        {
            _ApplicableDates = new XbimSet<IfcDateTimeSelect>(this);
        }

        private XbimSet<IfcDateTimeSelect> _ApplicableDates;

        /// <summary>
        /// Defines an ordered list of the dates for which the time-series data are applicable. For example, the definition of all public holiday dates for a given year allows the formulation of a "holiday" occupancy schedule from overall occupancy data. Local time can be used if the dates are not bound to a particular year. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcDateTimeSelect> ApplicableDates
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ApplicableDates;
            }
            set { this.SetModelValue(this, ref _ApplicableDates, value, v => ApplicableDates = v, "ApplicableDates"); }
        }

        private IfcTimeSeriesScheduleTypeEnum _TimeSeriesScheduleType;

        /// <summary>
        /// Defines the type of schedule, such as daily, weekly, monthly or annually. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcTimeSeriesScheduleTypeEnum TimeSeriesScheduleType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TimeSeriesScheduleType;
            }
            set { this.SetModelValue(this, ref _TimeSeriesScheduleType, value, v => TimeSeriesScheduleType = v, "TimeSeriesScheduleType"); }
        }

        private IfcTimeSeries _TimeSeries;

        /// <summary>
        /// The time series is used to represent the values at discrete points in time that define the schedule. For example, a 24-hour occupancy schedule would be a regular time series with a start time at midnight, end time at (the following) midnight, and with 24 values indicating the occupancy load for each hour of the 24-hour period. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcTimeSeries TimeSeries
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TimeSeries;
            }
            set { this.SetModelValue(this, ref _TimeSeries, value, v => TimeSeries = v, "TimeSeries"); }
        }

        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (TimeSeriesScheduleType == IfcTimeSeriesScheduleTypeEnum.USERDEFINED && ObjectType == null)
                result += "WR41: The type of IfcTimeSeriesSchedule shall be given by inherited attribute ObjectType, if the enumeration value in TimeSeriesScheduleType is set to USERDEFINED. \n";

            return result;
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _ApplicableDates.Add((IfcDateTimeSelect)value.EntityVal);
                    break;
                case 6:
                    _TimeSeriesScheduleType = (IfcTimeSeriesScheduleTypeEnum)Enum.Parse(typeof(IfcTimeSeriesScheduleTypeEnum), value.EnumVal);
                    break;
                case 7:
                    _TimeSeries = (IfcTimeSeries)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
