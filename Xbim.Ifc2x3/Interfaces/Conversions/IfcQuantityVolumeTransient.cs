using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityVolumeTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityVolume
    {
        private readonly IfcVolumeMeasure _volumeValue;

        internal IfcQuantityVolumeTransient()
        {
        }

        internal IfcQuantityVolumeTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcVolumeMeasure)) return;
            _volumeValue = new IfcVolumeMeasure((MeasureResource.IfcVolumeMeasure)value);
        }

        public IfcVolumeMeasure VolumeValue
        {
            get { return _volumeValue; }
            set {  throw new NotSupportedException(); }
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
