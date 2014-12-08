#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuilding.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Construction work that has the provision of shelter for its occupants or contents as one of its main purpose and is normally designed to stand permanently in one place.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Construction work that has the provision of shelter for its occupants or contents as one of its main purpose and is normally designed to stand permanently in one place.
    ///   Definition from IAI: A building represents a structure that provides shelter for its occupants or contents and stands in one place. The building is also used to provide a basic element within the spatial structure hierarchy for the components of a building project (together with site, storey, and space). 
    ///   A building is (if specified) associated to a site. A building may span over several connected or disconnected buildings. Therefore building complex provides for a collection of buildings included in a site. A building can also be decomposed in (vertical) parts, where each part defines a building section. This is defined by the composition type attribute of the supertype IfcSpatialStructureElements which is interpreted as follow:
    ///   COMPLEX = building complex 
    ///   ELEMENT = building 
    ///   PARTIAL = building section 
    ///   HISTORY: New entity in IFC Release 1.0.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcBuilding are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcBuilding are part of this IFC release:
    ///   Pset_BuildingCommon: common property set for all types of buildings 
    ///   Pset_BuildingWaterStorage: specific property set for buildings to capture the water supply requirements 
    ///   Pset_BuildingUse: specific property set for buildings to capture the current and anticipated real estate context. 
    ///   Pset_BuildingUseAdjacent: specific property set for buildings to capture the use information about the adjacent buildings. 
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcBuilding are defined by the IfcElementQuantity and attached by the IfcRelDefinesByProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   TotalHeight Calculated total height of the building, measured from the level of terrain to the top part of the building IfcQuantityLength  
    ///   SiteCoverage Calculated coverage of the building site area that is occupied by the building (also referred to as footprint). IfcQuantityArea 
    ///   GrossFloorArea Calculated sum of all areas covered by the building (normally including the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea 
    ///   NetFloorArea Calculated sum of all usable areas covered by the building (normally excluding the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityArea  
    ///   GrossVolume Calculated gross volume of all areas enclosed by the building (normally including the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   NetVolume Calculated net volume of all areas enclosed by the building (normally excluding the area of construction elements). The exact definition and calculation rules depend on the method of measurement used. IfcQuantityVolume 
    ///   Spatial Structure Use Definition
    ///   The IfcBuilding is used to build the spatial structure of a building (that serves as the primary project breakdown and is required to be hierarchical). The spatial structure elements are linked together by using the objectified relationship IfcRelAggregates. The IfcBuilding references them by its inverse relationships:
    ///   IfcBuilding.Decomposes -- referencing (IfcSite || IfcBuilding) by IfcRelAggregates.RelatingObject, If it refers to another instance of IfcBuilding, the referenced IfcBuilding needs to have a different and higher CompositionType, i.e. COMPLEX (if the other IfcBuilding has ELEMENT), or ELEMENT (if the other IfcBuilding has PARTIAL). 
    ///   IfcBuilding.IsDecomposedBy -- referencing (IfcBuilding || IfcBuildingStorey) by IfcRelAggregates.RelatedObjects. If it refers to another instance of IfcBuilding, the referenced IfcBuilding needs to have a different and lower CompositionType, i.e. ELEMENT (if the other IfcBuilding has COMPLEX), or PARTIAL (if the other IfcBuilding has ELEMENT). 
    ///   If there are building elements and/or other elements directly related to the IfcBuilding (like a curtain wall spanning several stories), they are associated with the IfcBuilding by using the objectified relationship IfcRelContainedInSpatialStructure. The IfcBuilding references them by its inverse relationship:
    ///   IfcBuilding.ContainsElements -- referencing any subtype of IfcProduct (with the exception of other spatial structure element) by IfcRelContainedInSpatialStructure.RelatedElements. 
    ///   Attribute Use Definition:
    ///   The heated space within a Building shall be handled by the IfcZone, including the property for overall height of the heated space in the Building. The following figure shall define the interpretation of building heights and elevations for IfcBuilding.
    ///   The ElevationOfRefHeight is used to give the height above sea level of the internal height 0.00. The height 0.00 is often used as a building internal reference height and equal to the floor finish level of the ground floor. 
    ///  
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcBuilding is given by the IfcProductDefinitionShape and IfcLocalPlacement, allowing multiple geometric representation. 
    ///   Local Placement 
    ///   The local placement for IfcBuilding is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if relative placement is used) to the IfcSpatialStructureElement of type "IfcSite", or of type "IfcBuilding" (e.g. to position a building relative to a building complex, or a building section to a building). 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of a 2D 'FootPrint' representation of type 'GeometricCurveSet' and a 3D 'Body' representation of type 'Brep' is supported.
    ///   Foot Print Representation 
    ///   The foot print representation of IfcBuilding is given by either a single 2D curve (such as IfcPolyline or IfcCompositeCurve), or by a list of 2D curves (in case of inner boundaries), if the building has an independent geometric representation.
    ///   The representation identifier and type of this geometric representation of IfcBuilding is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'FootPrint' 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricCurveSet' 
    ///   Body Representation 
    ///   The body (or solid model) geometric representation (if the building has an independent geometric representation) of IfcBuilding is defined using faceted B-Rep capabilities (with or without voids), based on the IfcFacetedBrep or on the IfcFacetedBrepWithVoids. 
    ///   The representation identifier and type of this representation of IfcBuilding is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Body' 
    ///   IfcShapeRepresentation.RepresentationType = 'Brep' 
    ///   Since the building shape is usually described by the exterior building elements, an independent shape representation shall only be given, if the building is exposed independently from its constituting elements.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBuilding : IfcSpatialStructureElement
    {
        #region Fields

        private IfcLengthMeasure? _elevationOfRefHeight;
        private IfcLengthMeasure? _elevationOfTerrain;
        private IfcPostalAddress _buildingAddress;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Elevation above sea level of the reference height used for all storey elevation measures, equals to height 0.0. It is usually the ground floor level.
        /// </summary>

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ElevationOfRefHeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elevationOfRefHeight;
            }
            set
            {
                this.SetModelValue(this, ref _elevationOfRefHeight, value, v => ElevationOfRefHeight = v,
                                           "ElevationOfRefHeight");
            }
        }


        /// <summary>
        ///   Optional. Elevation above the minimal terrain level around the foot print of the building, given in elevation above sea level.
        /// </summary>

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ElevationOfTerrain
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elevationOfTerrain;
            }
            set
            {
                this.SetModelValue(this, ref _elevationOfTerrain, value, v => ElevationOfTerrain = v,
                                           "ElevationOfTerrain");
            }
        }

        /// <summary>
        ///   Optional. Address given to the building for postal purposes.
        /// </summary>

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPostalAddress BuildingAddress
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _buildingAddress;
            }
            set
            {
                this.SetModelValue(this, ref _buildingAddress, value, v => BuildingAddress = v,
                                           "BuildingAddress");
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
                    _elevationOfRefHeight = value.RealVal;
                    break;
                case 10:
                    _elevationOfTerrain = value.RealVal;
                    break;
                case 11:
                    _buildingAddress = (IfcPostalAddress) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}