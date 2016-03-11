﻿using System;
using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.Interfaces
{
    // ReSharper disable once InconsistentNaming
    public partial interface IIfcSIUnit
    {
        double Power { get; }
    }
}
namespace Xbim.Ifc4.MeasureResource
{
    // ReSharper disable once InconsistentNaming
    public partial class IfcSIUnit
    {
        private static readonly Dictionary<IfcSIUnitName, IfcDimensionalExponents> ExponentsCache = new Dictionary<IfcSIUnitName, IfcDimensionalExponents>(); 
        internal IfcDimensionalExponents IfcDimensionsForSiUnit(IfcSIUnitName name)
        {
            IfcDimensionalExponents result;
            if (ExponentsCache.TryGetValue(name, out result))
                return result;
            switch (name)
            {
                case IfcSIUnitName.METRE: result = GetOrCreateExponents(new List<int> { 1, 0, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.SQUARE_METRE: result = GetOrCreateExponents(new List<int> { 2, 0, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.CUBIC_METRE: result = GetOrCreateExponents(new List<int> { 3, 0, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.GRAM: result = GetOrCreateExponents(new List<int> { 0, 1, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.SECOND: result = GetOrCreateExponents(new List<int> { 0, 0, 1, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.AMPERE: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 1, 0, 0, 0 }); break;
                case IfcSIUnitName.KELVIN: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 1, 0, 0 }); break;
                case IfcSIUnitName.MOLE: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 1, 0 }); break;
                case IfcSIUnitName.CANDELA: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 0, 1 }); break;
                case IfcSIUnitName.RADIAN: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.STERADIAN: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.HERTZ: result = GetOrCreateExponents(new List<int> { 0, 0, -1, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.NEWTON: result = GetOrCreateExponents(new List<int> { 1, 1, -2, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.PASCAL: result = GetOrCreateExponents(new List<int> { -1, 1, -2, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.JOULE: result = GetOrCreateExponents(new List<int> { 2, 1, -2, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.WATT: result = GetOrCreateExponents(new List<int> { 2, 1, -3, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.COULOMB: result = GetOrCreateExponents(new List<int> { 0, 0, 1, 1, 0, 0, 0 }); break;
                case IfcSIUnitName.VOLT: result = GetOrCreateExponents(new List<int> { 2, 1, -3, -1, 0, 0, 0 }); break;
                case IfcSIUnitName.FARAD: result = GetOrCreateExponents(new List<int> { -2, -1, 4, 1, 0, 0, 0 }); break;
                case IfcSIUnitName.OHM: result = GetOrCreateExponents(new List<int> { 2, 1, -3, -2, 0, 0, 0 }); break;
                case IfcSIUnitName.SIEMENS: result = GetOrCreateExponents(new List<int> { -2, -1, 3, 2, 0, 0, 0 }); break;
                case IfcSIUnitName.WEBER: result = GetOrCreateExponents(new List<int> { 2, 1, -2, -1, 0, 0, 0 }); break;
                case IfcSIUnitName.TESLA: result = GetOrCreateExponents(new List<int> { 0, 1, -2, -1, 0, 0, 0 }); break;
                case IfcSIUnitName.HENRY: result = GetOrCreateExponents(new List<int> { 2, 1, -2, -2, 0, 0, 0 }); break;
                case IfcSIUnitName.DEGREE_CELSIUS: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 1, 0, 0 }); break;
                case IfcSIUnitName.LUMEN: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 0, 1 }); break;
                case IfcSIUnitName.LUX: result = GetOrCreateExponents(new List<int> { -2, 0, 0, 0, 0, 0, 1 }); break;
                case IfcSIUnitName.BECQUEREL: result = GetOrCreateExponents(new List<int> { 0, 0, -1, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.GRAY: result = GetOrCreateExponents(new List<int> { 2, 0, -2, 0, 0, 0, 0 }); break;
                case IfcSIUnitName.SIEVERT: result = GetOrCreateExponents(new List<int> { 2, 0, -2, 0, 0, 0, 0 }); break;
                default: result = GetOrCreateExponents(new List<int> { 0, 0, 0, 0, 0, 0, 0 }); break;
            }

            ExponentsCache.Add(name, result);
            return result;
        }

        private IfcDimensionalExponents GetOrCreateExponents(IList<int> exponents)
        {
            var existing = Model.Instances.FirstOrDefault<IfcDimensionalExponents>(e =>
                e.LengthExponent == exponents[0] &&
                e.MassExponent == exponents[1] &&
                e.TimeExponent == exponents[2] &&
                e.ElectricCurrentExponent == exponents[3] &&
                e.ThermodynamicTemperatureExponent == exponents[4] &&
                e.AmountOfSubstanceExponent == exponents[5] &&
                e.LuminousIntensityExponent == exponents[6]
                );
            if (existing != null)
                return existing;

            return Model.Instances.New<IfcDimensionalExponents>(e =>
            {
                e.LengthExponent = exponents[0];
                e.MassExponent = exponents[1];
                e.TimeExponent = exponents[2];
                e.ElectricCurrentExponent = exponents[3];
                e.ThermodynamicTemperatureExponent = exponents[4];
                e.AmountOfSubstanceExponent = exponents[5];
                e.LuminousIntensityExponent = exponents[6];
            });

        }
        /// <summary>
        ///   returns the power of the SIUnit prefix, i.e. MILLI = 0.001, if undefined returns 1.0
        /// </summary>
        public double Power
        {
            get
            {
                var exponential = 1;
                if (UnitType == IfcUnitEnum.AREAUNIT) exponential = 2;
                if (UnitType == IfcUnitEnum.VOLUMEUNIT) exponential = 3;
                if (Prefix.HasValue)
                {
                    double factor;
                    switch (Prefix.Value)
                    {
                        case IfcSIPrefix.EXA:
                            factor = 1.0e+18; break;
                        case IfcSIPrefix.PETA:
                            factor = 1.0e+15; break;
                        case IfcSIPrefix.TERA:
                            factor = 1.0e+12; break;
                        case IfcSIPrefix.GIGA:
                            factor = 1.0e+9; break;
                        case IfcSIPrefix.MEGA:
                            factor = 1.0e+6; break;
                        case IfcSIPrefix.KILO:
                            factor = 1.0e+3; break;
                        case IfcSIPrefix.HECTO:
                            factor = 1.0e+2; break;
                        case IfcSIPrefix.DECA:
                            factor = 10; break;
                        case IfcSIPrefix.DECI:
                            factor = 1.0e-1; break;
                        case IfcSIPrefix.CENTI:
                            factor = 1.0e-2; break;
                        case IfcSIPrefix.MILLI:
                            factor = 1.0e-3; break;
                        case IfcSIPrefix.MICRO:
                            factor = 1.0e-6; break;
                        case IfcSIPrefix.NANO:
                            factor = 1.0e-9; break;
                        case IfcSIPrefix.PICO:
                            factor = 1.0e-12; break;
                        case IfcSIPrefix.FEMTO:
                            factor = 1.0e-15; break;
                        case IfcSIPrefix.ATTO:
                            factor = 1.0e-18; break;
                        default:
                            factor = 1.0; break;
                    }
                    return Math.Pow(factor, exponential);
                }
                return 1.0;
            }
        }



        /// <summary>
        /// Get Symbol string for SIUnit unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public new string Symbol
        {
            get
            {
                var ifcSiUnitName = Name;
                string value;
                string prefix = string.Empty;
                if (Prefix != null)
                {
                    var ifcSiPrefix = (IfcSIPrefix) Prefix;
                    switch (ifcSiPrefix)
                    {
                        case IfcSIPrefix.CENTI:
                            prefix = "c";
                            break;
                        case IfcSIPrefix.MILLI:
                            prefix = "m";
                            break;
                        case IfcSIPrefix.KILO:
                            prefix = "k";
                            break;
                        default: //TODO: the other values of IfcSIPrefix
                            prefix = ifcSiPrefix.ToString();
                            break;
                    }
                }

                switch (ifcSiUnitName)
                {
                    case IfcSIUnitName.METRE:
                        value = prefix + "m";
                        break;
                    case IfcSIUnitName.SQUARE_METRE:
                        value = prefix + "m" + '\u00B2'; //((char)0x00B2)might need to look at this for other cultures
                        break;
                    case IfcSIUnitName.CUBIC_METRE:
                        value = prefix + "m" + '\u00B3'; //((char)0x00B3)
                        break;
                    case IfcSIUnitName.GRAM:
                        value = prefix + "g";
                        break;
                    default: //TODO: the other values of IfcSIUnitName
                        value = ToString();
                        break;
                }
                return value;
            }
        }

        /// <summary>
        /// Returns the full name of the unit
        /// </summary>
        /// <returns>string holding name</returns>
        public new string FullName
        {
            get
            {
                var prefixUnit = (Prefix.HasValue) ? Prefix.ToString() : ""; //see IfcSIPrefix
                var value = Name.ToString(); //see IfcSIUnitName
                //Handle the "_" in _name value, should work for lengths, but might have to look at other values later
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("_"))
                        return value.Replace("_", prefixUnit);
                    return prefixUnit + value; //combine to give length name
                }
                return string.Format("{0}{1}", Prefix.HasValue ? Prefix.Value.ToString() : "", Name);
            }
        }
    }
}
