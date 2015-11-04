using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    public class IfcQuantityVolumeTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityVolume
    {
        internal IfcQuantityVolumeTransient()
        {
        }

        internal IfcQuantityVolumeTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            Unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcVolumeMeasure)) return;
            VolumeValue = new IfcVolumeMeasure((MeasureResource.IfcVolumeMeasure)value);
        }

        public IfcVolumeMeasure VolumeValue { get; internal set; }
        public IfcLabel? Formula { get; internal set; }
    }
}
