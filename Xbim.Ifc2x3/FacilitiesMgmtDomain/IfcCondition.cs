using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcCondition determines the state or condition of an element at a particular point in time. 
    /// </summary>
    /// <remarks>
    /// IfcCondition is a particular subtype of IfcGroup that can contain only instances of IfcConditionCriterion. 
    /// The objectified relationship class IfcRelAssignsToGroup is used to assign the related instances of IfcConditionCriterion 
    /// to the relating instance of IfcCondition. 
    /// An IfcCondition is determined either from an observed or a measured state (see IfcConditionCriterion). 
    /// The condition is determined at a particular point in time, the time being determined through the IfcRelAssociatesDateTime 
    /// class with an appropriate designation for the value of the DateTimeType e.g. AssessmentDate. Note that other dates may be 
    /// assigned to IfcCondition for relevant purposes e.g. to recommend the date for the next condition assessment.
    /// IfcRelAssignsToProduct is used to relate one or more instances of IfcCondition to an artifact that is an instance of a 
    /// subtype of IfcProduct. For an instance of IfcAsset, condition is related through the use of IfcRelAssignsToGroup.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcCondition : IfcGroup
    {
        
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
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            return base.WhereRule();
        }
    }
}
