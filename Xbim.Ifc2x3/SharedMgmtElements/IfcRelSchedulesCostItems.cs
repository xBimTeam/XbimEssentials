using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcRelSchedulesCostItems is a subtype of IfcRelAssignsToControl that enables one or many instances of IfcCostItem to be assigned to an instance of IfcCostSchedule.
    /// HISTORY: New Entity in IFC Release 2.0. Modified in IFC 2x2
    /// </summary>
    public class IfcRelSchedulesCostItems : IfcRelAssignsToControl
    {
        // todo: implement where()
        //WHERE
        //WR11	 : 	SIZEOF(QUERY(temp <* SELF\IfcRelAssigns.RelatedObjects | NOT('IFCSHAREDMGMTELEMENTS.IFCCOSTITEM' IN TYPEOF(temp)) )) = 0;
        //WR12	 : 	'IFCSHAREDMGMTELEMENTS.IFCCOSTSCHEDULE' IN TYPEOF (SELF\IfcRelAssignsToControl.RelatingControl);
    }
}
