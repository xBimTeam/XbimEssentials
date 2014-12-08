#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSequenceEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This enumeration defines the different ways, in which a time lag is applied to a sequence between two processes.
    /// </summary>
    public enum IfcSequenceEnum
    {
        START_START,
        START_FINISH,
        FINISH_START,
        FINISH_FINISH,
        NOTDEFINED
    }
}