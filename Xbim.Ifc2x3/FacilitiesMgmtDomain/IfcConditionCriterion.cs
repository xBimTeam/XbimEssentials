using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc.SelectTypes;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcConditionCriterion is a particular measured or assessed criterion that contributes to the overall condition of an artefact.
    /// </summary>
    /// <remarks>
    /// An IfcConditionCriterion may be either an observed/assessed value or a measured value. This is determined by selection through the 
    /// IfcConditionCriterionSelect type that has a datatype either of IfcLabel (for a numeric or alphanumeric scale observation 
    /// e.g. on a scale of 1 to 10 where 1 represents 'as new' and 10 represents 'urgent replacement required') or of 
    /// IfcMeasureWithUnit (for a measured criterion that also includes units of measure).
    /// Each criterion must be named and optionally, may also have a description. A description used for a condition should be persistent 
    /// so that there is absolute consistency in condition recording.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcConditionCriterion : IfcControl
    {

        #region Fields

        private IfcConditionCriterionSelect _criterion;
        private IfcDateTimeSelect _criterionDateTime;

        #endregion

        #region Properties

        /// <summary>
        /// The measured or assessed value of a criterion.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcConditionCriterionSelect Criterion
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _criterion;
            }
            set { this.SetModelValue(this, ref _criterion, value, v => Criterion = v, "Criterion"); }
        }


        
        /// <summary>
        /// The time and/or date at which the criterion is determined.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcDateTimeSelect CriterionDateTime
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _criterionDateTime;
            }
            set { this.SetModelValue(this, ref _criterionDateTime, value, v => CriterionDateTime = v, "CriterionDateTime"); }
        }

        #endregion

        #region Methods
        
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
                    _criterion = (IfcConditionCriterionSelect)value.EntityVal;
                    break;
                case 6:
                    _criterionDateTime  = (IfcDateTimeSelect)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            string baseErr = base.WhereRule();

            if (!this.Name.HasValue)
            {
                baseErr += "WR1 IfcConditionCriterion : The Name attribute has to be provided for the condition criterion";
            }

            return baseErr;
        }
        #endregion
      
    }
}
