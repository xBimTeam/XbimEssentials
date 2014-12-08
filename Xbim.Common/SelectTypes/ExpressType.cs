#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ExpressType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;

#endregion

namespace Xbim.XbimExtensions.SelectTypes
{
    public interface ExpressType : Xbim.XbimExtensions.Interfaces.IPersistIfc
    {
        string ToPart21 { get; }
        Type UnderlyingSystemType { get; }
        object Value { get; }
    }
}