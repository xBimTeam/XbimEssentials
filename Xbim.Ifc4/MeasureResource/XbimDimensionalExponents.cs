using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.MeasureResource
{
    public class XbimDimensionalExponents 
    {
        private readonly long[] _exponents;

        /// <summary>
        ///   The power of the length base quantity.
        /// </summary>
        public long LengthExponent { get; set; }

        /// <summary>
        ///   The power of the mass base quantity.
        /// </summary>
        public long MassExponent { get; set; }

        /// <summary>
        ///   The power of the time base quantity.
        /// </summary>
        public long TimeExponent { get; set; }

        /// <summary>
        ///   The power of the electric current base quantity.
        /// </summary>
        public long ElectricCurrentExponent { get; set; }

        /// <summary>
        ///   The power of the thermodynamic temperature base quantity.
        /// </summary>
        public long ThermodynamicTemperatureExponent { get; set; }

        /// <summary>
        ///   The power of the amount of substance base quantity.
        /// </summary>
        public long AmountOfSubstanceExponent { get; set; }

        /// <summary>
        ///   The power of the luminous longensity base quantity.
        /// </summary>
        public long LuminousIntensityExponent { get; set; }

        public XbimDimensionalExponents()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        public XbimDimensionalExponents(int length, int mass, int time, int elec, int temp, int substs, int lumin)
        {
            _exponents = new long[7];
            _exponents[0] = length;
            _exponents[1] = mass;
            _exponents[2] = time;
            _exponents[3] = elec;
            _exponents[4] = temp;
            _exponents[5] = substs;
            _exponents[6] = lumin;
        }

        public long this[int index]
        {
            get { return _exponents == null ? 0 : _exponents[index]; }
        }

       

        public static XbimDimensionalExponents DimensionsForSiUnit(IfcSIUnitName? siUnit)
        {
            if (siUnit == null) return null;
            switch (siUnit)
            {
                case IfcSIUnitName.METRE:
                    return new XbimDimensionalExponents(1, 0, 0, 0, 0, 0, 0);
                case IfcSIUnitName.SQUARE_METRE:
                    return new XbimDimensionalExponents(2, 0, 0, 0, 0, 0, 0);
                case IfcSIUnitName.CUBIC_METRE:
                    return new XbimDimensionalExponents(3, 0, 0, 0, 0, 0, 0);
                case IfcSIUnitName.GRAM:
                    return new XbimDimensionalExponents(0, 1, 0, 0, 0, 0, 0);
                case IfcSIUnitName.SECOND:
                    return new XbimDimensionalExponents(0, 0, 1, 0, 0, 0, 0);
                case IfcSIUnitName.AMPERE:
                    return new XbimDimensionalExponents(0, 0, 0, 1, 0, 0, 0);
                case IfcSIUnitName.KELVIN:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 1, 0, 0);
                case IfcSIUnitName.MOLE:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 1, 0);
                case IfcSIUnitName.CANDELA:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1);
                case IfcSIUnitName.RADIAN:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
                case IfcSIUnitName.STERADIAN:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
                case IfcSIUnitName.HERTZ:
                    return new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0);
                case IfcSIUnitName.NEWTON:
                    return new XbimDimensionalExponents(1, 1, -2, 0, 0, 0, 0);
                case IfcSIUnitName.PASCAL:
                    return new XbimDimensionalExponents(-1, 1, -2, 0, 0, 0, 0);
                case IfcSIUnitName.JOULE:
                    return new XbimDimensionalExponents(2, 1, -2, 0, 0, 0, 0);
                case IfcSIUnitName.WATT:
                    return new XbimDimensionalExponents(2, 1, -3, 0, 0, 0, 0);
                case IfcSIUnitName.COULOMB:
                    return new XbimDimensionalExponents(0, 0, 1, 1, 0, 0, 0);
                case IfcSIUnitName.VOLT:
                    return new XbimDimensionalExponents(2, 1, -3, -1, 0, 0, 0);
                case IfcSIUnitName.FARAD:
                    return new XbimDimensionalExponents(-2, -1, 4, 1, 0, 0, 0);
                case IfcSIUnitName.OHM:
                    return new XbimDimensionalExponents(2, 1, -3, -2, 0, 0, 0);
                case IfcSIUnitName.SIEMENS:
                    return new XbimDimensionalExponents(-2, -1, 3, 2, 0, 0, 0);
                case IfcSIUnitName.WEBER:
                    return new XbimDimensionalExponents(2, 1, -2, -1, 0, 0, 0);
                case IfcSIUnitName.TESLA:
                    return new XbimDimensionalExponents(0, 1, -2, -1, 0, 0, 0);
                case IfcSIUnitName.HENRY:
                    return new XbimDimensionalExponents(2, 1, -2, -2, 0, 0, 0);
                case IfcSIUnitName.DEGREE_CELSIUS:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 1, 0, 0);
                case IfcSIUnitName.LUMEN:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1);
                case IfcSIUnitName.LUX:
                    return new XbimDimensionalExponents(-2, 0, 0, 0, 0, 0, 1);
                case IfcSIUnitName.BECQUEREL:
                    return new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0);
                case IfcSIUnitName.GRAY:
                    return new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0);
                case IfcSIUnitName.SIEVERT:
                    return new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0);
                default:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
            }
        }

        public static bool CorrectDimensions(IfcUnitEnum unit, XbimDimensionalExponents dim)
        {
            switch (unit)
            {
                case IfcUnitEnum.LENGTHUNIT:
                    return (dim == new XbimDimensionalExponents(1, 0, 0, 0, 0, 0, 0));
                case IfcUnitEnum.MASSUNIT:
                    return (dim == new XbimDimensionalExponents(0, 1, 0, 0, 0, 0, 0));
                case IfcUnitEnum.TIMEUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 1, 0, 0, 0, 0));
                case IfcUnitEnum.ELECTRICCURRENTUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 1, 0, 0, 0));
                case IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 1, 0, 0));
                case IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 0, 1, 0));
                case IfcUnitEnum.LUMINOUSINTENSITYUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1));
                case IfcUnitEnum.PLANEANGLEUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0));
                case IfcUnitEnum.SOLIDANGLEUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0));
                case IfcUnitEnum.AREAUNIT:
                    return (dim == new XbimDimensionalExponents(2, 0, 0, 0, 0, 0, 0));
                case IfcUnitEnum.VOLUMEUNIT:
                    return (dim == new XbimDimensionalExponents(3, 0, 0, 0, 0, 0, 0));
                case IfcUnitEnum.ABSORBEDDOSEUNIT:
                    return (dim == new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0));
                case IfcUnitEnum.RADIOACTIVITYUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0));
                case IfcUnitEnum.ELECTRICCAPACITANCEUNIT:
                    return (dim == new XbimDimensionalExponents(-2, 1, 4, 1, 0, 0, 0));
                case IfcUnitEnum.DOSEEQUIVALENTUNIT:
                    return (dim == new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0));
                case IfcUnitEnum.ELECTRICCHARGEUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 1, 1, 0, 0, 0));
                case IfcUnitEnum.ELECTRICCONDUCTANCEUNIT:
                    return (dim == new XbimDimensionalExponents(-2, -1, 3, 2, 0, 0, 0));
                case IfcUnitEnum.ELECTRICVOLTAGEUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -3, -1, 0, 0, 0));
                case IfcUnitEnum.ELECTRICRESISTANCEUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -3, -2, 0, 0, 0));
                case IfcUnitEnum.ENERGYUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -2, 0, 0, 0, 0));
                case IfcUnitEnum.FORCEUNIT:
                    return (dim == new XbimDimensionalExponents(1, 1, -2, 0, 0, 0, 0));
                case IfcUnitEnum.FREQUENCYUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0));
                case IfcUnitEnum.INDUCTANCEUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -2, -2, 0, 0, 0));
                case IfcUnitEnum.ILLUMINANCEUNIT:
                    return (dim == new XbimDimensionalExponents(-2, 0, 0, 0, 0, 0, 1));
                case IfcUnitEnum.LUMINOUSFLUXUNIT:
                    return (dim == new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1));
                case IfcUnitEnum.MAGNETICFLUXUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -2, -1, 0, 0, 0));
                case IfcUnitEnum.MAGNETICFLUXDENSITYUNIT:
                    return (dim == new XbimDimensionalExponents(0, 1, -2, -1, 0, 0, 0));
                case IfcUnitEnum.POWERUNIT:
                    return (dim == new XbimDimensionalExponents(2, 1, -3, 0, 0, 0, 0));
                case IfcUnitEnum.PRESSUREUNIT:
                    return (dim == new XbimDimensionalExponents(-1, 1, -2, 0, 0, 0, 0));
                default:
                    return false;
            }
        }

        public static XbimDimensionalExponents DimensionsForUnit(IfcUnitEnum unit)
        {
            switch (unit)
            {
                case IfcUnitEnum.LENGTHUNIT:
                    return new XbimDimensionalExponents(1, 0, 0, 0, 0, 0, 0);
                case IfcUnitEnum.MASSUNIT:
                    return new XbimDimensionalExponents(0, 1, 0, 0, 0, 0, 0);
                case IfcUnitEnum.TIMEUNIT:
                    return new XbimDimensionalExponents(0, 0, 1, 0, 0, 0, 0);
                case IfcUnitEnum.ELECTRICCURRENTUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 1, 0, 0, 0);
                case IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 1, 0, 0);
                case IfcUnitEnum.AMOUNTOFSUBSTANCEUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 1, 0);
                case IfcUnitEnum.LUMINOUSINTENSITYUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1);
                case IfcUnitEnum.PLANEANGLEUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
                case IfcUnitEnum.SOLIDANGLEUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
                case IfcUnitEnum.AREAUNIT:
                    return new XbimDimensionalExponents(2, 0, 0, 0, 0, 0, 0);
                case IfcUnitEnum.VOLUMEUNIT:
                    return new XbimDimensionalExponents(3, 0, 0, 0, 0, 0, 0);
                case IfcUnitEnum.ABSORBEDDOSEUNIT:
                    return new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0);
                case IfcUnitEnum.RADIOACTIVITYUNIT:
                    return new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0);
                case IfcUnitEnum.ELECTRICCAPACITANCEUNIT:
                    return new XbimDimensionalExponents(-2, 1, 4, 1, 0, 0, 0);
                case IfcUnitEnum.DOSEEQUIVALENTUNIT:
                    return new XbimDimensionalExponents(2, 0, -2, 0, 0, 0, 0);
                case IfcUnitEnum.ELECTRICCHARGEUNIT:
                    return new XbimDimensionalExponents(0, 0, 1, 1, 0, 0, 0);
                case IfcUnitEnum.ELECTRICCONDUCTANCEUNIT:
                    return new XbimDimensionalExponents(-2, -1, 3, 2, 0, 0, 0);
                case IfcUnitEnum.ELECTRICVOLTAGEUNIT:
                    return new XbimDimensionalExponents(2, 1, -3, -1, 0, 0, 0);
                case IfcUnitEnum.ELECTRICRESISTANCEUNIT:
                    return new XbimDimensionalExponents(2, 1, -3, -2, 0, 0, 0);
                case IfcUnitEnum.ENERGYUNIT:
                    return new XbimDimensionalExponents(2, 1, -2, 0, 0, 0, 0);
                case IfcUnitEnum.FORCEUNIT:
                    return new XbimDimensionalExponents(1, 1, -2, 0, 0, 0, 0);
                case IfcUnitEnum.FREQUENCYUNIT:
                    return new XbimDimensionalExponents(0, 0, -1, 0, 0, 0, 0);
                case IfcUnitEnum.INDUCTANCEUNIT:
                    return new XbimDimensionalExponents(2, 1, -2, -2, 0, 0, 0);
                case IfcUnitEnum.ILLUMINANCEUNIT:
                    return new XbimDimensionalExponents(-2, 0, 0, 0, 0, 0, 1);
                case IfcUnitEnum.LUMINOUSFLUXUNIT:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 1);
                case IfcUnitEnum.MAGNETICFLUXUNIT:
                    return new XbimDimensionalExponents(2, 1, -2, -1, 0, 0, 0);
                case IfcUnitEnum.MAGNETICFLUXDENSITYUNIT:
                    return new XbimDimensionalExponents(0, 1, -2, -1, 0, 0, 0);
                case IfcUnitEnum.POWERUNIT:
                    return new XbimDimensionalExponents(2, 1, -3, 0, 0, 0, 0);
                case IfcUnitEnum.PRESSUREUNIT:
                    return new XbimDimensionalExponents(-1, 1, -2, 0, 0, 0, 0);
                default:
                    return new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
            }
        }

        public static XbimDimensionalExponents DeriveDimensionalExponents(IEnumerable<IfcDerivedUnitElement> unitElements)
		  {
            var elements = unitElements as IList<IfcDerivedUnitElement> ?? unitElements.ToList();
            if(!elements.Any())
				  throw new ArgumentNullException();

			  #region Strict Implementation
			  var result = new XbimDimensionalExponents(0, 0, 0, 0, 0, 0, 0);
			  foreach (var unitElement in elements)
			  {
				  result.LengthExponent = result.LengthExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.LengthExponent);
				  result.MassExponent = +result.MassExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.MassExponent);
				  result.TimeExponent = result.TimeExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.TimeExponent);
				  result.ElectricCurrentExponent = result.ElectricCurrentExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.ElectricCurrentExponent);
				  result.ThermodynamicTemperatureExponent = result.ThermodynamicTemperatureExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.ThermodynamicTemperatureExponent);
				  result.AmountOfSubstanceExponent = result.AmountOfSubstanceExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.AmountOfSubstanceExponent);
				  result.LuminousIntensityExponent = result.LuminousIntensityExponent +
						(unitElement.Exponent * unitElement.Unit.Dimensions.LuminousIntensityExponent);
			  }
			  return result;
			  #endregion Strict Implementation
		  }
        public static XbimDimensionalExponents DeriveDimensionalExponents(IfcUnit unit)
        {
            var derivedUnit = unit as IfcDerivedUnit;
            if (derivedUnit != null)
				  return DeriveDimensionalExponents(derivedUnit.Elements);
           throw new NotImplementedException();
        }

        public static XbimDimensionalExponents CorrectUnitAssignment(List<IfcUnit> units)
        {
            throw new NotImplementedException();
        }

      
    }
}