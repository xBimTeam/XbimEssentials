using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc4.MeasureResource
{
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
    }
}
