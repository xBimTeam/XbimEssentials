#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc2x3
#endregion

namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcCostScheduleTypeEnum is a list of the available types of cost schedule from which that required may be selected.
    /// </summary>
    public enum IfcCostScheduleTypeEnum
    {
        /// <summary>
        /// An allocation of money for a particular purpose.
        /// </summary>
        BUDGET,
        /// <summary>
        /// An assessment of the amount of money needing to be expended for a defined purpose based on incomplete information about the goods and services required for a construction or installation.
        /// </summary>
        COSTPLAN,
        /// <summary>
        /// An assessment of the amount of money needing to be expended for a defined purpose based on actual information about the goods and services required for a construction or installation.
        /// </summary>
        ESTIMATE,
        /// <summary>
        /// An offer to provide goods and services.
        /// </summary>
        TENDER,
        /// <summary>
        /// A complete listing of all work items forming construction or installation works in which costs have been allocated to work items.
        /// </summary>
        PRICEDBILLOFQUANTITIES,
        /// <summary>
        /// A complete listing of all work items forming construction or installation works in which costs have not yet been allocated to work items.
        /// </summary>
        UNPRICEDBILLOFQUANTITIES,
        /// <summary>
        /// A listing of each type of goods forming construction or installation works with the cost of purchase, construction/installation, overheads and profit assigned so that additional items of that type can be costed.
        /// </summary>
        SCHEDULEOFRATES,
        USERDEFINED,
        NOTDEFINED
    }
}
