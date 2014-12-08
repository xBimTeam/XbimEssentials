#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IFace.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;


#endregion

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IFace
    {
        IEnumerable<IBoundary> Boundaries { get; }
        IVector3D Normal { get; }
        bool HasHoles { get; }
        bool IsPolygonal { get; }
    }
}