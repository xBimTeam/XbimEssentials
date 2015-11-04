using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    public class IfcQuantityWeightTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityWeight
    {
        internal IfcQuantityWeightTransient()
        {
        }

        internal IfcQuantityWeightTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcMassMeasure)) return;
            WeightValue = new IfcMassMeasure((MeasureResource.IfcMassMeasure)value);
        }

        public IfcMassMeasure WeightValue { get; internal set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
