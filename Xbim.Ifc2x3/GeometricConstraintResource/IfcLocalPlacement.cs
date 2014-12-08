#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLocalPlacement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;

using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    ///   The IfcLocalPlacement defines the relative placement of a product in relation to the placement of another product or the absolute placement of a product within the geometric representation context of the project.
    /// </summary>
    /// <remarks>
    ///   Definition from IFC: The IfcLocalPlacement defines the relative placement of a product in relation to the placement of another product or the absolute placement of a product within the geometric representation context of the project. 
    ///   The IfcLocalPlacement allows that an IfcProduct can be placed by this IfcLocalPlacement (through the attributeObjectPlacement) within the local coordinate system of the object placement of another IfcProduct, which is referenced by the PlacementRelTo. Rules to prevent cyclic relative placements have to be introduced on the application level.
    ///   If the PlacementRelTo is not given, then the IfcProduct is placed absolutely within the world coordinate system.
    ///   HISTORY: New entity in IFC Release 1.0.
    ///   Geometry use definitions:
    ///   The following conventions shall apply as default relative positions if the relative placement is used. The conventions are given for all five direct subtypes of IfcProduct, the IfcSpatialStructureElement, IfcElement, IfcAnnotation, IfcGrid, IfcPort. More detailed placement information is given at the level of subtypes of those five types mentioned.
    ///   For the subtypes of IfcSpatialStructureElement the following conventions apply 
    ///   IfcSite shall be placed absolutely within the world coordinate system established by the geometric representation context of the IfcProject 
    ///   IfcBuilding shall be placed relative to the local placement of IfcSite 
    ///   IfcBuildingStorey shall be placed relative to the local placement of IfcBuilding 
    ///   For IfcGrid and IfcAnnotation the convention applies that it shall be placed relative 
    ///   to the local placement of its container (IfcSite, IfcBuilding, IfcBuildingStorey) 
    ///   it should be the same container element that is referenced by the IfcRelContainedInSpatialStructure containment relationship, 
    ///   For IfcPort the convention applies that it shall be placed relative 
    ///   to the local placement of the element it belongs to (IfcElement) 
    ///   it should be the same element that is referenced by the IfcRelConnectsPortToElement connection relationship, 
    ///   For IfcElement the convention applies that it shall be placed relative: 
    ///   to the local placement of its container (IfcSite, IfcBuilding, IfcBuildingStorey) 
    ///   it should be the same container element that is referenced by the IfcRelContainedInSpatialStructure containment relationship, 
    ///   to the local placement of the IfcElement to which it is tied by an element composition relationship 
    ///   for features that are located relative to the main component (such as openings), as expressed by IfcRelVoidsElement and IfcRelProjectsElement, 
    ///   for elements that fill an opening (such as doors or windows), as expressed byIfcRelFillsElement, 
    ///   for coverings that cover the element, as expressed byIfcRelCoversBldgElements, 
    ///   for sub components that are aggregated to the main component, as expressed by IIfcRelAggregates and IfcRelNests) 
    ///   If the PlacementRelTo relationship is not given, then it defaults to an absolute placement within the world coordinate system established by the referenced geometric representation context within the project. 
    ///   EXPRESS specification:
    ///   Formal Propositions:
    ///   WR21   :   Ensures that a 3D local placement can only be relative (if exists) to a 3D parent local placement (and not to a 2D parent local placement).
    /// </remarks>
    [IfcPersistedEntityAttribute,IndexedClass]
    public class IfcLocalPlacement : IfcObjectPlacement
    {
        #region Fields

        private IfcObjectPlacement _placementRelTo;
        private IfcAxis2Placement _relativePlacement;

        #endregion

        #region Constructors

        public IfcLocalPlacement()
        {
        }

        public IfcLocalPlacement(IfcAxis2Placement relativePlacement)
            : this()
        {
            if (relativePlacement == null)
                throw new ArgumentNullException("relativePlacement argument can not be null");
            _relativePlacement = relativePlacement;
        }

        public IfcLocalPlacement(IfcAxis2Placement relativePlacement, IfcObjectPlacement placementRelTo)
            : this()
        {
            if (relativePlacement == null)
                throw new ArgumentNullException("relativePlacement argument can not be null");
            _relativePlacement = relativePlacement;
            _placementRelTo = placementRelTo;
        }

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Reference to Object that provides the relative placement by its local coordinate system. If it is omitted, then the local placement is given to the WCS, established by the geometric representation context.
        /// </summary>

        
        [IfcAttribute(1, IfcAttributeState.Optional),IndexedProperty]
        public IfcObjectPlacement PlacementRelTo
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _placementRelTo;
            }
            set { this.SetModelValue(this, ref _placementRelTo, value, v => PlacementRelTo = v, "PlacementRelTo"); }
        }

        /// <summary>
        ///   Geometric placement that defines the transformation from the related coordinate system into the relating. The placement can be either 2D or 3D, depending on the dimension count of the coordinate system.
        /// </summary>

       
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement RelativePlacement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relativePlacement;
            }
            set
            {
                this.SetModelValue(this, ref _relativePlacement, value, v => RelativePlacement = v,
                                           "RelativePlacement");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _placementRelTo = (IfcObjectPlacement) value.EntityVal;
                    break;
                case 1:
                    _relativePlacement = (IfcAxis2Placement)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        internal override void CopyValues(IfcObjectPlacement value)
        {
            IfcLocalPlacement lp = value as IfcLocalPlacement;
            PlacementRelTo = lp.PlacementRelTo;
            RelativePlacement = lp.RelativePlacement;
        }


        public override string WhereRule()
        {
            if (_placementRelTo != null)
            {
                if (_placementRelTo is IfcLocalPlacement && _relativePlacement is IfcAxis2Placement3D &&
                    ((IfcLocalPlacement) _placementRelTo).RelativePlacement.Dim != 3)
                    return
                        "WR21 LocalPlacement : A 3D local placement can only be relative (if exists) to a 3D parent local placement (and not to a 2D parent local placement).\n";
            }
            return "";
        }
    }
}