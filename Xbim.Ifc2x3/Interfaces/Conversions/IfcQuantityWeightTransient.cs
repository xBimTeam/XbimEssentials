using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityWeightTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityWeight
    {
        private readonly IfcMassMeasure _weightValue;

        internal IfcQuantityWeightTransient()
        {
        }

        internal IfcQuantityWeightTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcMassMeasure)) return;
            _weightValue = new IfcMassMeasure((MeasureResource.IfcMassMeasure)value);
        }

        public IfcMassMeasure WeightValue
        {
            get { return _weightValue; }
            set { throw new NotSupportedException(); }
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
