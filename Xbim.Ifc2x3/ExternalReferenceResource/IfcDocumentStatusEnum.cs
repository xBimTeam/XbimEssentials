#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDocumentStatusEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ExternalReferenceResource
{
    /// <summary>
    ///   Enables selection of the status of document information from a list of choices.
    /// </summary>
    public enum IfcDocumentStatusEnum
    {
        DRAFT,
        FINALDRAFT,
        FINAL,
        REVISION,
        NOTDEFINED
    }
}