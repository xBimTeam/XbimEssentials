#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementAssembly.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   A container class that represents complex element assemblies aggregated from several elements, such as discrete elements, building elements, or other elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A container class that represents complex element assemblies aggregated from several elements, such as discrete elements, building elements, or other elements.
    ///   EXAMPLE: Steel construction assemblies, such as trusses and different kinds of frames, can be represented by the IfcElementAssembly entity. Other examples include slab fields aggregated from a number of precast concrete slabs or reinforcement units made from several reinforcement bars. 
    ///   HISTORY: New Entity for Release IFC2x Edition 2.
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcElementAssembly is given by the IfcProductDefinitionShape, allowing multiple geometric representations. 
    ///   Local Placement
    ///   The local placement for IfcElementAssembly is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcBuildingElement, which is used in the Decomposes inverse attribute, i.e. the local placement is defined relative to the local placement of the building element in which the assembly is contained. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   The geometry of an IfcElementAssembly is generally formed from its components, in which case it does not need to have an explicit geometric representation. In some cases it may be useful to also expose a simple explicit representation as a bounding box representation of the complex composed shape independently.
    ///   Formal Propositions:
    ///   WR1   :   The attribute ObjectType shall be given, if the predefined type is set to USERDEFINED.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcElementAssembly : IfcElement
    {
       
        #region Fields
        private IfcAssemblyPlaceEnum? _assemblyPlace;
        private IfcElementAssemblyTypeEnum _predefinedType;
        #endregion

        #region Ifc Properties
        /// <summary>
        /// The nominal diameter describing the cross-section size of the fastener.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcAssemblyPlaceEnum? AssemblyPlace
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _assemblyPlace;
            }
            set { this.SetModelValue(this, ref _assemblyPlace, value, v => AssemblyPlace = v, "AssemblyPlace"); }
        }

        /// <summary>
        /// The nominal length describing the longitudinal dimensions of the fastener.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcElementAssemblyTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }
        #endregion

        #region IfcParse

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _assemblyPlace = (IfcAssemblyPlaceEnum)
                        Enum.Parse(typeof(IfcAssemblyPlaceEnum), value.EnumVal, true);
                    break;
                case 9:
                    _predefinedType = (IfcElementAssemblyTypeEnum)
                        Enum.Parse(typeof(IfcElementAssemblyTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
        #endregion
    }
}