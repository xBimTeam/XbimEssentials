using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4.Interfaces
{
    public static class MeasureWithUnitExtensions
    {
        public static IfcPhysicalSimpleQuantity ToPhysicalSimpleQuantity(this IfcMeasureWithUnit measure)
        {
            var model = measure.Model;
            var value = measure.ValueComponent;
            using (var txn = model.CurrentTransaction == null
                ? model.BeginTransaction("PhysicalSimpleQuantityTranslation")
                : null)
            {
                try
                {
                    var namedUnit = measure.UnitComponent as IfcNamedUnit;
                    //IfcQuantityArea, IfcQuantityCount, IfcQuantityLength, IfcQuantityTime, IfcQuantityVolume, IfcQuantityWeight
                    if (value is IfcAreaMeasure)
                    {
                        return model.Instances.New<IfcQuantityArea>(psq =>
                        {
                            psq.AreaValue = (IfcAreaMeasure) value;
                            psq.Unit = namedUnit;
                        });
                    }
                    if (value is IfcCountMeasure)
                    {
                        return model.Instances.New<IfcQuantityCount>(psq =>
                        {
                            psq.CountValue = (IfcCountMeasure)value;
                            psq.Unit = namedUnit;
                        });
                    }
                    if (value is IfcLengthMeasure)
                    {
                        return model.Instances.New<IfcQuantityLength>(psq =>
                        {
                            psq.LengthValue = (IfcLengthMeasure)value;
                            psq.Unit = namedUnit;
                        });
                    }
                    if (value is IfcTimeMeasure)
                    {
                        return model.Instances.New<IfcQuantityTime>(psq =>
                        {
                            psq.TimeValue = (IfcTimeMeasure)value;
                            psq.Unit = namedUnit;
                        });
                    }
                    if (value is IfcVolumeMeasure)
                    {
                        return model.Instances.New<IfcQuantityVolume>(psq =>
                        {
                            psq.VolumeValue = (IfcVolumeMeasure)value;
                            psq.Unit = namedUnit;
                        });
                    }
                    if (value is IfcMassMeasure)
                    {
                        return model.Instances.New<IfcQuantityWeight>(psq =>
                        {
                            psq.WeightValue = (IfcMassMeasure)value;
                            psq.Unit = namedUnit;
                        });
                    }
                    return null;
                }
                finally
                {
                    if(txn != null)
                        txn.Commit();
                }
            }
        }
    }
}
