using System;
using System.Linq;
using Xbim.Common;

using Xbim.Ifc4x3.PropertyResource;
using Xbim.Ifc4x3.QuantityResource;

namespace Xbim.Ifc4x3.MeasureResource
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
    public partial class IfcUnitAssignment
    {
        /// <summary>
        ///   Returns the factor to scale units by to convert them to SI millimetres, if they are SI units, returns 1 otherwise
        /// </summary>
        /// <returns></returns>
        public double LengthUnitPower
        {
             get
             {
                 var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                 if (si != null && si.Prefix.HasValue)
                     return si.Power;
                 var cu =
                     Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                 if (cu == null) return 1.0;
                 var mu = cu.ConversionFactor;
                 var uc = mu.UnitComponent as IfcSIUnit;
                 //some BIM tools such as StruCAD write the conversion value out as a Length Measure
                 if (uc == null) return 1.0;


                 var et = ((IExpressValueType) mu.ValueComponent);
                 var cFactor = 1.0;
                 if (et.UnderlyingSystemType == typeof (double))
                     cFactor = (double) et.Value;
                 else if (et.UnderlyingSystemType == typeof (int))
                     cFactor = (int) et.Value;
                 else if (et.UnderlyingSystemType == typeof (long))
                     cFactor = (long) et.Value;

                 return uc.Power*cFactor;
             }
        }

        public double Power(IfcUnitEnum unitType)
        {          
            var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null && si.Prefix.HasValue)
                return si.Power;
            var cu =
                Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (cu == null) return 1.0;
            var mu = cu.ConversionFactor;
            var uc = mu.UnitComponent as IfcSIUnit;
            //some BIM tools such as StruCAD write the conversion value out as a Length Measure
            if (uc == null) return 1.0;


            var et = ((IExpressValueType)mu.ValueComponent);
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
        /// <param name = "siUnitName"></param>
        /// <param name = "siPrefix"></param>
        /// <returns></returns>
        public bool SetSiLengthUnits(IfcSIUnitName siUnitName, IfcSIPrefix? siPrefix)
        {
            var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (si != null)
            {
                si.Prefix = siPrefix;
                si.Name = siUnitName;
                return true;
            }
            return false;
        }

        public  void SetOrChangeSiUnit(IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                             IfcSIPrefix? siUnitPrefix)
        {
            var model = Model;
            var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                si.Prefix = siUnitPrefix;
                si.Name = siUnitName;
            }
            else
            {
                Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = unitType;
                                                                     s.Name = siUnitName;
                                                                     s.Prefix = siUnitPrefix;
                                                                 }));
            }
        }

        public  IfcNamedUnit GetUnitFor(IfcPropertySingleValue property)
        {

            if (property.Unit != null)
                return (IfcNamedUnit)property.Unit;

            // nominal value can be of types with subtypes:
            //	IfcMeasureValue, IfcSimpleValue, IfcDerivedMeasureValue

            IfcUnitEnum? requiredUnit;
            // types from https://ifc43-docs.standards.buildingsmart.org/IFC/RELEASE/IFC4x3/HTML/lexical/IfcMeasureValue.htm
            if (property.NominalValue is IfcVolumeMeasure)
                requiredUnit = IfcUnitEnum.VOLUMEUNIT;
            else if (property.NominalValue is IfcAreaMeasure)
                requiredUnit = IfcUnitEnum.AREAUNIT;
            else if (property.NominalValue is IfcLengthMeasure)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (property.NominalValue is IfcPositiveLengthMeasure)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (property.NominalValue is IfcNonNegativeLengthMeasure)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (property.NominalValue is IfcAmountOfSubstanceMeasure)
                requiredUnit = IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT;
            else if (property.NominalValue is IfcContextDependentMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (property.NominalValue is IfcCountMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (property.NominalValue is IfcDescriptiveMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (property.NominalValue is IfcElectricCurrentMeasure)
                requiredUnit = IfcUnitEnum.ELECTRICCURRENTUNIT; 
            else if (property.NominalValue is IfcLuminousIntensityMeasure)
                requiredUnit = IfcUnitEnum.LUMINOUSINTENSITYUNIT;
            else if (property.NominalValue is IfcMassMeasure)
                requiredUnit = IfcUnitEnum.MASSUNIT;
            else if (property.NominalValue is IfcNormalisedRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (property.NominalValue is IfcNumericMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (property.NominalValue is IfcParameterValue)
                requiredUnit = null; // todo: not sure what to do here.
            else if (property.NominalValue is IfcPlaneAngleMeasure)
                requiredUnit = IfcUnitEnum.PLANEANGLEUNIT;
            else if (property.NominalValue is IfcPositiveRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (property.NominalValue is IfcPositivePlaneAngleMeasure)
                requiredUnit = IfcUnitEnum.PLANEANGLEUNIT;
            else if (property.NominalValue is IfcRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (property.NominalValue is IfcSolidAngleMeasure)
                requiredUnit = IfcUnitEnum.SOLIDANGLEUNIT;
            else if (property.NominalValue is IfcThermodynamicTemperatureMeasure)
                requiredUnit = IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT;
            else if (property.NominalValue is IfcTimeMeasure)
                requiredUnit = IfcUnitEnum.TIMEUNIT;
            else if (property.NominalValue is IfcComplexNumber)
                requiredUnit = null; // todo: not sure what to do here.
            // types from IfcSimpleValue
            else if (property.NominalValue is IfcSimpleValue)
                requiredUnit = null;
            else
                requiredUnit = null;
            // more measures types to be taken from http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcmeasureresource/lexical/ifcderivedmeasurevalue.htm
            
            if (requiredUnit == null)
                return null;

            IfcNamedUnit nu = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit) ??
                              (IfcNamedUnit) Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        public IfcNamedUnit GetUnitFor(IfcPhysicalSimpleQuantity quantity)
        {
            if (quantity.Unit != null)
                return quantity.Unit;

            IfcUnitEnum? requiredUnit = null;

            // list of possible types taken from:
            // https://ifc43-docs.standards.buildingsmart.org/IFC/RELEASE/IFC4x3/HTML/lexical/IfcPhysicalSimpleQuantity.htm
            //
            if (quantity is IfcQuantityLength)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (quantity is IfcQuantityArea)
                requiredUnit = IfcUnitEnum.AREAUNIT;
            else if (quantity is IfcQuantityVolume)
                requiredUnit = IfcUnitEnum.VOLUMEUNIT;
            else if (quantity is IfcQuantityCount) // really not sure what to do here.
                return null;
            else if (quantity is IfcQuantityWeight)
                requiredUnit = IfcUnitEnum.MASSUNIT;
            else if (quantity is IfcQuantityTime)
                requiredUnit = IfcUnitEnum.TIMEUNIT;
            else if (quantity is IfcQuantityNumber)
                requiredUnit = null;

            if (requiredUnit == null)
                return null;

            IfcNamedUnit nu = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit) ??
                              (IfcNamedUnit) Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        public IfcNamedUnit AreaUnit
        {
            get
            {
                IfcNamedUnit nu = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.AREAUNIT) ??
                                  (IfcNamedUnit) Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.AREAUNIT);
                return nu;
            }
        }
        public IfcNamedUnit LengthUnit
        {
            get
            {
                IfcNamedUnit nu = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT) ??
                                  (IfcNamedUnit) Units.OfType<IfcConversionBasedUnit>()
                    .FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                return nu;
            }
        }
        public IfcNamedUnit VolumeUnit
        {
            get
            {
                IfcNamedUnit nu = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.VOLUMEUNIT) ??
                                  (IfcNamedUnit) Units.OfType<IfcConversionBasedUnit>()
                    .FirstOrDefault(u => u.UnitType == IfcUnitEnum.VOLUMEUNIT);
                return nu;
            }
        }
        public string LengthUnitName
        {
            get
            {
                var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                if (si != null)
                {
                    if (si.Prefix.HasValue)
                        return string.Format("{0}{1}", si.Prefix.Value.ToString(), si.Name.ToString());
                    else
                        return si.Name.ToString();
                }
                else
                {
                    var cu =
                        Units.OfType<IfcConversionBasedUnit>()
                            .FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                    if (cu != null)
                    {
                        return cu.Name;
                    }
                    else
                    {
                        var cbu =
                            Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(
                                u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                        if (cbu != null)
                        {
                            return cbu.Name;
                        }
                    }
                }
                return "";
            }
        }
        public  void SetOrChangeConversionUnit(IfcUnitEnum unitType, ConversionBasedUnit unit)
        {
            
            var si = Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                Units.Remove(si);
                try
                {
                    Model.Delete(si);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            Units.Add(GetNewConversionUnit(Model, unitType, unit));
        }
        private static IfcConversionBasedUnit GetNewConversionUnit(IModel model, IfcUnitEnum unitType, ConversionBasedUnit unitEnum)
        {
            var unit = model.Instances.New<IfcConversionBasedUnit>();
            unit.UnitType = unitType;

            switch (unitEnum)
            {
                case ConversionBasedUnit.Inch:
                    SetConversionUnitsParameters(model, unit, "inch", 25.4, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.Foot:
                    SetConversionUnitsParameters(model, unit, "foot", 304.8, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.Yard:
                    SetConversionUnitsParameters(model, unit, "yard", 914, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.Mile:
                    SetConversionUnitsParameters(model, unit, "mile", 1609, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 null, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.Acre:
                    SetConversionUnitsParameters(model, unit, "acre", 4046.86, IfcUnitEnum.AREAUNIT,
                                                 IfcSIUnitName.SQUARE_METRE, null, GetAreaDimension(model));
                    break;
                case ConversionBasedUnit.Litre:
                    SetConversionUnitsParameters(model, unit, "litre", 0.001, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.PintUk:
                    SetConversionUnitsParameters(model, unit, "pint UK", 0.000568, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.PintUs:
                    SetConversionUnitsParameters(model, unit, "pint US", 0.000473, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.GallonUk:
                    SetConversionUnitsParameters(model, unit, "gallon UK", 0.004546, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.GallonUs:
                    SetConversionUnitsParameters(model, unit, "gallon US", 0.003785, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.Ounce:
                    SetConversionUnitsParameters(model, unit, "ounce", 28.35, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 null, GetMassDimension(model));
                    break;
                case ConversionBasedUnit.Pound:
                    SetConversionUnitsParameters(model, unit, "pound", 0.454, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 IfcSIPrefix.KILO, GetMassDimension(model));
                    break;
                case ConversionBasedUnit.SquareFoot:
                    SetConversionUnitsParameters(model, unit, "square foot", 92903.04 , IfcUnitEnum.AREAUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetAreaDimension(model));
                    break;
                case ConversionBasedUnit.CubicFoot:
                    SetConversionUnitsParameters(model, unit, "cubic foot", 28316846.6, IfcUnitEnum.VOLUMEUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI,  GetVolumeDimension(model));
                    break;
            }

            return unit;
        }
        private static void SetConversionUnitsParameters(IModel model, IfcConversionBasedUnit unit, IfcLabel name,
                                                         IfcRatioMeasure ratio, IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                                         IfcSIPrefix? siUnitPrefix, IfcDimensionalExponents dimensions)
        {
            unit.Name = name;
            unit.ConversionFactor = model.Instances.New<IfcMeasureWithUnit>();
            unit.ConversionFactor.ValueComponent = ratio;
            unit.ConversionFactor.UnitComponent = model.Instances.New<IfcSIUnit>(s =>
                                                                           {
                                                                               s.UnitType = unitType;
                                                                               s.Name = siUnitName;
                                                                               s.Prefix = siUnitPrefix;
                                                                           });
            unit.Dimensions = dimensions;
        }
        private static IfcDimensionalExponents GetLengthDimension(IModel model)
        {
            var dimension = model.Instances.New<IfcDimensionalExponents>();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 1;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IfcDimensionalExponents GetVolumeDimension(IModel model)
        {
            var dimension = model.Instances.New<IfcDimensionalExponents>();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 3;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IfcDimensionalExponents GetAreaDimension(IModel model)
        {
            var dimension = model.Instances.New<IfcDimensionalExponents>();
            dimension.AmountOfSubstanceExponent = 0;
            dimension.ElectricCurrentExponent = 0;
            dimension.LengthExponent = 2;
            dimension.LuminousIntensityExponent = 0;
            dimension.MassExponent = 0;
            dimension.ThermodynamicTemperatureExponent = 0;
            dimension.TimeExponent = 0;

            return dimension;
        }
        private static IfcDimensionalExponents GetMassDimension(IModel model)
        {
            var dimension = model.Instances.New<IfcDimensionalExponents>();
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
