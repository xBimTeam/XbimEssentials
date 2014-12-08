#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTrimmingPreference.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   This type is used to describe the preferred way of trimming a parametric curve where the trimming is multiply defined.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: This type is used to describe the preferred way of trimming a parametric curve where the trimming is multiply defined. 
    ///   NOTE Corresponding STEP type: trimming_preference, please refer to ISO/IS 10303-42:1994, p. 18 for the final definition of the formal standard. 
    ///   HISTORY New Type in IFC Release 1.0
    /// </remarks>
    public enum IfcTrimmingPreference
    {
        /// <summary>
        ///   Indicates that trimming by Cartesian point is preferred.
        /// </summary>
        CARTESIAN,

        /// <summary>
        ///   Indicates the preference for the parameter value.
        /// </summary>
        PARAMETER,

        /// <summary>
        ///   Indicates that no preference is communicated.
        /// </summary>
        UNSPECIFIED
    }
}