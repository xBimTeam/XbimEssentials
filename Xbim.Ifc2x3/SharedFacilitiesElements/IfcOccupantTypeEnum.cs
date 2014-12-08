using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
    /// <summary>
    /// IfcOccupantTypeEnum defines the types of occupant from which the type required can be selected.
    /// </summary>
    public enum IfcOccupantTypeEnum
    {
        /// <summary>
        /// Actor receiving the assignment of a property agreement from an assignor
        /// </summary>
        ASSIGNEE,
        /// <summary>
        /// Actor assigning a property agreement to an assignor
        /// </summary>
        ASSIGNOR,
        /// <summary>
        /// Actor receiving the lease of a property from a lessor
        /// </summary>
        LESSEE,
        /// <summary>
        /// Actor leasing a property to a lessee
        /// </summary>
        LESSOR,
        /// <summary>
        /// Actor participating in a property agreement on behalf of an owner, lessor or assignor
        /// </summary>
        LETTINGAGENT,
        /// <summary>
        /// Actor that owns a property
        /// </summary>
        OWNER,
        /// <summary>
        /// Actor renting the use of a property for a period of time
        /// </summary>
        TENANT,
        /// <summary>
        /// Actor is user defined
        /// </summary>
        USERDEFINED,
        /// <summary>
        /// Actor is not defined
        /// </summary>
        NOTDEFINED
    }
}
