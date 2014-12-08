#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRailing.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   The railing is a frame assembly adjacent to human circulation spaces and at some space boundaries where it is used in lieu of walls or to compliment walls.
    /// </summary>
    /// <remarks>
    ///   Definition of IAI: The railing is a frame assembly adjacent to human circulation spaces and at some space boundaries where it is used in lieu of walls or to compliment walls. Designed to aid humans, either as an optional physical support, or to prevent injury by falling. A list of references to accessory/mounting hardware for this railing might be given by including these assessories (IfcDiscreteAssessory) through the objectified relationship IfcRelAggregates. 
    ///   HISTORY New Entity in IFC Release 2.0 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcRailing are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcRailing are part of this IFC release:
    ///   Pset_RailingCommon: common property set for all railing occurrences 
    ///   Geometry Use Definitions 
    ///   The geometric representation of IfcRailing is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Included are: 
    ///   Local placement
    ///   The local placement for IfcRailing is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level 
    ///   If the IfcRailing, however, is used by an IfcStair or IfcRamp, and this container class defines its own local placement, then the PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the aggregate. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representation 
    ///   Currently the use of 'BoundingBox', 'SurfaceModel', 'Brep' and 'MappedRepresentation' representations of IfcRailing are supported. The conventions to use these representations are given at the level of the supertype, IfcBuildingElement.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRailing : IfcBuildingElement
    {
        #region Fields

        private IfcRailingTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(9, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcRailingTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

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
                    _predefinedType = (IfcRailingTypeEnum) Enum.Parse(typeof (IfcRailingTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}