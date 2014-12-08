
namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcProjectOrderRecordTypeEnum is a designation of the type of event being recorded.
    /// </summary>
    public enum IfcProjectOrderRecordTypeEnum
    {
        /// <summary>
        /// A record of instructions to bring about a change to a construction or installation.
        /// </summary>
        CHANGE,
        /// <summary>
        /// A record of instructions to carry out maintenance work on one or more assets or components.
        /// </summary>
        MAINTENANCE,
        /// <summary>
        /// A record of instructions to move actors and/or artefacts.
        /// </summary>
        MOVE,
        /// <summary>
        /// A record of instructions to purchase goods or services.
        /// </summary>
        PURCHASE,
        /// <summary>
        /// A record of instructions to carry out work generally.
        /// </summary>
        WORK,
        USERDEFINED,
        NOTDEFINED
    }
}
