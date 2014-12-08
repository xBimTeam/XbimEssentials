#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    UnitAssignmentExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc2x3.PropertyResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class UnitAssignmentExtensions
    {
        /// <summary>
        ///   Returns the factor to scale units by to convert them to SI millimetres, if they are SI units, returns 1 otherwise
        /// </summary>
        /// <param name = "ua"></param>
        /// <returns></returns>
        public static double LengthUnitPower(this IfcUnitAssignment ua)
        {
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (si != null && si.Prefix.HasValue)
                return si.Power();
            else
            {
                IfcConversionBasedUnit cu =
                    ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                if (cu != null)
                {
                    IfcMeasureWithUnit mu = cu.ConversionFactor;
                    IfcSIUnit uc = mu.UnitComponent as IfcSIUnit;
                    //some BIM tools such as StruCAD write the conversion value out as a Length Measure
                    if (uc != null)
                    {
                        ExpressType et = ((ExpressType)mu.ValueComponent);
                        double cFactor = 1.0;
                        if(et.UnderlyingSystemType==typeof(double))
                            cFactor = (double) et.Value;
                        else if(et.UnderlyingSystemType==typeof(int))
                            cFactor = (double) ((int)et.Value);
                        else if (et.UnderlyingSystemType == typeof(long))
                            cFactor = (double)((long)et.Value);

                        return uc.Power() * cFactor ;
                    }
                }
            }
            return 1.0;
        }

        public static double GetPower(this IfcUnitAssignment ua, IfcUnitEnum unitType)
        {
           
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null && si.Prefix.HasValue)
                return si.Power();
            else
            {
                IfcConversionBasedUnit cu =
                    ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == unitType);
                if (cu != null)
                {
                    IfcMeasureWithUnit mu = cu.ConversionFactor;
                    IfcSIUnit uc = mu.UnitComponent as IfcSIUnit;
                    //some BIM tools such as StruCAD write the conversion value out as a Length Measure
                    if (uc != null)
                    {
                        ExpressType et = ((ExpressType)mu.ValueComponent);
                        double cFactor = 1.0;
                        if (et.UnderlyingSystemType == typeof(double))
                            cFactor = (double)et.Value;
                        else if (et.UnderlyingSystemType == typeof(int))
                            cFactor = (double)((int)et.Value);
                        else if (et.UnderlyingSystemType == typeof(long))
                            cFactor = (double)((long)et.Value);

                        return uc.Power() * cFactor;
                    }
                }
            }
            return 1.0;
        }

        /// <summary>
        /// Sets the Length Unit to be SIUnit and SIPrefix, returns false if the units are not SI
        /// </summary>
        /// <param name = "ua"></param>
        /// <param name = "siUnitName"></param>
        /// <param name = "siPrefix"></param>
        /// <returns></returns>
        public static bool SetSILengthUnits(this IfcUnitAssignment ua, IfcSIUnitName siUnitName, IfcSIPrefix? siPrefix)
        {
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (si != null)
            {
                si.Prefix = siPrefix;
                si.Name = siUnitName;
                return true;
            }
            else
                return false;
        }

        public static void SetOrChangeSIUnit(this IfcUnitAssignment ua, IfcUnitEnum unitType, IfcSIUnitName siUnitName,
                                             IfcSIPrefix? siUnitPrefix)
        {
            IModel model = ua.ModelOf;
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                si.Prefix = siUnitPrefix;
                si.Name = siUnitName;
            }
            else
            {
                ua.Units.Add(model.Instances.New<IfcSIUnit>(s =>
                                                                 {
                                                                     s.UnitType = unitType;
                                                                     s.Name = siUnitName;
                                                                     s.Prefix = siUnitPrefix;
                                                                 }));
            }
        }

        public static IfcNamedUnit GetUnitFor(this IfcUnitAssignment ua, IfcPropertySingleValue Property)
        {

            if (Property.Unit != null)
                return (IfcNamedUnit)Property.Unit;


            
            // nominal value can be of types with subtypes:
            //	IfcMeasureValue, IfcSimpleValue, IfcDerivedMeasureValue

            IfcUnitEnum? requiredUnit = null;
            // types from http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcmeasureresource/lexical/ifcmeasurevalue.htm
            if (Property.NominalValue is IfcVolumeMeasure)
                requiredUnit = IfcUnitEnum.VOLUMEUNIT;
            else if (Property.NominalValue is IfcAreaMeasure)
                requiredUnit = IfcUnitEnum.AREAUNIT;
            else if (Property.NominalValue is IfcLengthMeasure)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (Property.NominalValue is IfcPositiveLengthMeasure)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (Property.NominalValue is IfcAmountOfSubstanceMeasure)
                requiredUnit = IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT;
            else if (Property.NominalValue is IfcContextDependentMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (Property.NominalValue is IfcCountMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (Property.NominalValue is IfcDescriptiveMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (Property.NominalValue is IfcElectricCurrentMeasure)
                requiredUnit = IfcUnitEnum.ELECTRICCURRENTUNIT; 
            else if (Property.NominalValue is IfcLuminousIntensityMeasure)
                requiredUnit = IfcUnitEnum.LUMINOUSINTENSITYUNIT;
            else if (Property.NominalValue is IfcMassMeasure)
                requiredUnit = IfcUnitEnum.MASSUNIT;
            else if (Property.NominalValue is IfcNormalisedRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here
            else if (Property.NominalValue is IfcNumericMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (Property.NominalValue is IfcParameterValue)
                requiredUnit = null; // todo: not sure what to do here.
            else if (Property.NominalValue is IfcPlaneAngleMeasure)
                requiredUnit = IfcUnitEnum.PLANEANGLEUNIT;
            else if (Property.NominalValue is IfcPositiveRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (Property.NominalValue is IfcPositivePlaneAngleMeasure)
                requiredUnit = IfcUnitEnum.PLANEANGLEUNIT;
            else if (Property.NominalValue is IfcRatioMeasure)
                requiredUnit = null; // todo: not sure what to do here.
            else if (Property.NominalValue is IfcSolidAngleMeasure)
                requiredUnit = IfcUnitEnum.SOLIDANGLEUNIT;
            else if (Property.NominalValue is IfcThermodynamicTemperatureMeasure)
                requiredUnit = IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT;
            else if (Property.NominalValue is IfcTimeMeasure)
                requiredUnit = IfcUnitEnum.TIMEUNIT;
            else if (Property.NominalValue is IfcComplexNumber)
                requiredUnit = null; // todo: not sure what to do here.

            // types from IfcSimpleValue
            else if (Property.NominalValue is IfcSimpleValue)
                requiredUnit = null;

            // more measures types to be taken from http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcmeasureresource/lexical/ifcderivedmeasurevalue.htm
            
            if (requiredUnit == null)
                return null;

            IfcNamedUnit nu = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            if (nu == null)
                nu = ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        public static IfcNamedUnit GetUnitFor(this IfcUnitAssignment ua, IfcPhysicalSimpleQuantity Quantity)
        {
            if (Quantity.Unit != null)
                return Quantity.Unit;

            IfcUnitEnum? requiredUnit = null; 

            // list of possible types taken from:
            // http://www.buildingsmart-tech.org/ifc/IFC2x3/TC1/html/ifcquantityresource/lexical/ifcphysicalsimplequantity.htm
            //
            if (Quantity is IfcQuantityLength)
                requiredUnit = IfcUnitEnum.LENGTHUNIT;
            else if (Quantity is IfcQuantityArea)
                requiredUnit = IfcUnitEnum.AREAUNIT;
            else if (Quantity is IfcQuantityVolume)
                requiredUnit = IfcUnitEnum.VOLUMEUNIT;
            else if (Quantity is IfcQuantityCount) // really not sure what to do here.
                return null;
            else if (Quantity is IfcQuantityWeight)
                requiredUnit = IfcUnitEnum.MASSUNIT;
            else if (Quantity is IfcQuantityTime)
                requiredUnit = IfcUnitEnum.TIMEUNIT;

            if (requiredUnit == null)
                return null;

            IfcNamedUnit nu = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            if (nu == null)
                nu = ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == (IfcUnitEnum)requiredUnit);
            return nu;
        }

        public static IfcNamedUnit GetAreaUnit(this IfcUnitAssignment ua)
        {
            IfcNamedUnit nu = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.AREAUNIT);
            if (nu == null)
                nu = ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.AREAUNIT);
            return nu;
        }
        public static IfcNamedUnit GetLengthUnit(this IfcUnitAssignment ua)
        {
            IfcNamedUnit nu = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (nu == null)
                nu = ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            return nu;
        }
        public static IfcNamedUnit GetVolumeUnit(this IfcUnitAssignment ua)
        {
            IfcNamedUnit nu = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.VOLUMEUNIT);
            if (nu == null)
                nu = ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.VOLUMEUNIT);
            return nu;
        }
        public static string GetLengthUnitName(this IfcUnitAssignment ua)
        {
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
            if (si != null)
            {
                if (si.Prefix.HasValue)
                    return string.Format("{0}{1}", si.Prefix.Value.ToString(), si.Name.ToString());
                else
                    return si.Name.ToString();
            }
            else
            {
                IfcConversionBasedUnit cu =
                    ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                if (cu != null)
                {
                    return cu.Name;
                }
                else
                {
                    IfcConversionBasedUnit cbu =
                        ua.Units.OfType<IfcConversionBasedUnit>().FirstOrDefault(
                            u => u.UnitType == IfcUnitEnum.LENGTHUNIT);
                    if (cbu != null)
                    {
                        return cbu.Name;
                    }
                }
            }
            return "";
        }
        public static void SetOrChangeConversionUnit(this IfcUnitAssignment ua, IfcUnitEnum unitType, ConversionBasedUnit unit)
        {
            IModel model = ua.ModelOf;
            IfcSIUnit si = ua.Units.OfType<IfcSIUnit>().FirstOrDefault(u => u.UnitType == unitType);
            if (si != null)
            {
                ua.Units.Remove(si);
                try
                {
                    model.Delete(si);
                } catch (Exception){}
            }
            ua.Units.Add(GetNewConversionUnit(model, unitType, unit));
        }
        private static IfcConversionBasedUnit GetNewConversionUnit(IModel model, IfcUnitEnum unitType, ConversionBasedUnit unitEnum)
        {
            IfcConversionBasedUnit unit = model.Instances.New<IfcConversionBasedUnit>();
            unit.UnitType = unitType;

            switch (unitEnum)
            {
                case ConversionBasedUnit.INCH:
                    SetConversionUnitsParameters(model, unit, "inch", 25.4, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.FOOT:
                    SetConversionUnitsParameters(model, unit, "foot", 304.8, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.YARD:
                    SetConversionUnitsParameters(model, unit, "yard", 914, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.MILE:
                    SetConversionUnitsParameters(model, unit, "mile", 1609, IfcUnitEnum.LENGTHUNIT, IfcSIUnitName.METRE,
                                                 null, GetLengthDimension(model));
                    break;
                case ConversionBasedUnit.ACRE:
                    SetConversionUnitsParameters(model, unit, "acre", 4046.86, IfcUnitEnum.AREAUNIT,
                                                 IfcSIUnitName.SQUARE_METRE, null, GetAreaDimension(model));
                    break;
                case ConversionBasedUnit.LITRE:
                    SetConversionUnitsParameters(model, unit, "litre", 0.001, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.PINT_UK:
                    SetConversionUnitsParameters(model, unit, "pint UK", 0.000568, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.PINT_US:
                    SetConversionUnitsParameters(model, unit, "pint US", 0.000473, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.GALLON_UK:
                    SetConversionUnitsParameters(model, unit, "gallon UK", 0.004546, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.GALLON_US:
                    SetConversionUnitsParameters(model, unit, "gallon US", 0.003785, IfcUnitEnum.VOLUMEUNIT,
                                                 IfcSIUnitName.CUBIC_METRE, null, GetVolumeDimension(model));
                    break;
                case ConversionBasedUnit.OUNCE:
                    SetConversionUnitsParameters(model, unit, "ounce", 28.35, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 null, GetMassDimension(model));
                    break;
                case ConversionBasedUnit.POUND:
                    SetConversionUnitsParameters(model, unit, "pound", 0.454, IfcUnitEnum.MASSUNIT, IfcSIUnitName.GRAM,
                                                 IfcSIPrefix.KILO, GetMassDimension(model));
                    break;
                case ConversionBasedUnit.SQUARE_FOOT:
                    SetConversionUnitsParameters(model, unit, "square foot", 92903.04 , IfcUnitEnum.AREAUNIT, IfcSIUnitName.METRE,
                                                 IfcSIPrefix.MILLI, GetAreaDimension(model));
                    break;
                case ConversionBasedUnit.CUBIC_FOOT:
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
            IfcDimensionalExponents dimension = model.Instances.New<IfcDimensionalExponents>();
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
            IfcDimensionalExponents dimension = model.Instances.New<IfcDimensionalExponents>();
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
            IfcDimensionalExponents dimension = model.Instances.New<IfcDimensionalExponents>();
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
            IfcDimensionalExponents dimension = model.Instances.New<IfcDimensionalExponents>();
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

    public enum ConversionBasedUnit
    {
        INCH,
        FOOT,
        YARD,
        MILE,
        ACRE,
        LITRE,
        PINT_UK,
        PINT_US,
        GALLON_UK,
        GALLON_US,
        OUNCE,
        POUND,
        SQUARE_FOOT,
        CUBIC_FOOT
    }
}
