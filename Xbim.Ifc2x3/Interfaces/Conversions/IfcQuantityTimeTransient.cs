using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityTimeTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityTime
    {
        private readonly IfcTimeMeasure _timeValue;

        internal IfcQuantityTimeTransient()
        {
        }

        internal IfcQuantityTimeTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcTimeMeasure)) return;
            _timeValue = new IfcTimeMeasure((MeasureResource.IfcTimeMeasure)value);
        }

        public IfcTimeMeasure TimeValue
        {
            get { return _timeValue; }
            set { throw new NotSupportedException(); }
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
