using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    public class IfcQuantityAreaTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityArea
    {
        internal IfcQuantityAreaTransient()
        {
        }

        internal IfcQuantityAreaTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcAreaMeasure)) return;
            AreaValue = new IfcAreaMeasure((MeasureResource.IfcAreaMeasure)value);
        }

        public IfcAreaMeasure AreaValue { get; internal set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
