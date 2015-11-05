using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityCountTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityCount
    {
        internal IfcQuantityCountTransient()
        {
        }

        internal IfcQuantityCountTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcCountMeasure)) return;
            CountValue = new IfcCountMeasure((MeasureResource.IfcCountMeasure)value);
        }

        public IfcCountMeasure CountValue { get; private set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
