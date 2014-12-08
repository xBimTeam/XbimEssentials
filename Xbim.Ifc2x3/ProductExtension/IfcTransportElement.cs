#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTransportElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Generalization of all transport related objects that move people, animals or goods within a building or building complex.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Generalization of all transport related objects that move people, animals or goods within a building or building complex. The IfcTransportElement defines the occurrence of a covering type, that (if given) is expressed by the IfcTransportElementType.
    ///   EXAMPLE: Transportation elements include elevator (lift), escalator, moving walkway, etc.
    ///   IFC2x PLATFORM CHANGE: The attribute OperationType is now optional and should only be inserted when there is no type information, given by IfcTransportElementType, is assigned to the IfcTransportElement occurrence by IfcRelDefinesByType.
    ///   HISTORY New entity in IFC Release 2x.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcTransportElement are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcTransportElement are part of this IFC release:
    ///   Pset_TransportElementCommon: common property set for all transport element occurrences 
    ///   Pset_TransportElementElevator: specific property set for all occurrences of transport elements with the PredefinedType: ELEVATOR 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcTransportElement is given by the IfcProductDefinitionShape, allowing multiple geometric representation. 
    ///   Local Placement
    ///   The local placement for IfcTransportElement is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement , which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   SurfaceModel Representation
    ///   Any IfcTransportElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple surface models, based on either shell or face based models. It is ensured by assigning the value 'SurfaceModel' to the RepresentationType attribute of IfcShapeRepresentation. In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   Brep Representation
    ///   Any IfcTransportElement (so far no further constraints are defined at the level of its subtypes) may be represented as a single or multiple Boundary Representation elements (which are restricted to faceted Brep with or without voids). The Brep representation allows for the representation of complex element shape. It is ensured by assigning the value 'Brep' to the RepresentationType attribute of IfcShapeRepresentation. In some cases it may be useful to also expose a simple representation as a bounding box representation of the same complex shape.
    ///   MappedRepresentation
    ///   The new mapped item, IfcMappedItem, should be used if appropriate as it allows for reusing the geometry definition of the transport element type at occurrences of the same equipement type. In this case the IfcShapeRepresentation.RepresentationType = MappedRepresentation is used.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcTransportElement : IfcElement
    {
        #region Fields

        private IfcTransportElementTypeEnum? _operationType;
        private IfcMassMeasure? _capacityByWeight;
        private IfcCountMeasure? _capacityByNumber;

        #endregion

        /// <summary>
        ///   Optional. Predefined type for transport element.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcTransportElementTypeEnum? OperationType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operationType;
            }
            set { this.SetModelValue(this, ref _operationType, value, v => OperationType = v, "OperationType"); }
        }

        /// <summary>
        ///   Optional. Capacity of the transport element measured by weight.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcMassMeasure? CapacityByWeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _capacityByWeight;
            }
            set
            {
                this.SetModelValue(this, ref _capacityByWeight, value, v => CapacityByWeight = v,
                                           "CapacityByWeight");
            }
        }

        /// <summary>
        ///   Optional. Capacity of the transportation element measured in numbers of person.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcCountMeasure? CapacityByNumber
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _capacityByNumber;
            }
            set
            {
                this.SetModelValue(this, ref _capacityByNumber, value, v => CapacityByNumber = v,
                                           "CapacityByNumber");
            }
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
                    _operationType =
                        (IfcTransportElementTypeEnum)
                        Enum.Parse(typeof (IfcTransportElementTypeEnum), value.EnumVal, true);
                    break;
                case 9:
                    _capacityByWeight = value.RealVal;
                    break;
                case 10:
                    _capacityByNumber = value.NumberVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}