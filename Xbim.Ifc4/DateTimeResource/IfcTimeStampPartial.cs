using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc4.DateTimeResource
{

    public partial struct IfcTimeStamp
    {
        public DateTime ToDateTime()
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from 1970/1/1 00:00:00
            return (dt.AddSeconds(this));
        }
       
    }

}
