using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// The linear dimension is a draughting callout that presents the length (or distance) between two points along a linear curve. It consists of a dimension curve and optionally one or two projection curves.
    /// 
    /// NOTE: The dimension text is handled through the IfcStructuredDimensionCallout and associated via the IfcDimensionCalloutRelationship.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLinearDimension : IfcDimensionCurveDirectedCallout
    {

    }
}
