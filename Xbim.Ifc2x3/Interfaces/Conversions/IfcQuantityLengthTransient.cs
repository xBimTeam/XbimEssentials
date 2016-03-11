using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityLengthTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityLength
    {
        internal IfcQuantityLengthTransient()
        {
        }

        internal IfcQuantityLengthTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcLengthMeasure)) return;
            LengthValue = new IfcLengthMeasure((MeasureResource.IfcLengthMeasure)value);
        }

        public IfcLengthMeasure LengthValue { get; internal set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
