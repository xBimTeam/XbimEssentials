using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    [IfcPersistedEntity]
    public class IfcDimensionCalloutRelationship : IfcDraughtingCalloutRelationship
    {
        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (!new[] { "primary", "secondary" }.Any(v => v == Name))
                result += "WR11: The name of the relationship shall either be 'primary' or 'secondary'. \n";
            if (!( RelatingDraughtingCallout is IfcDimensionCurveDirectedCallout))
                result += "WR12: The relating draughting callout shall be a dimension (linear, diameter, radius, or angular). \n";

            if (RelatedDraughtingCallout is IfcDimensionCurveDirectedCallout)
                result += "WR13: The related draughting callout shall not be a dimension (linear, diameter, radius, or angular) \n";

            return result;
        }
    }
}
