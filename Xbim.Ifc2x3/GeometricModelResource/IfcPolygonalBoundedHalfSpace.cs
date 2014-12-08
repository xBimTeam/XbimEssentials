#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPolygonalBoundedHalfSpace.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   The polygonal bounded half space is a special subtype of a half space solid, where the material of the half space used in Boolean expressions is bounded by a polygonal boundary.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The polygonal bounded half space is a special subtype of a half space solid, where the material of the half space used in Boolean expressions is bounded by a polygonal boundary. The base surface of the half space is positioned by its normal relativeto the object coordinate system (as defined at the supertype IfcHalfSpaceSolid), and its polygonal (with or without arc segments) boundary is defined in the XY plane of the position coordinate system established by the Position attribute, the subtraction body is extruded perpendicular to the XY plane of the position coordinate system, i.e. into the direction of the positive Z axis defined by the Position attribute.
    ///   The boundary is defined by a 2 dimensional polyline (or 2 dimensional composite curve, consisting of straight segments and circular arc segments) within the XY plane of the position coordinate system. The side of the surface which is in the half space is determined by the surface normal and the agreement flag. If the agreement flag is TRUE, then the subset is the one the normal points away from. If the agreement flag is FALSE, then the subset is the one the normal points into. 
    ///   NOTE  A polygonal bounded half space is not a subtype of IfcSolidModel, half space solids are only useful as operands in Boolean expressions.
    ///   HISTORY  New class in IFC Release 2x 
    ///   Informal propositions:
    ///   The IfcPolyline or the IfcCompositeCurve providing the PolygonalBoundary shall be closed. 
    ///   If the PolygonalBoundary is given by an IfcCompositeCurve, it shall only have IfcCompositeCurveSegment's of type IfcPolyline, or IfcTrimmedCurve (having a BasisCurve of type IfcLine, or IfcCircle) 
    ///   Illustration:
    ///   black coordinates
    ///   Object coordinate system (usually provided by IfcLocalPlacement) 
    ///   green coordinates
    ///   Position coordinate system, the PolygonalBoundary is given within this coordinate system. It is provided by IfcPolygonalBoundedHalfSpace.Position. This coordinate system is relative to the object coordinate system. The extrusion direction of the subtraction body is the positve Z axis. 
    ///   red coordinates
    ///   Normal of the plane. It is provided by the BaseSurface, i.e.  IfcSurface.Positon. This normal is also relative to the object coordinate system. 
    ///  
    ///   Purpose
    ///   The polygonal bounded half space is used to limit the volume of the half space in Boolean difference expressions. Only the part that is defined by a theoretical intersection between the half space solid and an extruded area solid, defined by extruding the polygonal boundary, is used for Boolean expressions.
    ///   Parameter
    ///   The PolygonalBoundary defines the 2D polyline which bounds the effectiveness of the half space in Boolean expressions. The BaseSurface is defined by a plane, and the normal of the plane together with the AgreementFlag defines the side of the material of the half space.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPolygonalBoundedHalfSpace : IfcHalfSpaceSolid
    {
        private IfcAxis2Placement3D _position;
        private IfcBoundedCurve _polygonalBoundary;

        #region Part 21 Step file Parse routines

        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement3D Position
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _position;
            }
            set { this.SetModelValue(this, ref _position, value, v => _position = v, "Position"); }
        }

        /// <summary>
        ///   Two-dimensional polyline bounded curve, defined in the xy plane of the position coordinate system.
        /// </summary>
        /// <remarks>
        ///   WR41   :   The bounding polyline should have the dimensionality of 2.  
        ///   WR42   :   Only bounded curves of type IfcCompositeCurve, or IfcPolyline are valid boundaries.
        /// </remarks>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcBoundedCurve PolygonalBoundary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _polygonalBoundary;
            }
            set
            {
                this.SetModelValue(this, ref _polygonalBoundary, value, v => _polygonalBoundary = v,
                                           "PolygonalBoundary");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _position = (IfcAxis2Placement3D) value.EntityVal;
                    break;
                case 3:
                    _polygonalBoundary = (IfcBoundedCurve) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}