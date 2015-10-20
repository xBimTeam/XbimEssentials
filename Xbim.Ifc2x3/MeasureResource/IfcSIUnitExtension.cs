using System;

namespace Xbim.Ifc2x3.MeasureResource
{
    // ReSharper disable once InconsistentNaming
    public partial class IfcSIUnit
    {
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
    }
}
