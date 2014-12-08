#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRoofTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic configuration of the roof in terms of the different roof shapes.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This enumeration defines the basic configuration of the roof in terms of the different roof shapes. 
    ///   Roofs which are subdivided into more than these basic shapes have to be defined by the geometry only. Also roofs with non-regular shapes (free form roof ) have to be defined by the geometry only. The type of such roofs is FREEFORM.
    ///   HISTORY New Enumeration in IFC Release 2x.
    /// </remarks>
    public enum IfcRoofTypeEnum
    {
        /// <summary>
        ///   A roof having no slope, or one with only a slight pitch so as to drain rainwater.
        /// </summary>
        FLAT_ROOF,

        /// <summary>
        ///   A roof having a single slope.
        /// </summary>
        SHED_ROOF,

        /// <summary>
        ///   A roof sloping downward in two parts from a central ridge, so as to form a gable at each end.
        /// </summary>
        GABLE_ROOF,

        /// <summary>
        ///   A roof having sloping ends and sides meeting at an inclined projecting angle.
        /// </summary>
        HIP_ROOF,

        /// <summary>
        ///   A roof having a hipped end truncating a gable.
        /// </summary>
        HIPPED_GABLE_ROOF,

        /// <summary>
        ///   A ridged roof divided on each side into a shallower slope above a steeper one.
        /// </summary>
        GAMBREL_ROOF,

        /// <summary>
        ///   A roof having on each side a steeper lower part and a shallower upper part.
        /// </summary>
        MANSARD_ROOF,

        /// <summary>
        ///   A roof or ceiling having a semicylindrical form.
        /// </summary>
        BARREL_ROOF,

        /// <summary>
        ///   A gable roof in the form of a broad Gothic arch, with gently sloping convex surfaces.
        /// </summary>
        RAINBOW_ROOF,

        /// <summary>
        ///   A roof having two slopes, each descending inward from the eaves.
        /// </summary>
        BUTTERFLY_ROOF,

        /// <summary>
        ///   A pyramidal hip roof.
        /// </summary>
        PAVILION_ROOF,

        /// <summary>
        ///   A hemispherical hip roof.
        /// </summary>
        DOME_ROOF,

        /// <summary>
        ///   Free form roof
        /// </summary>
        FREEFORM,

        /// <summary>
        ///   No specification given
        /// </summary>
        NOTDEFINED
    }
}