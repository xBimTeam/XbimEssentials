using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcDimensionCurveDirectedCallout : IfcDraughtingCallout
    {
        /// <summary>
        /// Definition from IAI: The dimension curve directed callout is a dimension callout, which includes a dimension line. It normally presents an extent and/or direction of the product shape. Subtypes are introduced to declare specific forms of dimension curve directed callouts, such as:
        ///
        ///    linear dimension
        ///    radius dimension
        ///    diameter dimension
        ///    angular dimension
        ///
        /// </summary>
        /// <returns></returns>
        public override string WhereRule()
        {
            var result = "";
            if (Contents.Count(e => e is IfcDimensionCurve) != 1)
            {
                result += "WR41: There shall be exactly one dimension curve in the set of draughting callout elements. \n";
            }
            if (Contents.Count(e => e is IfcProjectionCurve) > 2)
            {
                result += "WR42: There shall be only zero, one, or two projection curves within the content of the callout. \n";
            }

            return result;
        }
    }
}
