using Xbim.Ifc2x3.Interfaces.Conversions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4.Interfaces
{
    internal static class MeasureWithUnitExtensions
    {
        public static IIfcPhysicalSimpleQuantity ToPhysicalSimpleQuantity(this IfcMeasureWithUnit measure)
        {
            var value = measure.ValueComponent;

            //IfcQuantityArea, IfcQuantityCount, IfcQuantityLength, IfcQuantityTime, IfcQuantityVolume, IfcQuantityWeight
            if (value is IfcAreaMeasure)
            {
                return new IfcQuantityAreaTransient(measure);
            }
            if (value is IfcCountMeasure)
            {
                return new IfcQuantityCountTransient(measure);
            }
            if (value is IfcLengthMeasure)
            {
                return new IfcQuantityLengthTransient(measure);
            }
            if (value is IfcTimeMeasure)
            {
                return new IfcQuantityTimeTransient(measure);
            }
            if (value is IfcVolumeMeasure)
            {
                return new IfcQuantityVolumeTransient(measure);
            }
            if (value is IfcMassMeasure)
            {
                return new IfcQuantityWeightTransient(measure);
            }
            return null;
        }
    }
}
