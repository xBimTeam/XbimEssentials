#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowStyleOperationEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic configuration of the window type in terms of the number of window panels 
    ///   and the subdivision of the total window. The window configurations are given for windows with one, 
    ///   two or three panels (including fixed panels).
    ///   Windows which are subdivided into more than three panels have to be defined by the geometry only. 
    ///   The type of such windows is USERDEFINED
    /// </summary>
    public enum IfcWindowStyleOperationEnum
    {
        /// <summary>
        ///   Window with one panel.
        /// </summary>
        SINGLE_PANEL,

        /// <summary>
        ///   Window with two panels. The configuration of the panels is vertically.
        /// </summary>
        DOUBLE_PANEL_VERTICAL,

        /// <summary>
        ///   Window with two panels. The configuration of the panels is horizontally.
        /// </summary>
        DOUBLE_PANEL_HORIZONTAL,

        /// <summary>
        ///   Window with three panels. The configuration of the panels is vertically.
        /// </summary>
        TRIPLE_PANEL_VERTICAL,

        /// <summary>
        ///   Window with three panels. The configuration of two panels is vertically and the third one is horizontally at the bottom.
        /// </summary>
        TRIPLE_PANEL_BOTTOM,

        /// <summary>
        ///   Window with three panels. The configuration of two panels is vertically and the third one is horizontally at the top.
        /// </summary>
        TRIPLE_PANEL_TOP,

        /// <summary>
        ///   Window with three panels. The configuration of two panels is horizontally and the third one is vertically at the left hand side.
        /// </summary>
        TRIPLE_PANEL_LEFT,

        /// <summary>
        ///   Window with three panels. The configuration of two panels is horizontally and the third one is vertically at the right hand side.
        /// </summary>
        TRIPLE_PANEL_RIGHT,

        /// <summary>
        ///   Window with three panels. The configuration of the panels is horizontally.
        /// </summary>
        TRIPLE_PANEL_HORIZONTAL,

        /// <summary>
        ///   user defined operation type
        /// </summary>
        USERDEFINED,
        NOTDEFINED
    }
}