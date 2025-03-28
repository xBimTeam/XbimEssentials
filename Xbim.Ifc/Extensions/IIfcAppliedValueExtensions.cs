using System;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc
{
    public static class IIfcAppliedValueExtensions
    {
        public static string AsString(this IIfcAppliedValue obj)
        {

            StringBuilder value = new StringBuilder();
            if ((obj.Description.HasValue) &&
                (!string.IsNullOrEmpty(obj.Description))
                )
            {
                value.Append(obj.Description);
                value.Append(", ");
            }

            if (obj.AppliedValue != null)//not nullable should be? incorrect name?
            {
                value.Append("AppliedValue: ");
                if (obj.AppliedValue is IfcRatioMeasure)
                {
                    var ifcRatioMeasure = (IfcRatioMeasure)obj.AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcRatioMeasure.Value));
                }
                if (obj.AppliedValue is IfcMonetaryMeasure)
                {
                    IfcMonetaryMeasure ifcMonetaryMeasure = (IfcMonetaryMeasure)obj.AppliedValue;
                    value.Append(string.Format("{0,0:N2}", ifcMonetaryMeasure.Value));
                }
                if (obj.AppliedValue is IfcMeasureWithUnit)
                {
                    value.Append(((IfcMeasureWithUnit)obj.AppliedValue).AsString());

                }
                value.Append(", ");
            }

            if (obj.UnitBasis != null) //not nullable should be?
            {
                value.Append("UnitBase: ");
                value.Append(((IfcMeasureWithUnit)obj.UnitBasis).AsString());
                value.Append(", ");
            }
            if (obj.ApplicableDate != null) //not nullable should be?
            {
                value.Append("ApplicableDate: ");
                value.Append(obj.ApplicableDate.ToString());
                value.Append(", ");
            }
            if (obj.FixedUntilDate != null) //not nullable should be?
            {
                value.Append("FixedUntilDate: ");
                value.Append(obj.FixedUntilDate.ToString());
                value.Append(", ");
            }
            
            if (obj is IIfcCostValue)
            {
                IIfcCostValue ifcCostValue = (IIfcCostValue)obj;
                if (ifcCostValue.Category != null)
                {
                    value.Append("CostType: ");
                    value.Append(ifcCostValue.Category);
                    value.Append(", ");
                }

                if (ifcCostValue.Condition != null)//not nullable should be?
                {
                    value.Append("Condition: ");
                    value.Append(ifcCostValue.Condition);
                    value.Append(", ");
                }
            }
            if (obj is Ifc2x3.CostResource.IfcEnvironmentalImpactValue)
            {
                Ifc2x3.CostResource.IfcEnvironmentalImpactValue ifcEnvironmentalImpactValue = (Ifc2x3.CostResource.IfcEnvironmentalImpactValue)obj;
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
