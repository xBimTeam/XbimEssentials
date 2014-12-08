using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// A projection curve is an annotated curve within a dimension that points to a point of the product shape that is measured.
    /// 
    /// NOTE: The IfcProjectionCurve is an entity that had been adopted from ISO 10303, 
    /// Industrial automation systems and integration—Product data representation and exchange, 
    /// Part 101: Integrated application resources: Draughting.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcProjectionCurve : IfcAnnotationCurveOccurrence
    {
    }
}
