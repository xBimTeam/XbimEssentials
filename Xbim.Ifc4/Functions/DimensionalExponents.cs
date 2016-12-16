namespace Xbim.Ifc4.Functions
{
    internal class DimensionalExponents
    {

        internal DimensionalExponents(int len, int mass, int time, int elec, int temp, int substance, int lum)
        {
            LengthExponent = len;
            MassExponent = mass;
            TimeExponent = time;
            ElectricCurrentExponent = elec;
            ThermodynamicTemperatureExponent = temp;
            AmountOfSubstanceExponent = substance;
            LuminousIntensityExponent = lum;
        }

        public long LengthExponent { get; set; }
        public long MassExponent { get; set; }
        public long TimeExponent { get; set; }
        public long ElectricCurrentExponent { get; set; }
        public long ThermodynamicTemperatureExponent { get; set; }
        public long AmountOfSubstanceExponent { get; set; }
        public long LuminousIntensityExponent { get; set; }
    }
}