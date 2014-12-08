using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcDimensionPair : IfcDraughtingCalloutRelationship
    {
        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (!new[] { "chained", "parallel" }.Any(v => v == Name))
                result += "WR11: The name of the relationship shall either be 'chained' or 'parallel'. \n";

            if (!(RelatingDraughtingCallout is IfcDimensionCurveDirectedCallout))
                result += "WR12: The relating draughting callout shall be a dimension. \n";

            if (!(RelatedDraughtingCallout is IfcDimensionCurveDirectedCallout))
                result += "WR12: The related draughting callout shall be a dimension. \n";

            return result;
        }
    }
}
