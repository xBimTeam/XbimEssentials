using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// Definition from IAI: A dimension curve is an annotated curve within a dimension that 
    /// has the dimension text and may have terminator symbols assigned. It is used to 
    /// present the extent and the direction of the dimension.
    ///
    /// NOTE: The dimension text is not directly associated to the IfcDimensionCurve. 
    /// It is associated as IfcStructuredDimensionCallout via the IfcDimensionCalloutRelationship 
    /// to the dimension (one of IfcAngularDimension, IfcDiameterDimension, IfcLinearDimension, or IfcRadiusDimension).
    /// 
    /// NOTE: The IfcDimensionCurve is an entity that had been adopted from ISO 10303, 
    /// Industrial automation systems and integration—Product data representation and exchange, 
    /// Part 101: Integrated application resources: Draughting.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcDimensionCurve : IfcAnnotationCurveOccurrence
    {

        public override string WhereRule()
        {
            var result = base.WhereRule();

            if (!ModelOf.Instances.Where<IfcDraughtingCallout>(dc => dc.Contents.Contains(this)).Any())
                result += "WR51: A dimension curve shall be used by at least one referencing dimension curve directed callout, i.e. it should not be used outside of the context of a dimension element group.   \n";

            if (AnnotatedBySymbols.OfType<IfcDimensionCurveTerminator>().Count(t => t.Role == IfcDimensionExtentUsage.ORIGIN) > 1 ||
                AnnotatedBySymbols.OfType<IfcDimensionCurveTerminator>().Count(t => t.Role == IfcDimensionExtentUsage.TARGET) > 1)
                result += "WR52: The dimension curve should not be annotated with more than one terminator having the role \"Origin\", nor with more than one terminator having the role \"Target\".  \n";

            if (AnnotatedBySymbols.Count(a => !(a is IfcDimensionCurveTerminator)) != 0)
                result += "WR53: All terminators assigned to a dimension curve shall be dimension curve terminators. \n";

            return result;
        }

        /// <summary>
        ///  	Reference to the terminator symbols that may be assigned to the dimension curve. 
        ///  	There shall be either zero, one or two terminator symbols assigned.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 2)]
        public IEnumerable<IfcTerminatorSymbol> AnnotatedBySymbols
        {
            get { return ModelOf.Instances.Where<IfcTerminatorSymbol>(a => a.AnnotatedCurve == this); }
        }
    }
}
