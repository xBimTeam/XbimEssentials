using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// Definition from IAI: A terminator symbol is a special type of an annotated symbol which is assigned to a curve to indicate a direction, origin, target, or any other associated meaning.
    /// 
    /// NOTE: The IfcTerminatorSymbol is an entity that had been adopted from ISO 10303, Industrial automation systems and integration—Product data representation and exchange, Part 101: Integrated application resources: Draughting.
    /// 
    /// NOTE Corresponding STEP name: terminator_symbol. Please refer to ISO/IS 10303-101:1994 page 19 for the final definition of the formal standard.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTerminatorSymbol : IfcAnnotationSymbolOccurrence
    {
        #region
        IfcAnnotationCurveOccurrence _annotatedCurve;
        #endregion

        /// <summary>
        /// The curve being annotated by the terminator symbol. 
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcAnnotationCurveOccurrence AnnotatedCurve
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _annotatedCurve;
            }
            set { this.SetModelValue(this, ref _annotatedCurve, value, v => AnnotatedCurve = v, "AnnotatedCurve"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _annotatedCurve = (IfcAnnotationCurveOccurrence)(value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
