using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.ProcessExtensions
{
    /// <summary>
    /// An IfcWorkSchedule represents a task schedule in a work plan, which in turn can contain a set of schedules for different purposes.
    /// </summary>
    /// <remarks>
    /// An IfcWorkSchedule includes a set of elements (created through relating schedule time controls to tasks) with references to the 
    /// resources used for the tasks included in the work schedule. Additionally, through the IfcWorkControl abstract supertype, 
    /// the actors creating the schedule can be specified and schedule time information such as start time, finish time, and total 
    /// float of the schedule can also be specified.
    ///
    /// IfcWorkSchedule can reference a project (i.e. the single IfcProject instance). The documents of the IfcWorkSchedule can be 
    /// referenced by the IfcRelAssociatesDocuments relationship. Moreover, a work schedule can include other work schedules as sub-items
    /// through IfcRelNests relationship.
    /// </remarks>
    public class IfcWorkSchedule : IfcWorkControl
    {
        // No implementation
    }
}
