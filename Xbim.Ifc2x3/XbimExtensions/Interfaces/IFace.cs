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
using Xbim.Ifc.GeometryResource;

#endregion

namespace Xbim.XbimExtensions
{
    public interface IFace
    {
        IEnumerable<IBoundary> Boundaries { get; }
        IfcDirection Normal { get; }
        bool HasHoles { get; }
        bool IsPolygonal { get; }
    }
}