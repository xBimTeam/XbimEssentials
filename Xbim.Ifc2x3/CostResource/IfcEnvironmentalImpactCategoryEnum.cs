#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEnvironmentalImpactCategoryEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   The IfcEnvironmentalImpactCategoryEnum defines the range of categories into which an environmental impact can be broken down and from which the category required may be selected.
    /// </summary>
    public enum IfcEnvironmentalImpactCategoryEnum
    {
        COMBINEDVALUE,

        /// <summary>
        ///   An environmental impact value is deduced from values in more than one category as a result of using the applied value relationship.
        /// </summary>
        DISPOSAL,

        /// <summary>
        ///   An environmental impact value due to disposal.
        /// </summary>
        EXTRACTION,

        /// <summary>
        ///   An environmental impact value due to extraction.
        /// </summary>
        INSTALLATION,

        /// <summary>
        ///   An environmental impact value due to installation.
        /// </summary>
        MANUFACTURE,

        /// <summary>
        ///   An environmental impact value due to manufacture and manufacturing processes.
        /// </summary>
        TRANSPORTATION,

        /// <summary>
        ///   An environmental impact value due to transportation.
        /// </summary>
        USERDEFINED,
        NOTDEFINED
    }
}