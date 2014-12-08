#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpace.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   A space represents an area or volume bounded actually or theoretically. Spaces are areas or volumes that provide for certain functions within a building.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A space represents an area or volume bounded actually or theoretically. Spaces are areas or volumes that provide for certain functions within a building.
    ///   A space is (if specified) associated to a building storey (or in case of exterior spaces to a site). A space may span over several connected spaces. Therefore a space group provides for a collection of spaces included in a storey. A space can also be decomposed in parts, where each part defines a partial space. This is defined by the composition type attribute of the supertype IfcSpatialStructureElement which is interpreted as follow:
    ///   COMPLEX = space group 
    ///   ELEMENT = space 
    ///   PARTIAL = partial space 
    ///   The following guidelines should apply for using the Name, Description, LongName and ObjectType attributes. 
    ///   Name holds the unique name (or space number) from the plan. 
    ///   Description holds any additional information field the user may have specified, there are no further recommendations. 
    ///   LongName holds the full name of the space, it is often used in addition to the Name, if a number is assigned to the room, then the descriptive name is exchanged as LongName. 
    ///   ObjectType holds the space type, i.e. usually the functional category of the space . 
    ///   HISTORY New Entity in IFC Release 1.0 
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcSpace are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcSpace are part of this IFC release:
    ///   Pset_SpaceCommon: common property set for all types of spaces 
    ///   Pset_SpaceParking: specific property set for only those spaces that are used to define parking spaces by ObjectType = 'Parking' 
    ///   Pset_SpaceParkingAisle: specific property set for only those spaces that are used to define parking aisle by ObjectType = 'ParkingAisle' 
    ///   Pset_SpaceFireSafetyRequirements: common property set for all types of spaces to capture the fire safety requirements 
    ///   Pset_SpaceLightingRequirements: common property set for all types of spaces to capture the lighting requirements 
    ///   Pset_SpaceOccupancyRequirements: common property set for all types of spaces to capture the occupancy requirements 
    ///   Pset_SpaceThermalRequirements: common property set for all types of spaces to capture the thermal requirements 
    ///   Pset_SpaceThermalDesign: common property set for all all types of spaces to capture building service design values 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcSpace are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   AverageHeight Floor Height (without flooring) to Ceiling height (without suspended ceiling) for this space (measured from top of slab of this space to the bottom of slab of space above); the average shall be taken if room shape is not prismatic.  IfcQuantityLength  
    ///   AverageGrossHeight Floor Height to Floor Height for this space (measured from top of slab of this space to top of slab of space above); the average shall be taken if room shape is not prismatic.  IfcQuantityLength 
    ///   AverageClearHeight Clear Height between floor level (including finish) and ceiling level (including finish and sub construction) of this space; the average shall be taken if room shape is not prismatic. IfcQuantityLength 
    ///   GrossPerimeter Calculated gross perimeter at the floor level of this space. It all sides of the space, including those parts of the perimeter that are created by virtual boundaries and openings. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   NetPerimeter Calculated net perimeter at the floor level of this space. It normally excludes those parts of the perimeter that are created by by virtual boundaries and openings. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityLength 
    ///   GrossFloorArea Calculated sum of all floor areas covered by the space. It normally includes the area covered by elements inside the space (columns, inner walls, etc.). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetFloorArea Calculated sum of all usable floor areas covered by the space. It normally excludes the area covered by elements inside the space (columns, inner walls, etc.), floor openings, or other protruding elements. Special rules apply for areas that have a low headroom. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossCeilingArea Calculated sum of all ceiling areas of the space. It normally includes the area covered by elements inside the space (columns, inner walls, etc.). The ceiling area is the real (and not the projected) area (e.g. in case of sloped ceilings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetCeilingArea Calculated sum of all ceiling areas covered by the space. It normally excludes the area covered by elements inside the space (columns, inner walls, etc.) or by ceiling openings. The ceiling area is the real (and not the projected) area (e.g. in case of sloped ceilings). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossWallArea Calculated sum of all wall areas bounded by the space. It normally includes the area covered by elements inside the wall area (doors, windows, other openings, etc.). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetWallArea Calculated sum of all wall areas bounded by the space. It normally excludes the area covered by elements inside the wall area (doors, windows, other openings, etc.). Special rules apply for areas that have a low headroom. The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   GrossVolume Calculated gross volume of all areas enclosed by the space (normally including the volume of construction elements inside the space). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Calculated net volume of all areas enclosed by the space (normally excluding the volume of construction elements inside the space). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   Spatial Structure Use Definition
    ///   The IfcSpace is used to build the spatial structure of a building (that serves as the primary project breakdown and is required to be hierarchical). The spatial structure elements are linked together by using the objectified relationship IfcRelAggregates. The IfcSpace references them by its inverse relationships:
    ///   IfcSpace.Decomposes -- referencing (IfcSite || IfcBuildingStorey || IfcSpace) by IfcRelAggregates.RelatingObject, If it refers to another instance of IfcSpace, the referenced IfcSpace needs to have a different and higher CompositionType, i.e. COMPLEX (if the other IfcSpace has ELEMENT), or ELEMENT (if the other IfcSpace has PARTIAL). 
    ///   IfcSpace.IsDecomposedBy -- referencing (IfcSpace) by IfcRelAggregates.RelatedObjects. If it refers to another instance of IfcSpace, the referenced IfcSpace needs to have a different and lower CompositionType, i.e. ELEMENT (if the other IfcSpace has COMPLEX), or PARTIAL (if the other IfcSpace has ELEMENT). 
    ///   If there are building elements and/or other elements directly related to the IfcSpace (like most furniture and distribution elements), they are associated with the IfcSpace by using the objectified relationship IfcRelContainedInSpatialStructure. The IfcSpace references them by its inverse relationship:
    ///   IfcSpace.ContainsElements -- referencing any subtype of IfcProduct (with the exception of other spatial structure element) by IfcRelContainedInSpatialStructure.RelatedElements. 
    ///   Attribute Use Definition:  The following figure describes the heights and elevations of the IfcSpace.
    ///  
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcSpace is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   NOTE1 If the surrounding instances of IfcRelSpaceBoundary define a complete geometric representation of a particular representation view for that space, then this view shall be omitted from the multiple representations of IfcSpace.
    ///   NOTE2 In cases of inconsistency between the geometric representation of the IfcSpace and its surrounding IfcRelSpaceBoundary, the geometric representation of the space should take priority over the geometric representation of the surrounding space boundaries. 
    ///   Local Placement
    ///   The local placement for IfcSpace is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point to the local placement of the IfcSpatialStructureElement of type "IfcBuildingStorey", if relative placement is used, or of type "IfcSpace" (e.g. to position a space relative to a space group, or a partial space to a space). 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of a 2D 'FootPrint' representation of type 'Curve2D' or 'GeometricCurveSet' and a 3D 'Body' representation of type 'SweptSolid, 'Clipping' and 'Brep' is supported.
    ///   'Foot' Print representation
    ///   The 2D geometric representation of IfcSpace is defined using the 'Curve2D' or 'GeometricCurveSet' geometry. The following attribute values should be inserted
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'FootPrint'. 
    ///   IfcShapeRepresentation.RepresentationType = 'Curve2D' or 'GeometricCurveSet' . 
    ///   The following constraints apply to the 2D representation: 
    ///   Profile: IfcBoundedCurve is required, using IfcPolyline for faceted space contours or IfcCompositeCurve for space contours with arc segments. For spaces with inner boundaries, a set of IfcBoundedCurve's is used, that should be grouped into an IfcGeometricCurveSet. 
    ///   Two-dimensional bounded curve representing the foot print of IfcSpace.
    ///  
    ///   'Swept Solid' representation
    ///   The standard geometric representation of IfcSpace is defined using the swept area solid geometry. The following attribute values should be inserted
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Body'. 
    ///   IfcShapeRepresentation.RepresentationType = 'SweptSolid'. 
    ///   The following constraints apply to the standard representation: 
    ///   Solid: IfcExtrudedAreaSolid is required, 
    ///   Profile: IfcArbitraryClosedProfileDef is required, IfcArbitraryProfileDefWithVoids shall be supported. 
    ///   Extrusion: The extrusion direction shall be vertically, i.e., along the positive Z Axis of the co-ordinate system of the containing spatial structure element. 
    ///   Extrusion of an arbitrary profile definition with voids into the swept area solid of IfcSpace.
    ///  
    ///   'Clipping' representation
    ///   The advanced geometric representation of IfcSpace is defined using the swept area solid geometry that can be subjected to a Boolean expression. The following attribute values should be inserted. 
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Body'. 
    ///   IfcShapeRepresentation.RepresentationType = 'Clipping'. 
    ///   The following additional constraints apply to the advanced representation: 
    ///   Solid: see standard geometric representation, 
    ///   Profile: see standard geometric representation, 
    ///   Extrusion: see standard geometric representation, 
    ///   Boolean result: The difference operation with the second operand being of type IfcHalfSpaceSolid (or one of its subtypes) shall be supported. 
    ///   Extrusion of an arbitrary profile definition into the swept area solid. The solid and an half space solid are operands of the Boolean result of IfcSpace. 
    ///   'Brep' representation
    ///   The fallback advanced geometric representation of IfcSpace is defined using the Brep solid geometry. may be represented as a single or multiple instances of IfcFacetedBrep or IfcFacetedBrepWithVoids. The Brep representation allows for the representation of complex element shape. The following attribute values for the IfcShapeRepresentation holding this geometric representation shall be used: 
    ///   IfcShapeRepresentation.RepresentationIdentifier : 'Body' 
    ///   IfcShapeRepresentation.RepresentationType : 'Brep'
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcSpace : IfcSpatialStructureElement
    {
        #region Fields and Events

        private IfcInternalOrExternalEnum _interiorOrExteriorSpace = IfcInternalOrExternalEnum.NOTDEFINED;
        private IfcLengthMeasure? _elevationWithFlooring;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Defines, whether the Space is interior (Internal), or exterior (External), i.e. part of the outer space.
        /// </summary>

        [IfcAttribute(10, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcInternalOrExternalEnum InteriorOrExteriorSpace
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _interiorOrExteriorSpace;
            }
            set
            {
                this.SetModelValue(this, ref _interiorOrExteriorSpace, value, v => InteriorOrExteriorSpace = v,
                                           "InteriorOrExteriorSpace");
            }
        }


        /// <summary>
        ///   Optional. Level of flooring of this space; the average shall be taken, if the space ground surface is sloping or if there are level differences within this space.
        /// </summary>

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ElevationWithFlooring
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elevationWithFlooring;
            }
            set
            {
                this.SetModelValue(this, ref _elevationWithFlooring, value, v => ElevationWithFlooring = v,
                                           "ElevationWithFlooring");
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
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _interiorOrExteriorSpace =
                        (IfcInternalOrExternalEnum) Enum.Parse(typeof (IfcInternalOrExternalEnum), value.EnumVal, true);
                    break;
                case 10:
                    _elevationWithFlooring = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse. Reference to IfcCovering by virtue of the objectified relationship IfcRelCoversSpaces. 
        ///   It defines the concept of a space having coverings assigned. Those coverings may represent different flooring, or tiling areas.
        /// </summary>
        /// <remarks>
        ///   NOTE  Coverings are often managed by the space, and not by the building element, which they cover.
        ///   IFC2x Edition3 CHANGE  New inverse relationship. Upward compatibility for file based exchange is guaranteed.
        /// </remarks>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelCoversSpaces> HasCoverings
        {
            get { return ModelOf.Instances.Where<IfcRelCoversSpaces>(r => r.RelatedSpace == this); }
        }

        /// <summary>
        ///   Inverse. Reference to Set of Space Boundaries that defines the physical or virtual delimitation of that Space.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelSpaceBoundary> BoundedBy
        {
            get { return ModelOf.Instances.Where<IfcRelSpaceBoundary>(r => r.RelatingSpace == this); }
        }

        #endregion
    }
}
