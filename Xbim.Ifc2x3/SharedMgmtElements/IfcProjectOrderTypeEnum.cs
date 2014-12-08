namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcProjectOrderTypeEnum is a list of the types of project order that may be identified.
    /// </summary>
    public enum IfcProjectOrderTypeEnum
    {
        /// <summary>
        /// An instruction to make a change to a product or work being undertaken and a description of the work that is to be performed.	
        /// </summary>
        CHANGEORDER,
        /// <summary>
        /// An instruction to carry out maintenance work and a description of the work that is to be performed.	
        /// </summary>
        MAINTENANCEWORKORDER,
        /// <summary>
        /// An instruction to move persons and artefacts and a description of the move locations, objects to be moved, etc.	
        /// </summary>
        MOVEORDER,
        /// <summary>
        /// An instruction to purchase goods and/or services and a description of the goods and/or services to be purchased that is to be performed.	
        /// </summary>
        PURCHASEORDER,
        /// <summary>
        /// A general instruction to carry out work and a description of the work to be done. Note the difference between a work order generally and a maintenance work order.	
        /// </summary>
        WORKORDER,
        USERDEFINED,
        NOTDEFINED
    }
}
