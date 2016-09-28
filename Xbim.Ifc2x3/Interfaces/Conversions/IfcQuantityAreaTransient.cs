using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityAreaTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityArea
    {
        private readonly IfcAreaMeasure _areaValue;

        internal IfcQuantityAreaTransient()
        {
        }

        internal IfcQuantityAreaTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcAreaMeasure)) return;
            _areaValue = new IfcAreaMeasure((MeasureResource.IfcAreaMeasure)value);
        }

        public IfcAreaMeasure AreaValue
        {
            get { return _areaValue; }
            set { throw new NotSupportedException(); }
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
