#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IVertex3D.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IVertex3D
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        // object Vertex { get; }
    }
}