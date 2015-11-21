using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.MeasureResource
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
