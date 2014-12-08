#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcObjectType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This enumeration defines the applicable object categories (i.e. the subtypes at the 2nd level of the IFC inheritance tree) . 
    ///   Attached to an object, it indicates to which subtype of IfcObject the entity referencing it would otherwise comply with.
    /// </summary>
    public enum IfcObjectType
    {
        Product,
        Process,
        Control,
        Resource,
        Actor,
        Group,
        Project,
        NotDefined
    }
}