using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityTimeTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityTime
    {
        internal IfcQuantityTimeTransient()
        {
        }

        internal IfcQuantityTimeTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcTimeMeasure)) return;
            TimeValue = new IfcTimeMeasure((MeasureResource.IfcTimeMeasure)value);
        }

        public IfcTimeMeasure TimeValue { get; internal set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
