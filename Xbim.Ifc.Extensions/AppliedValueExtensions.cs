using System.Text;
using Xbim.Ifc2x3.CostResource;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class AppliedValueExtensions
    {
        public static string GetAsString(this IfcAppliedValue ifcAppliedValue)
        {
            
            StringBuilder value = new StringBuilder();
            if ((ifcAppliedValue.Description.HasValue) &&
                (!string.IsNullOrEmpty(ifcAppliedValue.Description))
                )
            {
                value.Append(ifcAppliedValue.Description);
                value.Append(", ");
            }

            if (ifcAppliedValue.AppliedValue != null)//not nullable should be? incorrect name?
            {
                value.Append("AppliedValue: ");
                if (ifcAppliedValue.AppliedValue is IfcRatioMeasure)
                {
                    IfcRatioMeasure ifcRatioMeasure = (IfcRatioMeasure)ifcAppliedValue.AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcRatioMeasure.Value));
                }
                if (ifcAppliedValue.AppliedValue is IfcMonetaryMeasure)
                {
                    IfcMonetaryMeasure ifcMonetaryMeasure = (IfcMonetaryMeasure)ifcAppliedValue.AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcMonetaryMeasure.Value));
                }
                if (ifcAppliedValue.AppliedValue is IfcMeasureWithUnit)
                {
                    value.Append(GetMeasureWithUnitAsString((IfcMeasureWithUnit)ifcAppliedValue.AppliedValue));
                    
                }
                value.Append(", ");
            }

            if (ifcAppliedValue.UnitBasis != null) //not nullable should be?
            {
                value.Append("UnitBase: ");
                value.Append(GetMeasureWithUnitAsString((IfcMeasureWithUnit)ifcAppliedValue.UnitBasis));
                value.Append(", ");
            }
            if (ifcAppliedValue.ApplicableDate != null) //not nullable should be?
            {
                value.Append("ApplicableDate: ");
                value.Append(ifcAppliedValue.ApplicableDate.GetAsString());
                value.Append(", ");
            }
            if (ifcAppliedValue.FixedUntilDate != null) //not nullable should be?
            {
                value.Append("FixedUntilDate: ");
                value.Append(ifcAppliedValue.FixedUntilDate.GetAsString());
                value.Append(", ");
            }

            if (ifcAppliedValue is IfcCostValue)
            {
                IfcCostValue ifcCostValue = (IfcCostValue)ifcAppliedValue;
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
            if (ifcAppliedValue is IfcEnvironmentalImpactValue)
            {
                IfcEnvironmentalImpactValue ifcEnvironmentalImpactValue = (IfcEnvironmentalImpactValue)ifcAppliedValue;
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

        private static string GetMeasureWithUnitAsString(IfcMeasureWithUnit ifcMeasureWithUnit)
        {
            string value = string.Format("{0,0:N2}", ifcMeasureWithUnit.ValueComponent.Value);
            IfcUnit ifcUnit = ifcMeasureWithUnit.UnitComponent;
            string unit = ifcUnit.GetSymbol();
            if (!string.IsNullOrEmpty(unit))
            {
                value += unit;
            }
            
            return value;
        }
    }
}
