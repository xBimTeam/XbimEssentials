using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.ProcessExtensions
{
    /// <summary>
    /// An IfcWorkPlan represents work plans in a construction or a facilities management project.
    /// </summary>
    /// <remarks>
    /// A work plan contains a set of work schedules for different purposes (including construction and facilities management). 
    /// Through inheritance from IfcWorkControl, it also have references to all the activities (i.e. IfcTask) and resources used in the 
    /// work schedules.
    /// 
    /// A work plan has information such as start date, finish date, total free float, and so on. IfcWorkPlan can also refer to the 
    /// construction project represented by the single IfcProject instance.
    /// </remarks>
    public class IfcWorkPlan : IfcWorkControl
    {
        // No implementation
    }
}
