#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCoilTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   Enumeration defining the typical types of coils.
    /// </summary>
    public enum IfcCoilTypeEnum
    {
        /// <summary>
        ///   Cooling coil using a refrigerant to cool the air stream directly
        /// </summary>
        DXCOOLINGCOIL,

        /// <summary>
        ///   Cooling coil using chilled water to cool the air stream.
        /// </summary>
        WATERCOOLINGCOIL,

        /// <summary>
        ///   Heating coil using steam as heating source.
        /// </summary>
        STEAMHEATINGCOIL,

        /// <summary>
        ///   Heating coil using hot water as a heating source.
        /// </summary>
        WATERHEATINGCOIL,

        /// <summary>
        ///   Heating coil using electricity as a heating source.
        /// </summary>
        ELECTRICHEATINGCOIL,

        /// <summary>
        ///   Heating coil using gas as a heating source.
        /// </summary>
        GASHEATINGCOIL,

        /// <summary>
        ///   User-defined coil type.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined coil type.
        /// </summary>
        NOTDEFINED
    }
}