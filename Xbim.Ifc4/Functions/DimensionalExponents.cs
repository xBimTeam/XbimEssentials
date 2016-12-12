namespace Xbim.Ifc4.Functions
{
    internal class DimensionalExponents
    {
        private readonly int _len;
        private readonly int _mass;
        private readonly int _time;
        private readonly int _elec;
        private readonly int _temp;
        private readonly int _substance;
        private readonly int _lum;

        internal DimensionalExponents(int len, int mass, int time, int elec, int temp, int substance, int lum)
        {
            _len = len;
            _mass = mass;
            _time = time;
            _elec = elec;
            _temp = temp;
            _substance = substance;
            _lum = lum;
        }
    }
}