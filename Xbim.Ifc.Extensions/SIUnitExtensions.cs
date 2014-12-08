#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    SIUnitExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Ifc2x3.MeasureResource;
using System;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class SIUnitExtensions
    {
        /// <summary>
        ///   returns the power of the SIUnit prefix, i.e. MILLI = 0.001, if undefined returns 1.0
        /// </summary>
        /// <param name = "si"></param>
        /// <returns></returns>
        public static double Power(this IfcSIUnit si)
        {
            int exponential = 1;
            if (si.UnitType == IfcUnitEnum.AREAUNIT) exponential = 2;
            if (si.UnitType == IfcUnitEnum.VOLUMEUNIT) exponential = 3;
            double factor;
            if (si.Prefix.HasValue)
            {
                switch (si.Prefix.Value)
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
            else
                return 1.0;
        }
    }
}