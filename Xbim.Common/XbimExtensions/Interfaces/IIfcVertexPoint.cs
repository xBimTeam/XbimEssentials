#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IIfcVertexPoint.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IIfcVertexPoint : IIfcVertex
    {
        IVertex3D VertexGeometry { get; }
    }
}