#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAssemblyPlaceEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Enumeration defining where the assembly is intended to take place, either in a factory or on the building site.
    /// </summary>
    public enum IfcAssemblyPlaceEnum
    {
        SITE,
        FACTORY,
        NOTDEFINED
    }
}