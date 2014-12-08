#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGrid.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   A planar design grid (IfcGrid) defined in 3D space used as an aid in locating structural and design elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A planar design grid (IfcGrid) defined in 3D space used as an aid in locating structural and design elements. The position of the grid (ObjectPlacement) is defined by a 3D coordinate system (and thereby the design grid can be used in plan, section or in any position relative to the world coordinate system). The position can be relative to the object placement of other products or grids. The XY plane of the 3D coordinate system is used to place the grid axes, which are 2D curves (e.g., line, circle, trimmed curve, polyline, or composite curve).
    ///   The inherited attributes Name and Description can be used to define a descriptive name of the grid and to indicate the grid's purpose. A grid is defined by (normally) two, or (in case of a triangular grid) three lists of grid axes. The following table shows some examples.
    ///    
    ///   rectangular grid radial grid triangular grid 
    ///   The grid axes, defined within the design grid, are those elements to which project objects will be placed relatively using the IfcGridPlacement.
    ///   HISTORY New entity in IFC Release 1.0.
    ///   Informal Proposition
    ///   Grid axes, which are referenced in different lists of axes (UAxes, VAxes, WAxes) shall not be parallel. 
    ///   Grid axes should be defined such as there are no two grid axes which intersect twice. 
    ///   Informal Proposition #2 
    ///   left side: ambiguous intersections A1 and A2, a grid containing such grid axes is not a valid design grid. 
    ///   right side: the conflict can be resolved by splitting one grid axis in a way, such as no ambiguous intersections exist. 
    ///  
    ///   Geometry Use Definitions
    ///   The geometric representation of IfcGrid is given by the IfcProductDefinitionShape, allowing geometric curve set geometric representation. Included are:
    ///   Local Placement
    ///   The local placement for IfcGrid is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. 
    ///   The PlacementRelTo relationship of IfcLocalPlacement shall point (if given) to the local placement of the same IfcSpatialStructureElement, which is used in the ContainedInStructure inverse attribute, or to a spatial structure element at a higher level, referenced by that. 
    ///   If the relative placement is not used, the absolute placement is defined within the world coordinate system. 
    ///   Geometric Representations
    ///   Currently, the use of a 2D 'FootPrint' representation of type 'Curve2D' or 'GeometricCurveSet' and a 3D 'Body' representation of type 'SweptSolid, 'Clipping' and 'Brep' is supported.
    ///   Standard Representation using geometric curve set representation
    ///   The 2D geometric representation of IfcGrid is defined using the 'GeometricCurveSet' geometry. The following attribute values should be inserted
    ///   IfcShapeRepresentation.RepresentationIdentifier = 'GridAxes'. 
    ///   IfcShapeRepresentation.RepresentationType = 'GeometricCurveSet' . 
    ///   The following constraints apply to the 2D representation: 
    ///   The IfcGeometricCurveSet shall be an Item of the IfcShapeRepresentation. It should contain a set of subtypes of IfcCurve, each representing a grid axis. Applicable subtypes of IfcCurve are: IfcPolyline, IfcCircle, IfcTrimmedCurve (based on BaseCurve referencing IfcLine or IfcCircle).  
    ///   Each subtype of IfcCurve may have a curve style assigned, using IfcAnnotationCurveOccurrence referencing IfcCurveStyle. 
    ///   Optionally the grid axis labels may be added as IfcTextLiteral, and they may have text styles assigned, using IfcAnnotationTextOccurrence referencing IfcTextStyle. 
    ///   Illustration
    ///   The IfcGrid defines a placement coordinate system using the ObjectPlacement. The XY plane of the coordinate system is used to place the 2D grid axes. The Representation of IfcGrid is defined using IfcProductRepresentation, referencing an IfcShapeRepresentation, that includes IfcGeometricCurveSet as Items. All grid axes are added as IfcPolyline to the IfcGeometricCurveSet.
    ///   The attributes UAxes and VAxes define lists of IfcGridAxis within the context of the grid. Each instance of IfcGridAxis refer to the same instance of IfcPolyline within the IfcGeometricCurveSet (above) that represents the IfcGridAxis.
    ///  
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcGrid : IfcProduct
    {
        public IfcGrid()
        {
            _uAxes = new XbimListUnique<IfcGridAxis>(this);
            _vAxes = new XbimListUnique<IfcGridAxis>(this);
        }

        #region Fields

        private XbimListUnique<IfcGridAxis> _uAxes;
        private XbimListUnique<IfcGridAxis> _vAxes;
        private XbimListUnique<IfcGridAxis> _wAxes;

        #endregion

        /// <summary>
        ///   List of grid axes defining the first row of grid lines.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.ListUnique, IfcAttributeType.Class, 1)]
        public XbimListUnique<IfcGridAxis> UAxes
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _uAxes;
            }
            set { this.SetModelValue(this, ref _uAxes, value, v => UAxes = v, "UAxes"); }
        }

        /// <summary>
        ///   List of grid axes defining the second row of grid lines.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory, IfcAttributeType.ListUnique, IfcAttributeType.Class, 1)]
        public XbimListUnique<IfcGridAxis> VAxes
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _vAxes;
            }
            set { this.SetModelValue(this, ref _vAxes, value, v => VAxes = v, "VAxes"); }
        }

        /// <summary>
        ///   Optional. List of grid axes defining the third row of grid lines. It may be given in the case of a triangular grid.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional, IfcAttributeType.ListUnique, IfcAttributeType.Class, 1)]
        public XbimListUnique<IfcGridAxis> WAxes
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _uAxes;
            }
            set { this.SetModelValue(this, ref _wAxes, value, v => WAxes = v, "WAxes"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _uAxes.Add((IfcGridAxis)value.EntityVal);
                    break;
                case 8:
                    _vAxes.Add((IfcGridAxis)value.EntityVal);
                    break;
                case 9:
                    if (_wAxes == null) _wAxes = new XbimListUnique<IfcGridAxis>(this);
                    _wAxes.Add((IfcGridAxis)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        /// <summary>
        ///   Inverse. Relationship to a spatial structure element, to which the grid is primarily associated.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelContainedInSpatialStructure> ContainedInStructure
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelContainedInSpatialStructure>(
                        r => r.RelatedElements.Contains(this));
            }
        }
    }
}