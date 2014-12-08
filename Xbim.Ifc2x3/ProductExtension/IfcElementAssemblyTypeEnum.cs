#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementAssemblyTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An enumeration defining the basic configuration types for element assemblies.
    /// </summary>
    public enum IfcElementAssemblyTypeEnum
    {
        ACCESSORY_ASSEMBLY,
        ARCH,
        BEAM_GRID,
        BRACED_FRAME,
        GIRDER,
        REINFORCEMENT_UNIT,
        RIGID_FRAME,
        SLAB_FIELD,
        TRUSS,
        USERDEFINED,
        NOTDEFINED
    }
}