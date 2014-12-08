#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceTextureEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    public enum IfcSurfaceTextureEnum
    {
        /// <summary>
        ///   Shows the amplitude of the microstructure of a surface. Example: the waves on a watersurface.
        /// </summary>
        BUMP,

        /// <summary>
        ///   Shows where a transparent surface is less transparent.
        /// </summary>
        OPACITY,

        /// <summary>
        ///   Shows the extent of reflection on a reflecting surface.
        /// </summary>
        REFLECTION,

        /// <summary>
        ///   Shows the map with self illumination, white parts have maximum illumination, black part none.
        /// </summary>
        SELFILLUMINATION,

        /// <summary>
        ///   Shows where a surface is more or less 'shiny'.
        /// </summary>
        SHININESS,

        /// <summary>
        ///   Shows the specular highlights are on a surface.
        /// </summary>
        SPECULAR,

        /// <summary>
        ///   Shows for each pixel an own color value.
        /// </summary>
        TEXTURE,

        /// <summary>
        ///   Shows where a transparent surface is more transparent. The opposite of opacity map.
        /// </summary>
        TRANSPARENCYMAP,
        NOTDEFINED
    }
}