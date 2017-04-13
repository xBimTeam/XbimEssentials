using System.Text;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc4.CostResource
{
    public partial class IfcAppliedValue
    {
        public string AsString()
        {
            
            StringBuilder value = new StringBuilder();
            if (!string.IsNullOrEmpty(Description))
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
                var unit = AppliedValue as IfcMeasureWithUnit;
                if (unit != null)
                {
                    value.Append(unit.AsString());

                }
                value.Append(", ");
            }

            if (UnitBasis != null) //not nullable should be?
            {
                value.Append("UnitBase: ");
                value.Append(UnitBasis.AsString());
                value.Append(", ");
            }
            if (ApplicableDate != null) //not nullable should be?
            {
                value.Append("ApplicableDate: ");
                value.Append(ApplicableDate.ToString());
                value.Append(", ");
            }
            if (FixedUntilDate != null) //not nullable should be?
            {
                value.Append("FixedUntilDate: ");
                value.Append(FixedUntilDate.ToString());
                value.Append(", ");
            }

            return value.ToString();
        }

        
    }
}
