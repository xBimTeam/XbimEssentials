using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ConstraintResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.ControlExtension
{
    /// <summary>
    /// The entity IfcRelAssociatesConstraint is used to apply constraint information defined by IfcConstraint, in IfcConstraintResource schema, to all subtypes of IfcRoot. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcRelAssociatesConstraint : IfcRelAssociates
    {
        private IfcLabel _Intent;

        /// <summary>
        /// The intent of the constraint usage with regard to its related IfcConstraint and IfcObjects, IfcPropertyDefinitions or IfcRelationships. Typical values can be e.g. RATIONALE or EXPECTED PERFORMANCE. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcLabel Intent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Intent;
            }
            set { this.SetModelValue(this, ref _Intent, value, v => Intent = v, "Intent"); }
        }

        private IfcConstraint _RelatingConstraint;

        /// <summary>
        /// Reference to constraint that is being applied using this relationship.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcConstraint RelatingConstraint
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RelatingConstraint;
            }
            set { this.SetModelValue(this, ref _RelatingConstraint, value, v => RelatingConstraint = v, "RelatingConstraint"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _Intent = value.StringVal;
                    break;
                case 6:
                    _RelatingConstraint = (IfcConstraint)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
