#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSite.cs
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
    ///   Area where construction works are undertaken.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO 6707-1:1989: Area where construction works are undertaken.
    ///   Definition from IAI: A defined area of land, possibly covered with water, on which the project construction is to be completed. A site may be used to erect building(s) or other AEC products. 
    ///   A site (IfcSite) may include a definition of the single geographic reference point for this site (global position using Longitude, Latitude and Elevation) for the project. This definition is given for informational purposes only; it is not intended to provide an absolute placement in relation to the real world. 
    ///   The geometrical placement of the site, defined by the IfcLocalPlacement, shall be always relative to the spatial structure element, in which this site is included, or absolute, i.e. to the world coordinate system, as established by the geometric representation context of the project. 
    ///   A project may span over several connected or disconnected sites. Therefore site complex provides for a collection of sites included in a project. A site can also be decomposed in parts, where each part defines a site section. This is defined by the composition type attribute of the supertype IfcSpatialStructureElements which is interpreted as follow:
    ///   COMPLEX = site complex 
    ///   ELEMENT = site 
    ///   PARTIAL = site section 
    ///   HISTORY: New entity in IFC Release 1.0.
    ///   Property Set Use Definition
    ///   The property sets relating to the IfcSite are defined by the IfcPropertySet and attached by the IfcRelDefinesByProperties relationship. It is accessible by the inverse IsDefinedBy relationship. The following property set definitions specific to the IfcSite are part of this IFC release:
    ///   Pset_SiteCommon: common property set for all types of site 
    ///   Quantity Use Definition
    ///   The quantities relating to the IfcSite are defined by the IfcElementQuantity and attached by the IfcRelAssignsProperties. The following quantities are foreseen, but will be subjected to the local standard of measurement:
    ///   Name Description Value Type 
    ///   Perimeter Perimeter of the Site boundary IfcQuantityLength  
    ///   GrossArea Gross area for this site (horizontal projections) IfcQuantityArea 
    ///   Spatial Structure Use Definition
    ///   The IfcSite is used to build the spatial structure of a building (that serves as the primary project breakdown and is required to be hierarchical). The spatial structure elements are linked together by using the objectified relationship IfcRelAggregates. The IfcSite references them by its inverse relationships:
    ///   IfcSite.Decomposes -- referencing (IfcProject || IfcSite) by IfcRelAggregates.RelatingObject, If it refers to another instance of IfcSite, the referenced IfcSite needs to have a different and higher CompositionType, i.e. COMPLEX (if the other IfcSite has ELEMENT), or ELEMENT (if the other IfcSite has PARTIAL). 
    ///   IfcSite.IsDecomposedBy -- referencing (IfcSite || IfcBuilding || IfcSpace) by IfcRelAggregates.RelatedObjects. If it refers to another instance of IfcSite, the referenced IfcSite needs to have a different and lower CompositionType, i.e. ELEMENT (if the other IfcSite has COMPLEX), or PARTIAL (if the other IfcSite has ELEMENT). 
    ///   If there are building elements and/or other elements directly related to the IfcSite (like a fence, or a shear wall), they are associated with the IfcSite by using the objectified relationship IfcRelContainedInSpatialStructure. The IfcIfcSite references them by its inverse relationship:
    ///   IfcSite.ContainsElements -- referencing any subtype of IfcProduct (with the exception of other spatial structure element) by IfcRelContainedInSpatialStructure.RelatedElements. 
    ///   Geometry Use Definitions 
    ///   The geometric representation of IfcSite is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. 
    ///   Local placement
    ///   The local placement for IfcSite is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point to the IfcSpatialStructureElement of type "IfcSite", if relative placement is used (e.g. to position a site relative a a site complex, or a site section to a site). 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. If there is only one site object, then this is the default situation. 
    ///   Foot Print Representation 
    ///   The foot print representation of IfcSite is given by either a single 2D curve (such as IfcPolyline or IfcCompositeCurve), or by a list of 2D curves (in case of inner boundaries).
    ///   The representation identifier and type of this geometric representation of IfcSite is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'FootPrint' 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricCurveSet' 
    ///   Survey Points Representation 
    ///   The survey point representation of IfcSite is defined using a set of survey points and optionally breaklines. The breaklines are restricted to only connect points given in the set of survey points. Breaklines, if given, are used to constrain the triangulation.
    ///   The representation identifier and type of this geometric representation of IfcSite is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'SurveyPoints' 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricSet' 
    ///   A set of survey points, given as 3D Cartesian points within the object coordinate system of the site. 
    ///   The set of IfcCartesianPoint is included in the set of IfcGeometricSet.Elements.
    ///  
    ///   result after facetation 
    ///   A set of survey points, given as 3D Cartesian points, and a set of break points, given as a set of lines, connecting some survey points, within the object coordinate system of the site. 
    ///   The set of IfcCartesianPoint and the set of IfcPolyline are included in the set of IfcGeometricSet.Elements.
    ///  
    ///   result after facetation taking the breaklines into account. 
    ///   NOTE  The geometric representation of the site has been based on the ARM level description of the site_shape_representation given within the ISO 10303-225 "Building Elements using explicit shape representation".
    ///   Facetation Representation 
    ///   The facetation representation of IfcSite is defined using a surface model, based on the IfcFaceBasedSurfaceModel or on the IfcShellBasedSurfaceModel. Normally the surface model is the result after triangulation of the site survey points.
    ///   The representation identifier and type of this representation of IfcSite is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Facetation' 
    ///   IfcShapeRepresentation.RepresentationType = 'SurfaceModel' 
    ///   Body Representation 
    ///   The body (or solid model) representation of IfcSite is defined using a faceted boundary representation based on the IfcFacetedBrep or on the IfcFacetedBrepWithVoids. 
    ///   The representation identifier and type of this representation of IfcSite is:
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'Body' 
    ///   IfcShapeRepresentation.RepresentationType = 'Brep'
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcSite : IfcSpatialStructureElement
    {
        #region Fields

        private IfcCompoundPlaneAngleMeasure? _refLatitude;
        private IfcCompoundPlaneAngleMeasure? _refLongitude;
        private IfcLengthMeasure? _refElevation;
        private IfcLabel? _landTitleNumber;
        private IfcPostalAddress _siteAddress;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. World Latitude at reference point (most likely defined in legal description). Defined as integer values for Degrees, minutes, seconds.
        /// </summary>
        /// <remarks>
        ///   Latitudes are measured relative to the equator, north of the equator by positive values - from 0 till +90, south of the equator by negative values - from 0 till -90.
        /// </remarks>

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcCompoundPlaneAngleMeasure? RefLatitude
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _refLatitude;
            }
            set { this.SetModelValue(this, ref _refLatitude, value, v => RefLatitude = v, "RefLatitude"); }
        }

        /// <summary>
        ///   Optional. World Longitude at reference point (most likely defined in legal description). Defined as integer values for degrees, minutes, seconds.
        /// </summary>
        /// <remarks>
        ///   Longitudes are measured relative to Greenwich as the prime meridian: longitudes west of Greenwich have positive values - from 0 till +180, longitudes east of Greenwich have negative values - from 0 till -180.
        /// </remarks>

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcCompoundPlaneAngleMeasure? RefLongitude
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _refLongitude;
            }
            set { this.SetModelValue(this, ref _refLongitude, value, v => RefLongitude = v, "RefLongitude"); }
        }

        /// <summary>
        ///   Optional. Datum elevation relative to sea level.
        /// </summary>

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcLengthMeasure? RefElevation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _refElevation;
            }
            set { this.SetModelValue(this, ref _refElevation, value, v => RefElevation = v, "RefElevation"); }
        }

        /// <summary>
        ///   Optional. The land title number (designation of the site within a regional system).
        /// </summary>

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcLabel? LandTitleNumber
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _landTitleNumber;
            }
            set
            {
                this.SetModelValue(this, ref _landTitleNumber, value, v => LandTitleNumber = v,
                                           "LandTitleNumber");
            }
        }

        /// <summary>
        ///   Optional. Address given to the site for postal purposes.
        /// </summary>

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcPostalAddress SiteAddress
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _siteAddress;
            }
            set { this.SetModelValue(this, ref _siteAddress, value, v => SiteAddress = v, "SiteAddress"); }
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
                    if (!_refLatitude.HasValue) _refLatitude = new IfcCompoundPlaneAngleMeasure();
                    _refLatitude = _refLatitude.Value.Add((int)value.IntegerVal);
                    break;
                case 10:
                    if (!_refLongitude.HasValue) _refLongitude = new IfcCompoundPlaneAngleMeasure();
                    _refLongitude = _refLongitude.Value.Add((int)value.IntegerVal);
                    break;
                case 11:
                    _refElevation = value.RealVal;
                    break;
                case 12:
                    _landTitleNumber = value.StringVal;
                    break;
                case 13:
                    _siteAddress = (IfcPostalAddress)value.EntityVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            return "Site - " + base.ToString();
        }
    }
}