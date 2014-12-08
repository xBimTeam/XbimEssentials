#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcChangeActionEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.UtilityResource
{
    /// <summary>
    ///   Enumeration identifying the type of change that might have occurred to the object during the last session 
    ///   (e.g., unchanged, added, deleted, etc.). 
    ///   This information is required in a partial model exchange scenario so that an application or model server 
    ///   will know how an object might have been affected by the previous application
    /// </summary>
    /// <remarks>
    ///   Consider Application A receives an IFC dataset, adds a new object and sets IfcChangeActionEnum to ADDED. Application B then receives this IFC dataset but doesn't do anything to the object added by Application A. Consequently, the object's IfcChangeActionEnum remains set at ADDED. Consequently, the intent is that an application only modifies the value of IfcChangeActionEnum when it does something to the object, with the further intent that a model server is responsible for clearing the IfcChangeActionEnum back to UNCHANGED when it is checked back into the repository.
    /// </remarks>
    public enum IfcChangeActionEnum
    {
        /// <summary>
        ///   Object has not been modified. This is the default state.
        /// </summary>
        NOCHANGE,

        /// <summary>
        ///   A modification to the object has been made by the user and application defined by the LastModifyingUser and LastModifyingApplication respectively.
        /// </summary>
        MODIFIED,

        /// <summary>
        ///   The object has been added by the user and application defined by the LastModifyingUser and LastModifyingApplication respectively.
        /// </summary>
        ADDED,

        /// <summary>
        ///   The object has been deleted by the user and application defined by the LastModifyingUser and LastModifyingApplication respectively.
        /// </summary>
        DELETED,

        /// <summary>
        ///   The object has been added and modified by the user and application defined by the LastModifyingUser and LastModifyingApplication respectively.
        /// </summary>
        /// <remarks>
        ///   Note that only the first four enumerations should be used. The MODIFIEDADDED and MODIFIEDDELETED are left for compatibility purposes but should not be used.
        /// </remarks>
        MODIFIEDADDED,

        /// <summary>
        ///   The object has been modified and deleted by the user and application defined by the LastModifyingUser and LastModifyingApplication respectively.
        /// </summary>
        /// <remarks>
        ///   Note that only the first four enumerations should be used. The MODIFIEDADDED and MODIFIEDDELETED are left for compatibility purposes but should not be used.
        /// </remarks>
        MODIFIEDDELETED,
    }
}