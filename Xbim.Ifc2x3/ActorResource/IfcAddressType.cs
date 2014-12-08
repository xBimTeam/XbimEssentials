#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAddressType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ActorResource
{
    /// <summary>
    ///   Type of the Address
    /// </summary>
    public enum IfcAddressType
    {
        /// <summary>
        ///   A office address
        /// </summary>
        Office,

        /// <summary>
        ///   A  site address
        /// </summary>
        Site,

        /// <summary>
        ///   A home address
        /// </summary>
        Home,

        /// <summary>
        ///   A postal distribution point address
        /// </summary>
        DistributionPoint,

        /// <summary>
        ///   A user defined address type to be provided
        /// </summary>
        UserDefined,
    }
}