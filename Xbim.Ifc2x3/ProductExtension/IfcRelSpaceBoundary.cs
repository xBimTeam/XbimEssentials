#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelSpaceBoundary.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The space boundary (IfcRelSpaceBoundary) defines the physical or virtual delimiter of a space as its relationship to the surrounding elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The space boundary (IfcRelSpaceBoundary) defines the physical or virtual delimiter of a space as its relationship to the surrounding elements. 
    ///   In the case of physical space boundary, the placement and shape of the boundary may be given, and the building element, providing the boundary, is referenced, 
    ///   In the case of virtual space boundary, the placement and shape of the boundary may be given, but no building element is referenced. 
    ///   The exact definition of how space boundaries are broken down depends on the view, more detailed conventions on how space boundaries are decomposed can only be given at the domain or application type level.
    ///   EXAMPLE: In an architectural or FM related view, a space boundary is defined from the inside of the space and does not take the providing building element into account. A plane area (even if the building element changes) is still seen as a single space boundary. In an HVAC related view, the decomposition of the space boundary depends on the material of the providing building element and the adjacent spaces behind.
    ///   HISTORY: New entity in IFC Release 1.5, the entity has been modified in IFC Release 2x .
    ///   IFC2x PLATFORM CHANGE: The data type of the attributeRelatedBuildingElement has been changed from IfcBuildingElement to its supertype IfcElement with upward compatibility for file based exchange. The data type of the attribute ConnectionGeometry has been changed from IfcConnectionSurfaceGeometry to its supertype IfcConnectionGeometry with upward compatibility for file based exchange.
    ///   The IfcRelSpaceBoundary is defined as an objectified relationship that handles the element to space relationship by objectifying the relationship between an element and the space it bounds. It is given as a one-to-one relationship, but allows each building element to define many such relationship and each space to be defined by many such relationships. 
    ///   Note: With the upward compatible platform extension the IfcRelSpaceBoundary can now also be given to an IfcOpeningElement.
    ///   Use Definitions
    ///   If the IfcRelSpaceBoundary is used to express a virtual boundary, the attribute PhysicalOrVirtualBoundary has to be set to VIRTUAL. If this virtual boundary is between two spaces, and the correct location is of interest, the attribute RelatedBuildingElement shall point to an instance of IfcVirtualElement, and the attribute ConnectionGeometry is required to be inserted. 
    ///   NOTE: The connection geometry, either by a 2D curve or a 3D surface, is used to describe the portion of the "virtual wall" that separates the two spaces. All instances of IfcRelSpaceBoundary given at the adjacent spaces share the same instance of IfcVirtualElement. Each instance of IfcRelSpaceBoundary provides in addition the ConnectionGeometry given within the local placement of each space.
    ///   NOTE: IfcVirtualElement has been introduced in IFC2x2 Addendum 1 to facilitate virtual space boundaries.
    ///   If the IfcRelSpaceBoundary is used to express a physical boundary between two spaces, the attribute PhysicalOrVirtualBoundary has to be set to PHYSICAL. The attribute RelatedBuildingElement has to be given and points to the element providing the space boundary. The attribute ConnectionGeometry may be inserted, in this case it describes the physical space boundary geometically, or it may be omited, in that case it describes a physical space boundary logically.
    ///   Geometry Use Definitions:
    ///   The IfcRelSpaceBoundary may have geometry attached. If geometry is not attached, the relationship between space and building element is handled only on a logical level. If geometry is attached, it is given within the local coordinate systems of the space and (if given in addition) of the building element.
    ///   NOTE: The attributes CurveOnRelatingElement at IfcConnectionCurveGeometry or SurfaceOnRelatingElement at IfcConnectionSurfaceGeometry provide the geometry within the local coordinate system of the IfcSpace, whereas the attributes CurveOnRelatedElement at IfcConnectionCurveGeometry or SurfaceOnRelatedElement at IfcConnectionSurfaceGeometry provide the geometry within the local coordinate system of the subtype of IfcElement.
    ///   The connection geometry, when given, can be given as a curve (for 2D representations of space boundaries) or as a surface (for 3D representations of space boundaries).
    ///   Note: With the upward compatible platform extension the ConnectionGeometry can now also be given by a 2D curve.
    ///   The geometric representation (through the ConnectionGeometry attribute) is defined using either 2D curve geometry or extruded surfaces for space boundaries which bounds prismatic spaces. 
    ///   The following constraints apply to the 2D curve representation: 
    ///   Curve: IfcPolyline, IfcTrimmedCurve or IfcCompositeCurve 
    ///   The following constraints apply to the surface representation: 
    ///   Surface: IfcSurfaceOfLinearExtrusion 
    ///   Profile: IfcArbitraryOpenProfileDef 
    ///   Extrusion: The extrusion direction shall be vertically, i.e., along the positive Z Axis of the co-ordinate system of the containing spatial structure element. 
    ///   Formal Propositions:
    ///   WR1   :   If the space boundary is physical, it shall be provided by an element. If the space boundary is virtual, it shall either have a virtual element providing the space boundary, or none.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelSpaceBoundary : IfcRelConnects
    {
        #region Fields

        private IfcSpace _relatingSpace;
        private IfcElement _relatedBuildingElement;
        private IfcConnectionGeometry _connectionGeometry;
        private IfcPhysicalOrVirtualEnum _physicalOrVirtualBoundary;
        private IfcInternalOrExternalEnum _internalOrExternalBoundary;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Reference to one spaces that is delimited by this boundary.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcSpace RelatingSpace
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingSpace;
            }
            set { this.SetModelValue(this, ref _relatingSpace, value, v => RelatingSpace = v, "RelatingSpace"); }
        }

        /// <summary>
        ///   Optional. Reference to Element, that defines the Space Boundaries.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional), IndexedProperty]
        public IfcElement RelatedBuildingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedBuildingElement;
            }
            set
            {
                this.SetModelValue(this, ref _relatedBuildingElement, value, v => RelatedBuildingElement = v,
                                           "RelatedBuildingElement");
            }
        }

        /// <summary>
        ///   Optional. Physical representation of the space boundary. Provided as a curve or surface given within the LCS of the space.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcConnectionGeometry ConnectionGeometry
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _connectionGeometry;
            }
            set
            {
                this.SetModelValue(this, ref _connectionGeometry, value, v => ConnectionGeometry = v,
                                           "ConnectionGeometry");
            }
        }

        /// <summary>
        ///   Defines, whether the Space Boundary is physical (Physical) or virtual (Virtual).
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcPhysicalOrVirtualEnum PhysicalOrVirtualBoundary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _physicalOrVirtualBoundary;
            }
            set
            {
                this.SetModelValue(this, ref _physicalOrVirtualBoundary, value,
                                           v => PhysicalOrVirtualBoundary = v, "PhysicalOrVirtualBoundary");
            }
        }

        /// <summary>
        ///   Defines, whether the Space Boundary is internal (Internal), or external, i.e. adjacent to open space (that can be an partially enclosed space, such as terrace (External).
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcInternalOrExternalEnum InternalOrExternalBoundary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _internalOrExternalBoundary;
            }
            set
            {
                this.SetModelValue(this, ref _internalOrExternalBoundary, value,
                                           v => InternalOrExternalBoundary = v, "InternalOrExternalBoundary");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatingSpace = (IfcSpace) value.EntityVal;
                    break;
                case 5:
                    _relatedBuildingElement = (IfcElement) value.EntityVal;
                    break;
                case 6:
                    _connectionGeometry = (IfcConnectionGeometry) value.EntityVal;
                    break;
                case 7:
                    _physicalOrVirtualBoundary =
                        (IfcPhysicalOrVirtualEnum) Enum.Parse(typeof (IfcPhysicalOrVirtualEnum), value.EnumVal, true);
                    break;
                case 8:
                    _internalOrExternalBoundary =
                        (IfcInternalOrExternalEnum) Enum.Parse(typeof (IfcInternalOrExternalEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if ((_physicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL && _relatedBuildingElement != null &&
                 _relatedBuildingElement is IfcVirtualElement)
                ||
                (_physicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.VIRTUAL && _relatedBuildingElement != null &&
                 (!(_relatedBuildingElement is IfcVirtualElement))))
                return
                    "WR1 RelSpaceBoundary : If the space boundary is physical, it shall be provided by an element. If the space boundary is virtual, it shall either have a virtual element providing the space boundary, or none.\n";
            else return "";
        }
    }
}