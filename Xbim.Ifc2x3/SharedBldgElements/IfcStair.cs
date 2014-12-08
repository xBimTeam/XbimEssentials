#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStair.cs
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
    ///   Construction comprising a succession of horizontal stages (steps or landings) that make it possible to pass on foot to other levels.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Construction comprising a succession of horizontal stages (steps or landings) that make it possible to pass on foot to other levels.
    ///   Definition from IAI: A vertical passageway allowing occupants to walk (step) from one floor level to another floor level at a different elevation. It may include a landing as an intermediate floor slab. 
    ///   The stair is a container entity that aggregates all components of the stair, it represents. The aggregation is handled via the IfcRelAggregates relationship, relating an IfcStair with the related flights (IfcStairFlight) and landings (IfcSlab with type 'Landing').
    ///   HISTORY: New Entity in IFC Release 2.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcStair are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcStair are part of this IFC release:
    ///   Pset_StairCommon: common property set for all stair occurrences 
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcStair is given by the IfcProductDefinitionShape, allowing multiple geometric representation. Independent geometric representations should only be used when the IfcStair is not defined as an aggregate. If defined as an aggregate, the geometric representation is the sum of the representation of the components within the aggregate. 
    ///   Local placement
    ///   The local placement for IfcStair is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement that is used in the ContainedInStructure inverse attribute or to a referenced spatial structure element at a higher level. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   If the LocalPlacement is given for the IfcStair, then all components, which are aggregated to the stair should use this placement as their relative placement.
    ///   Geometric Representation
    ///   If the IfcStair has components (referenced by SELF\IfcObject.IsDecomposedBy) then no independent geometric representation shall defined for the IfcStair. The IfcStair is then geometrically represented by the geometric representation of its components. The components are accessed via SELF\IfcObject.IsDecomposedBy[1].RelatedObjects.
    ///   If the IfcStair has no components defined (empty set of SELF\IfcObject.IsDecomposedBy) then the IfcStair may be represented by an IfcShapeRepresentation with the RepresentationType = 'Brep'.
    ///   Illustration:
    ///   IfcStair defining only the local placement for all components. 
    ///   Formal Propositions:
    ///   WR1   :   Either the stair is not decomposed into its flights and landings (the stair can have independent geometry), or the geometry shall not be given at IfcStair directly.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcStair : IfcBuildingElement
    {
        #region Fields

        private IfcStairTypeEnum _shapeType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Predefined shape types for a stair that are specified in an Enum.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcStairTypeEnum ShapeType
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
                    _shapeType = (IfcStairTypeEnum) Enum.Parse(typeof (IfcStairTypeEnum), value.EnumVal, true);
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
                    "WR1 Stair: Either the stair is not decomposed into its flights and landings (the stair can have independent geometry), or the geometry shall not be given at Stair directly.\n";
            return baseErr;
        }

        #endregion
    }
}