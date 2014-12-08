#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingStorey.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The building storey has an elevation and typically represents a (nearly) horizontal aggregation of spaces that are vertically bound.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The building storey has an elevation and typically represents a (nearly) horizontal aggregation of spaces that are vertically bound. 
    ///   A storey is (if specified) associated to a building. A storey may span over several connected storeys. Therefore storey complex provides for a collection of storeys included in a building. A storey can also be decomposed in (horizontical) parts, where each part defines a partial storey. This is defined by the composition type attribute of the supertype IfcSpatialStructureElements which is interpreted as follow:
    ///   COMPLEX = building storey complex 
    ///   ELEMENT = building storey 
    ///   PARTIAL = partial building storey 
    ///   EXAMPLE: In split level houses, a storey is splitted into two or more partial storeys, each with a different elevation. It can be handled by defining a storey, which includes two or more partial storeys with the individual elevations.
    ///   HISTORY New entity in IFC Release 1.0 
    ///   Property Set Use Definition
    ///   The property sets relating to the IfcBuildingStorey are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcBuildingStorey are part of this IFC release:
    ///   Pset_BuildingStoreyCommon: common property set for all types of building stories 
    ///   Quantity Use Definition
    ///   The quantities relating to the building storey are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   TotalHeight Calculated height of this storey, from the bottom surface of the floor, to the bottom surface of the floor or roof above.  IfcQuantityLength  
    ///   GrossFloorArea Calculated sum of all areas covered by the building storey (as horizontal projections and normally including the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetFloorArea Calculated sum of all usable areas covered by the building storey (normally excluding the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea  
    ///   GrossVolume Calculated gross volume of all areas enclosed by the building storey (normally including the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Calculated net volume of all areas enclosed by the building storey (normally excluding the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   Spatial Structure Use Definition
    ///   The IfcBuildingStorey is used to build the spatial structure of a building (that serves as the primary project breakdown and is required to be hierarchical). The spatial structure elements are linked together by using the objectified relationship IfcRelAggregates. The IfcBuildingStoreyreferences them by its inverse relationships:
    ///   IfcBuildingStorey.Decomposes -- referencing (IfcBuilding || IfcBuildingStorey) by IfcRelAggregates.RelatingObject, If it refers to another instance of IfcBuildingStorey, the referenced IfcBuildingStorey needs to have a different and higher CompositionType, i.e. COMPLEX (if the other IfcBuildingStorey has ELEMENT), or ELEMENT (if the other IfcBuildingStorey has PARTIAL). 
    ///   IfcBuildingStorey.IsDecomposedBy -- referencing (IfcBuildingStorey || IfcSpace) by IfcRelAggregates.RelatedObjects. If it refers to another instance of IfcBuildingStorey, the referenced IfcBuildingStorey needs to have a different and lower CompositionType, i.e. ELEMENT (if the other IfcBuildingStorey has COMPLEX), or PARTIAL (if the other IfcBuildingStorey has ELEMENT). 
    ///   If there are building elements and/or other elements directly related to the IfcBuildingStorey (like most building elements, such as walls, columns, etc.), they are associated with the IfcBuildingStorey by using the objectified relationship IfcRelContainedInSpatialStructure. The IfcBuildingStorey references them by its inverse relationship:
    ///   IfcBuildingStorey.ContainsElements -- referencing any subtype of IfcProduct (with the exception of other spatial structure element) by IfcRelContainedInSpatialStructure.RelatedElements. 
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcBuildingStorey is given by the IfcProductDefinitionShape and IfcLocalPlacement, allowing multiple geometric representation. 
    ///   Local Placement
    ///   The local placement for IfcBuildingStorey is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if relative placement is used) to the IfcSpatialStructureElement of type "IfcBuilding", or of type "IfcBuildingStorey" (e.g. to position a building storey relative to a building storey complex, or a partial building storey to a building storey). 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of a 2D 'FootPrint' representation of type 'GeometricCurveSet' and a 3D 'Body' representation of type 'Brep' is supported.
    ///   Foot Print Representation 
    ///   The foot print representation of IfcBuildingStorey is given by either a single 2D curve (such as IfcPolyline or IfcCompositeCurve), or by a list of 2D curves (in case of inner boundaries), if the building storey has an independent geometric representation.
    ///   The representation identifier and type of this geometric representation of IfcBuildingStorey is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'FootPrint' 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricCurveSet' 
    ///   Body Representation 
    ///   The body (or solid model) geometric representation (if the building storey has an independent geometric representation) of IfcBuildingStorey is defined using faceted B-Rep capabilities (with or without voids), based on the IfcFacetedBrep or on the IfcFacetedBrepWithVoids. 
    ///   The representation identifier and type of this representation of IfcBuildingStorey is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Body' 
    ///   IfcShapeRepresentation.RepresentationType = 'Brep' 
    ///   Since the building storey shape is usually described by the exterior building elements, an independent shape representation shall only be given, if the building storey is exposed independently from its constituting elements.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBuildingStorey : IfcSpatialStructureElement
    {
        #region Fields

        private IfcLengthMeasure? _elevation;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional.    Elevation of the base of this storey, relative to the 0,00 internal reference height of the building. The 0.00 level is given by the absolute above sea level height by the ElevationOfRefHeight attribute given at IfcBuilding.
        /// </summary>

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLengthMeasure? Elevation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elevation;
            }
            set { this.SetModelValue(this, ref _elevation, value, v => _elevation = v, "Elevation"); }
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
                    _elevation = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}
