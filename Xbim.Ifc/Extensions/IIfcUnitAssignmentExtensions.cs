using System;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public enum ConversionBasedUnit
    {
        Inch,
        Foot,
        Yard,
        Mile,
        Acre,
        Litre,
        PintUk,
        PintUs,
        GallonUk,
        GallonUs,
        Ounce,
        Pound,
        SquareFoot,
        CubicFoot
    }
    public static class IIfcUnitAssignmentExtensions
    {
        public static double Power(this IIfcUnitAssignment obj, IfcUnitEnum unitType)
        {
            var siUnit = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (siUnit != null && siUnit.Prefix.HasValue)
                return siUnit.Power;
            var conversionUnit =
                obj.Units.OfType<IIfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (conversionUnit == null) return 1.0;
            var measureUnit = conversionUnit.ConversionFactor;
            var uc = measureUnit.UnitComponent as IIfcSIUnit;
            //some BIM tools such as StruCAD write the conversion value out as a Length Measure
            if (uc == null) return 1.0;


            var et = ((IExpressValueType)measureUnit.ValueComponent);
            var cFactor = 1.0;
            if (et.UnderlyingSystemType == typeof(double))
                cFactor = (double)et.Value;
            else if (et.UnderlyingSystemType == typeof(int))
                cFactor = (int)et.Value;
            else if (et.UnderlyingSystemType == typeof(long))
                cFactor = (long)et.Value;

            return uc.Power * cFactor;
        }

        /// <summary>
        /// Sets the Length Unit to be SIUnit and SIPrefix, returns false if the units are not SI
        /// </summary>
        /// <param name="obj"></param>
        /// <param name = "siUnitName"></param>
        /// <param name = "siPrefix"></param>
        /// <returns></returns>
        public static bool SetSiLengthUnits(this IIfcUnitAssignment obj, IfcSIUnitName siUnitName, IfcSIPrefix? siPrefix)
        {
            var si = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (si != null)
            {
                si.Prefix = siPrefix;
                si.Name = siUnitName;
                return true;
            }
            return false;
        }

        public static void SetOrChangeSiUnit(this IIfcUnitAssignment obj, IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                             IfcSIPrefix? siUnitPrefix)
        {
            var model = obj.Model;
            var si = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                si.Prefix = siUnitPrefix;
                si.Name = siUnitName;
            }
            else
            {
                var factory = new EntityCreator(model);
                obj.Units.Add(factory.SIUnit(s =>
                {
                    s.UnitType = unitType;
                    s.Name = siUnitName;
                    s.Prefix = siUnitPrefix;
                }));
            }
        }

        /// <summary>
        /// Gets the unit for a <see cref="IIfcPropertySingleValue"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IIfcNamedUnit GetUnitFor(this IIfcUnitAssignment obj, IIfcPropertySingleValue property)
        {

            if (property.Unit != null)
                return (IIfcNamedUnit)property.Unit;

            // nominal value can be of types with subtypes:
            //	IfcMeasureValue, IfcSimpleValue, IfcDerivedMeasureValue

            IfcUnitEnum? requiredUnit = property.NominalValue switch
            {
                Ifc4.MeasureResource.IfcVolumeMeasure => (IfcUnitEnum?)IfcUnitEnum.VOLUMEUNIT,
                Ifc4.MeasureResource.IfcAreaMeasure => (IfcUnitEnum?)IfcUnitEnum.AREAUNIT,
                Ifc4.MeasureResource.IfcLengthMeasure => (IfcUnitEnum?)IfcUnitEnum.LENGTHUNIT,
                Ifc4.MeasureResource.IfcPositiveLengthMeasure => (IfcUnitEnum?)IfcUnitEnum.LENGTHUNIT,
                Ifc4.MeasureResource.IfcNonNegativeLengthMeasure => (IfcUnitEnum?)IfcUnitEnum.LENGTHUNIT,
                Ifc4.MeasureResource.IfcAmountOfSubstanceMeasure => (IfcUnitEnum?)IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT,
                Ifc4.MeasureResource.IfcElectricCurrentMeasure => (IfcUnitEnum?)IfcUnitEnum.ELECTRICCURRENTUNIT,
                Ifc4.MeasureResource.IfcLuminousIntensityMeasure => (IfcUnitEnum?)IfcUnitEnum.LUMINOUSINTENSITYUNIT,
                Ifc4.MeasureResource.IfcMassMeasure => (IfcUnitEnum?)IfcUnitEnum.MASSUNIT,
                Ifc4.MeasureResource.IfcPlaneAngleMeasure => (IfcUnitEnum?)IfcUnitEnum.PLANEANGLEUNIT,
                Ifc4.MeasureResource.IfcPositivePlaneAngleMeasure => (IfcUnitEnum?)IfcUnitEnum.PLANEANGLEUNIT,
                Ifc4.MeasureResource.IfcThermodynamicTemperatureMeasure => (IfcUnitEnum?)IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT,
                Ifc4.MeasureResource.IfcSolidAngleMeasure => (IfcUnitEnum?)IfcUnitEnum.SOLIDANGLEUNIT,
                Ifc4.MeasureResource.IfcTimeMeasure => (IfcUnitEnum?)IfcUnitEnum.TIMEUNIT,

                Ifc4.MeasureResource.IfcContextDependentMeasure => null,// todo: not sure what to do here
                Ifc4.MeasureResource.IfcCountMeasure => null,// todo: not sure what to do here
                Ifc4.MeasureResource.IfcDescriptiveMeasure => null,// todo: not sure what to do here
                Ifc4.MeasureResource.IfcNormalisedRatioMeasure => null,// todo: not sure what to do here
                Ifc4.MeasureResource.IfcNumericMeasure => null,// todo: not sure what to do here.
                Ifc4.MeasureResource.IfcParameterValue => null,// todo: not sure what to do here.
                Ifc4.MeasureResource.IfcPositiveRatioMeasure => null,// todo: not sure what to do here.
                Ifc4.MeasureResource.IfcRatioMeasure => null,// todo: not sure what to do here.
                Ifc4.MeasureResource.IfcComplexNumber => null,// todo: not sure what to do here.
                                                              // types from IfcSimpleValue
                Ifc4.MeasureResource.IfcSimpleValue => null,
                _ => null,
            };
            // more measures types to be taken from http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcmeasureresource/lexical/ifcderivedmeasurevalue.htm

            if (requiredUnit == null)
                return null;

            IIfcNamedUnit nu = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit) ??
                              (IIfcNamedUnit)obj.Units.OfType<IIfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        /// <summary>
        /// Gets the unit for a <see cref="IIfcPhysicalSimpleQuantity"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static IIfcNamedUnit GetUnitFor(this IIfcUnitAssignment obj, IIfcPhysicalSimpleQuantity quantity)
        {
            if (quantity.Unit != null)
                return quantity.Unit;

            //IfcUnitEnum? requiredUnit = null;

            // list of possible types taken from:
            // http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcquantityresource/lexical/ifcphysicalsimplequantity.htm
            //
            IfcUnitEnum? requiredUnit = quantity switch
            {
                Ifc2x3.QuantityResource.IfcQuantityLength => IfcUnitEnum.LENGTHUNIT,
                Ifc2x3.QuantityResource.IfcQuantityArea => IfcUnitEnum.AREAUNIT,
                Ifc2x3.QuantityResource.IfcQuantityVolume => IfcUnitEnum.VOLUMEUNIT,
                Ifc2x3.QuantityResource.IfcQuantityCount => null,
                Ifc2x3.QuantityResource.IfcQuantityWeight => IfcUnitEnum.MASSUNIT,
                Ifc2x3.QuantityResource.IfcQuantityTime => IfcUnitEnum.TIMEUNIT,

                Ifc4.QuantityResource.IfcQuantityLength => IfcUnitEnum.LENGTHUNIT,
                Ifc4.QuantityResource.IfcQuantityArea => IfcUnitEnum.AREAUNIT,
                Ifc4.QuantityResource.IfcQuantityVolume => IfcUnitEnum.VOLUMEUNIT,
                Ifc4.QuantityResource.IfcQuantityCount => null,
                Ifc4.QuantityResource.IfcQuantityWeight => IfcUnitEnum.MASSUNIT,
                Ifc4.QuantityResource.IfcQuantityTime => IfcUnitEnum.TIMEUNIT,

                Ifc4x3.QuantityResource.IfcQuantityLength => IfcUnitEnum.LENGTHUNIT,
                Ifc4x3.QuantityResource.IfcQuantityArea => IfcUnitEnum.AREAUNIT,
                Ifc4x3.QuantityResource.IfcQuantityVolume => IfcUnitEnum.VOLUMEUNIT,
                Ifc4x3.QuantityResource.IfcQuantityCount => null,
                Ifc4x3.QuantityResource.IfcQuantityNumber => null,
                Ifc4x3.QuantityResource.IfcQuantityWeight => IfcUnitEnum.MASSUNIT,
                Ifc4x3.QuantityResource.IfcQuantityTime => IfcUnitEnum.TIMEUNIT,
                _ => null,
            };

            if (requiredUnit == null)
                return null;

            IIfcNamedUnit nu = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit) ??
                              (IIfcNamedUnit)obj.Units.OfType<IIfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        /// <summary>
        /// Gets the unit for an <see cref="IfcUnitEnum"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static IIfcNamedUnit GetUnitFor(this IIfcUnitAssignment obj, IfcUnitEnum unitType)
        {
            return obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType) ??
                (IIfcNamedUnit)obj.Units.OfType<IIfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == unitType);
        }

        public static void SetOrChangeConversionUnit(this IIfcUnitAssignment obj, IfcUnitEnum unitType, ConversionBasedUnit unit)
        {

            var si = obj.Units.OfType<IIfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                obj.Units.Remove(si);
                try
                {
                    obj.Model.Delete(si);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            obj.Units.Add(GetNewConversionUnit(obj.Model, unitType, unit));
        }
        
        private static IIfcConversionBasedUnit GetNewConversionUnit(IModel model, IfcUnitEnum unitType, ConversionBasedUnit unitEnum)
        {
            var factory = new EntityCreator(model);
            var unit = factory.ConversionBasedUnit();
            unit.UnitType = unitType;

            switch (unitEnum)
            {
                case ConversionBasedUnit.Inch:
                    SetConversionUnitsParameters(model, unit, "inch", 25.4, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(factory));
                    break;
                case ConversionBasedUnit.Foot:
                    SetConversionUnitsParameters(model, unit, "foot", 304.8, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(factory));
                    break;
                case ConversionBasedUnit.Yard:
                    SetConversionUnitsParameters(model, unit, "yard", 914, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(factory));
                    break;
                case ConversionBasedUnit.Mile:
                    SetConversionUnitsParameters(model, unit, "mile", 1609, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 null, GetLengthDimension(factory));
                    break;
                case ConversionBasedUnit.Acre:
                    SetConversionUnitsParameters(model, unit, "acre", 4046.86, IfcUnitEnum.AREAUNIT,
                                                 IfcSIUnitName.SQUARE_METRE, null, GetAreaDimension(factory));
                    break;
                case ConversionBasedUnit.Litre:
                    SetConversionUnitsParameters(model, unit, "litre", 0.001, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(factory));
                    break;
                case ConversionBasedUnit.PintUk:
                    SetConversionUnitsParameters(model, unit, "pint UK", 0.000568, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(factory));
                    break;
                case ConversionBasedUnit.PintUs:
                    SetConversionUnitsParameters(model, unit, "pint US", 0.000473, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(factory));
                    break;
                case ConversionBasedUnit.GallonUk:
                    SetConversionUnitsParameters(model, unit, "gallon UK", 0.004546, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(factory));
                    break;
                case ConversionBasedUnit.GallonUs:
                    SetConversionUnitsParameters(model, unit, "gallon US", 0.003785, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(factory));
                    break;
                case ConversionBasedUnit.Ounce:
                    SetConversionUnitsParameters(model, unit, "ounce", 28.35, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 null, GetMassDimension(factory));
                    break;
                case ConversionBasedUnit.Pound:
                    SetConversionUnitsParameters(model, unit, "pound", 0.454, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 IfcSIPrefix.KILO, GetMassDimension(factory));
                    break;
                case ConversionBasedUnit.SquareFoot:
                    SetConversionUnitsParameters(model, unit, "square foot", 92903.04, IfcUnitEnum.AREAUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetAreaDimension(factory));
                    break;
                case ConversionBasedUnit.CubicFoot:
                    SetConversionUnitsParameters(model, unit, "cubic foot", 28316846.6, IfcUnitEnum.VOLUMEUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetVolumeDimension(factory));
                    break;
            }

            return unit;
        }
        private static void SetConversionUnitsParameters(IModel model, IIfcConversionBasedUnit unit, Ifc4.MeasureResource.IfcLabel name,
                                                         Ifc4.MeasureResource.IfcRatioMeasure ratio, IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                                         IfcSIPrefix? siUnitPrefix, IIfcDimensionalExponents dimensions)
        {
            var factory = new EntityCreator(model);
            unit.Name = name;
            unit.ConversionFactor = factory.MeasureWithUnit();
            unit.ConversionFactor.ValueComponent = ratio;
            unit.ConversionFactor.UnitComponent = factory.SIUnit(s =>
            {
                s.UnitType = unitType;
                s.Name = siUnitName;
                s.Prefix = siUnitPrefix;
            });
            unit.Dimensions = dimensions;
        }
        private static IIfcDimensionalExponents GetLengthDimension(EntityCreator factory)
        {
            var dimension = factory.DimensionalExponents();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 1;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IIfcDimensionalExponents GetVolumeDimension(EntityCreator factory)
        {
            var dimension = factory.DimensionalExponents();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 3;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IIfcDimensionalExponents GetAreaDimension(EntityCreator factory)
        {
            var dimension = factory.DimensionalExponents();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 2;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IIfcDimensionalExponents GetMassDimension(EntityCreator factory)
        {
            var dimension = factory.DimensionalExponents();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 0;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 1;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
    }
}
