#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IPlacement3D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Ifc.GeometryResource;

#endregion

namespace Xbim.XbimExtensions
{
    /// <summary>
    ///   If the object supports placement by Axis2Placement3D, this returns the placement
    /// </summary>
    public interface IPlacement3D
    {
        IfcAxis2Placement3D Position { get; }
    }
}