#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowPanelOperationEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic ways to describe how window panels operate.
    /// </summary>
    public enum IfcWindowPanelOperationEnum
    {
        /// <summary>
        ///   panel that opens to the right when viewed from the outside
        /// </summary>
        SIDEHUNGRIGHTHAND,

        /// <summary>
        ///   panel that opens to the left when viewed from the outside
        /// </summary>
        SIDEHUNGLEFTHAND,

        /// <summary>
        ///   panel that opens to the right and is bottom hung
        /// </summary>
        TILTANDTURNRIGHTHAND,

        /// <summary>
        ///   panel that opens to the left and is bottom hung
        /// </summary>
        TILTANDTURNLEFTHAND,

        /// <summary>
        ///   panel is top hung
        /// </summary>
        TOPHUNG,

        /// <summary>
        ///   panel is bottom hung
        /// </summary>
        BOTTOMHUNG,

        /// <summary>
        ///   panel is swinging horizontally (hinges are in the middle)
        /// </summary>
        PIVOTHORIZONTAL,

        /// <summary>
        ///   panel is swinging vertically (hinges are in the middle)
        /// </summary>
        PIVOTVERTICAL,

        /// <summary>
        ///   panel is sliding horizontally
        /// </summary>
        SLIDINGHORIZONTAL,

        /// <summary>
        ///   panel is sliding vertically
        /// </summary>
        SLIDINGVERTICAL,

        /// <summary>
        ///   panel is removable
        /// </summary>
        REMOVABLECASEMENT,

        /// <summary>
        ///   panel is fixed
        /// </summary>
        FIXEDCASEMENT,

        /// <summary>
        ///   user defined operation type
        /// </summary>
        OTHEROPERATION,
        NOTDEFINED
    }
}