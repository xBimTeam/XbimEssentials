using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
    /// <summary>
    /// A draughting callout is a collection of annotated curves, symbols and text that presents some 
    /// product shape or definition properties within a drawing.
    ///
    ///EXAMPLE: Draughting callout are e.g., dimensioning and leader directed notes.
    ///
    ///NOTE: The IfcDraughtingCallout is an entity that had been adopted from ISO 10303, 
    ///Industrial automation systems and integration—Product data representation and exchange,
    ///Part 101: Integrated application resources: Draughting.
    ///
    ///NOTE Corresponding STEP name: draughting_callout. Please refer to ISO/IS 10303-101:1994 
    ///page 20 for the final definition of the formal standard.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcDraughtingCallout : IfcGeometricRepresentationItem
    {
        public IfcDraughtingCallout()
        {
            _contents = new XbimSet<IfcDraughtingCalloutElement>(this);
        }

        #region fields
        private XbimSet<IfcDraughtingCalloutElement> _contents;
        #endregion

        /// <summary>
        /// The annotation curves, symbols, or text comprising the presentation of information. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcDraughtingCalloutElement> Contents
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _contents;
            }
            set { this.SetModelValue(this, ref _contents, value, v => Contents = v, "Contents"); }
        }


        #region Inverse Relationships
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcDraughtingCalloutRelationship> IsRelatedFromCallout
        {
            get { return ModelOf.Instances.Where<IfcDraughtingCalloutRelationship>(a => a.RelatedDraughtingCallout == this); }
        }

        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcDraughtingCalloutRelationship> IsRelatedToCallout
        {
            get { return ModelOf.Instances.Where<IfcDraughtingCalloutRelationship>(a => a.RelatingDraughtingCallout == this); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _contents.Add((IfcDraughtingCalloutElement)(value.EntityVal));
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
