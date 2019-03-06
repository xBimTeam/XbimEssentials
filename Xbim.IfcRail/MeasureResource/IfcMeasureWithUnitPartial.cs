namespace Xbim.IfcRail.MeasureResource
{
    public partial class IfcMeasureWithUnit
    {
        public string AsString()
        {
            string value = string.Format("{0,0:N2}", ValueComponent.Value);
            var ifcUnit = UnitComponent;
            string unit = ifcUnit.Symbol();          
            if (!string.IsNullOrEmpty(unit))
            {
                value += unit;
            }
            return value;
        }
    }
}
