using System;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    internal class IfcQuantityCountTransient : IfcPhysicalSimpleQuantityTransient, Ifc4.Interfaces.IIfcQuantityCount
    {
        private readonly IfcCountMeasure _countValue;

        internal IfcQuantityCountTransient()
        {
        }

        internal IfcQuantityCountTransient(MeasureResource.IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;
            _unit = measure.UnitComponent as Ifc4.Interfaces.IIfcNamedUnit;
            if (!(value is MeasureResource.IfcCountMeasure)) return;
            _countValue = new IfcCountMeasure((MeasureResource.IfcCountMeasure)value);
        }

        public IfcCountMeasure CountValue
        {
            get { return _countValue; }
            set { throw  new NotSupportedException();}
        }

        public IfcLabel? Formula
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }
    }
}
