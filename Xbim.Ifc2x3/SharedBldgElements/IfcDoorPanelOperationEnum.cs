#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDoorPanelOperationEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic ways how individual door panels operate.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This enumeration defines the basic ways how individual door panels operate. 
    ///   HISTORY New Enumeration in IFC Release 2.0.
    ///   Swinging  
    ///   DoubleActing  
    ///   Sliding  
    ///   Folding  
    ///   Revolving  
    ///   Rollingup  
    ///   UserDefined   
    ///   NotDefined   
    ///   The opening direction of the door panels is given by the local placement of the IfcDoor. The positive y-axis determines the direction as shown in the figure.
    ///  
    ///   NOTE
    ///   Figures (symbolic representation) depend on the national building code 
    ///   These figures are only shown as illustrations 
    ///   EXPRESS specification:
    /// </remarks>
    public enum IfcDoorPanelOperationEnum
    {
        SWINGING,
        DOUBLE_ACTING,
        SLIDING,
        FOLDING,
        REVOLVING,
        ROLLINGUP,
        USERDEFINED,
        NOTDEFINED
    }
}