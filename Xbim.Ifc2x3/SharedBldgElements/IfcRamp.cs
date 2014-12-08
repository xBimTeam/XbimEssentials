#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRamp.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   Inclined way or floor joining two surfaces at different levels.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Inclined way or floor joining two surfaces at different levels.
    ///   Definition of IAI: An vertical passageway which provides a human circulation link between one floor level and another floor level at a different elevation. It may include a landing as an intermediate floor slab. A ramp normally does not include steps (stepped ramps are out of scope for this IFC Release). 
    ///   The ramp is a container entity that aggregates all components of the ramp, it represents. The aggregation is handled via the IfcRelAggregates relationship, relating an IfcRamp with the related flights (IfcRampFlight) and landings (IfcSlab with type 'Landing').
    ///   HISTORY New Entity in IFC Release 2.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcRamp are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcRamp are part of this IFC release:
    ///   Pset_RampCommon: common property set for all ramp occurrences 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcRamp is given by the IfcProductDefinitionShape, allowing multiple geometric representations. Independent geometric representations should only be used when the IfcRamp is not defined as an aggregate. If defined as an aggregate, the geometric representation is the sum of the representation of the components within the aggregate. 
    ///   Local placement
    ///   The local placement for IfcRamp is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   If the LocalPlacement is given for the IfcRamp, then all components, which are aggregated to the ramp should use this placement as their relative placement.
    ///   Geometric Representations
    ///   If the IfcRamp has components (referenced by SELF\IfcProduct.IsDecomposedBy) then no independent geometric representation shall be defined for the IfcRamp. The IfcRamp is then geometrically represented by the geometric representation of its components. The components are accessed via SELF\IfcProduct.IsDecomposedBy[1].RelatedObjects.
    ///   If the IfcRamp has no components defined (empty set of SELF\IfcProduct.IsDecomposedBy) then the IfcRamp may be represented by an IfcShapeRepresentation with the RepresentationType = 'Brep'.
    ///   Illustration:
    ///   IfcRamp defining only the local
    ///   Formal Propositions:
    ///   WR1   :   Either the ramp is not decomposed into its flights and landings (the ramp can have independent geometry), or the geometry shall not be given at IfcRamp directly.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRamp : IfcBuildingElement
    {
        #region Fields 

        private IfcRampTypeEnum _shapeType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Predefined shape types for a stair that are specified in an Enum.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcRampTypeEnum ShapeType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _shapeType;
            }
            set { this.SetModelValue(this, ref _shapeType, value, v => ShapeType = v, "ShapeType"); }
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
                    _shapeType = (IfcRampTypeEnum) Enum.Parse(typeof (IfcRampTypeEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (IsDecomposedBy.Count() > 0 && Representation != null)
                baseErr +=
                    "WR1 Ramp: Either the ramp is not decomposed into its flights and landings (the stair can have independent geometry), or the geometry shall not be given at Stair directly.\n";
            return baseErr;
        }

        #endregion
    }
}