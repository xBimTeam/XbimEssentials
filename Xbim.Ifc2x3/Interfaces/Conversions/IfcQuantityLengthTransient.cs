using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityLengthTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityLength
    {
        private readonly IfcLengthMeasure _lengthValue;

        internal IfcQuantityLengthTransient()
        {
        }

        internal IfcQuantityLengthTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcLengthMeasure)) return;
            _lengthValue = new IfcLengthMeasure((MeasureResource.IfcLengthMeasure)value);
        }

        public IfcLengthMeasure LengthValue
        {
            get { return _lengthValue; }
            set { throw new NotSupportedException();}
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
