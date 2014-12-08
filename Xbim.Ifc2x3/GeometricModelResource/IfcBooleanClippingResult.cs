#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBooleanClippingResult.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class IfcBooleanClippingResult : IfcBooleanResult
    {
        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (!(FirstOperand is IfcSweptAreaSolid || FirstOperand is IfcBooleanResult))
                baseErr +=
                    "WR1: BooleanClippingResult: The first operand of the Boolean clipping operation shall be either an IfcSweptAreaSolid or (in case of more than one clipping) an IfcBooleanResult.\n";
            if (!(SecondOperand is IfcHalfSpaceSolid))
                baseErr +=
                    "WR2: BooleanClippingResult: The second operand of the Boolean clipping operation shall be an IfcHalfSpaceSolid.\n";
            if (Operator != IfcBooleanOperator.Difference)
                baseErr += "WR3: BooleanClippingResult: The Boolean operator for clipping is always Difference.\n";
            return baseErr;
        }

        #endregion
    }
}