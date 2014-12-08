using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    /// <summary>
    /// IfcRelOccupiesSpaces is a relationship class that further constrains the parent relationship IfcRelAssignsToActor 
    /// to a relationship between occupants (IfcOccupant) and either a space (IfcSpace), a collection of spaces (IfcZone), 
    /// a building storey (IfcBuildingStorey), or a building (IfcBuilding).
    /// </summary>
    [IfcPersistedEntity]
    public class IfcRelOccupiesSpaces : IfcRelAssignsToActor
    {

    }
}
