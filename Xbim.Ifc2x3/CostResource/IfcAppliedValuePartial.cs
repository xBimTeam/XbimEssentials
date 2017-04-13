using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.DateTimeResource;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3.CostResource
{
    abstract partial  class IfcAppliedValue
    {
        public string AsString()
        {
            
            StringBuilder value = new StringBuilder();
            if ((Description.HasValue) &&
                (!string.IsNullOrEmpty(Description))
                )
            {
                value.Append(Description);
                value.Append(", ");
            }

            if (AppliedValue != null)//not nullable should be? incorrect name?
            {
                value.Append("AppliedValue: ");
                if (AppliedValue is IfcRatioMeasure)
                {
                    var ifcRatioMeasure = (IfcRatioMeasure)AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcRatioMeasure.Value));
                }
                if (AppliedValue is IfcMonetaryMeasure)
                {
                    IfcMonetaryMeasure ifcMonetaryMeasure = (IfcMonetaryMeasure)AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcMonetaryMeasure.Value));
                }
                if (AppliedValue is IfcMeasureWithUnit)
                {
                    value.Append(((IfcMeasureWithUnit)AppliedValue).AsString());

                }
                value.Append(", ");
            }

            if (UnitBasis != null) //not nullable should be?
            {
                value.Append("UnitBase: ");
                value.Append(((IfcMeasureWithUnit)UnitBasis).AsString());
                value.Append(", ");
            }
            if (ApplicableDate != null) //not nullable should be?
            {
                value.Append("ApplicableDate: ");
                value.Append(ApplicableDate.AsString());
                value.Append(", ");
            }
            if (FixedUntilDate != null) //not nullable should be?
            {
                value.Append("FixedUntilDate: ");
                value.Append(FixedUntilDate.AsString());
                value.Append(", ");
            }

            if ( this is IfcCostValue)
            {
                IfcCostValue ifcCostValue = (IfcCostValue)this;
                if (ifcCostValue.CostType != null)
                {
                    value.Append("CostType: ");
                    value.Append(ifcCostValue.CostType);
                    value.Append(", ");
                }

                if (ifcCostValue.Condition != null)//not nullable should be?
                {
                    value.Append("Condition: ");
                    value.Append(ifcCostValue.Condition);
                    value.Append(", ");
                }
            }
            if (this is IfcEnvironmentalImpactValue)
            {
                IfcEnvironmentalImpactValue ifcEnvironmentalImpactValue = (IfcEnvironmentalImpactValue)this;
                if (ifcEnvironmentalImpactValue.ImpactType != null)
                {
                    value.Append("ImpactType: ");
                    value.Append(ifcEnvironmentalImpactValue.ImpactType);
                    value.Append(", ");
                }

                //enum so should have a value as not nullable
                value.Append("Category: ");
                value.Append(ifcEnvironmentalImpactValue.Category.ToString());
                value.Append(", ");

                if (ifcEnvironmentalImpactValue.UserDefinedCategory != null)//not nullable should be?
                {
                    value.Append("UserDefinedCategory: ");
                    value.Append(ifcEnvironmentalImpactValue.UserDefinedCategory);
                    value.Append(", ");
                }
            }
            return value.ToString();
        }

        
    }
}
