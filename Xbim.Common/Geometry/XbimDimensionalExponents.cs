using System;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.Common.Geometry
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
    }
}