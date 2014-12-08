#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStateEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    /// <summary>
    ///   Enumeration identifying the state or accessibility of the object (e.g., read/write, locked, etc.). 
    ///   This concept was initially introduced in IFC 2.0 as IfcModifiedFlag of type BINARY(3) FIXED and has been modified in R2x to an enumeration. 
    ///   It was initially introduced as a first step towards providing facilities for partial model exchange from a server as requested by the IFC implementers. 
    ///   It is intended for use primarily by a model server so that an application can identify the state of the object.
    /// </summary>
    public enum IfcStateEnum
    {
        /// <summary>
        ///   Object is in a Read-Write state. It may be modified by an application
        /// </summary>
        READWRITE,

        /// <summary>
        ///   Object is in a Read-Only state. It may be viewed but not modified by an application
        /// </summary>
        READONLY,

        /// <summary>
        ///   Object is in a Locked state. It may not be accessed by an application
        /// </summary>
        LOCKED,

        /// <summary>
        ///   Object is in a Read-Write-Locked state. It may not be accessed by an application
        /// </summary>
        READWRITELOCKED,

        /// <summary>
        ///   Object is in a Read-Only-Locked state. It may not be accessed by an application
        /// </summary>
        READONLYLOCKED,
    }
}