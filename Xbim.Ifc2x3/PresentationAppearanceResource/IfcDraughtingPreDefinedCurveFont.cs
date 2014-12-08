#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDraughtingPreDefinedCurveFont.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcDraughtingPreDefinedCurveFont : IfcPreDefinedCurveFont
    {
        #region Constructors

        public IfcDraughtingPreDefinedCurveFont()
        {
            SetName(ValidFontNames[0]);
        }

        #endregion

        public static string[] ValidFontNames = new[]
                                                    {
                                                        "continuous", "chain", "chain double dash", "dashed", "dotted",
                                                        "by layer"
                                                    };

        public static IfcDraughtingPreDefinedCurveFont[] PrefinedCurveFonts;

        static IfcDraughtingPreDefinedCurveFont()
        {
            PrefinedCurveFonts = new IfcDraughtingPreDefinedCurveFont[ValidFontNames.Length];
            for (int i = 0; i < ValidFontNames.Length; i++)
            {
                PrefinedCurveFonts[i] = new IfcDraughtingPreDefinedCurveFont();
                PrefinedCurveFonts[i].SetName(ValidFontNames[i]);
            }
        }


        public override string WhereRule()
        {
            if (!ValidFontNames.Contains(((string) Name).ToLower()))
                return
                    @"WR31 DraughtingPreDefinedCurveFont : The name of the draughting_pre_defined_curve_font shall be 'continuous’, ’chain’, ’chain', double dash’, ’dashed’, or ’dotted’";
            else
                return "";
        }
    }
}