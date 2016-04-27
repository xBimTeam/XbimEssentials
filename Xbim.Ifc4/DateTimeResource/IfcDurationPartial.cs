using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Xbim.Ifc4.DateTimeResource
{
    public partial struct IfcDuration
    {
        public TimeSpan ToTimeSpan()
        {
            return XmlConvert.ToTimeSpan(Value.ToString());
        }
    }
}
