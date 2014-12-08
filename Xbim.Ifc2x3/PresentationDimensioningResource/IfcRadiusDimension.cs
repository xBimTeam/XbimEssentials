using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// The radial dimension is a draughting callout that presents the radial length of a conic element. It consists of a dimension curve and may have projection curves (but is often defined without projection curves).
    /// </summary>
    [IfcPersistedEntity]
    public class IfcRadiusDimension : IfcDimensionCurveDirectedCallout
    {
    }
}
