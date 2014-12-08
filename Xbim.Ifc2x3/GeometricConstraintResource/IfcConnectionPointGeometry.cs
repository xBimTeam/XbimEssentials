#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionPointGeometry.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    ///   The IfcConnectionPointGeometry is used to describe the geometric constraints that facilitate the physical connection of two objects at a point (here IfcCartesianPoint) or at an vertex with point coordinates associated
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcConnectionPointGeometry is used to describe the geometric constraints that facilitate the physical connection of two objects at a point (here IfcCartesianPoint) or at an vertex with point coordinates associated. It is envisioned as a control that applies to the element connection relationships. 
    ///   EXAMPLE  The connection relationship between two path based elements (like a column and a beam) has a geometric constraint which describes the connection points by a PointOnRelatingElement for the column and a PointOnRelatedElement for the beam. The exact usage of the IfcConnectionPointGeometry is further defined in the geometry use sections of the elements that use it.
    ///   NOTE  If the point connection has an offset, i.e. if the two points (or vertex points) at the relating and related element do not physically match, the subtyp IfcConnectionPointEccentricity shall be used. 
    ///   HISTORY  New entity in IFC Release 1.5, has been renamed from IfcPointConnectionGeometry in IFC Release 2x. 
    ///   IFC2x Edition 3 CHANGE  The provision of topology with associated geometry, IfcVertexPoint, is enabled by using the IfcPointOrVertexPoint. 
    ///   Geometry Use Definitions: 
    ///   The IfcPoint (or the IfcVertexPoint with an associated IfcPoint) at the PointOnRelatingElement attribute defines the point where the basic geometry items of the connected elements connects. The point coordinates are provided within the local coordinate system of the RelatingElement, as specified at the IfcRelConnectsSubtype that utilizes the IfcConnectionPointGeometry. Optionally, the same point coordinates can also be provided within the local coordinate system of the RelatedElement by using the PointOnRelatedElement attribute. If both point coordinates are not identical within a common parent coordinate system (latestly within the world coordinate system), the subtype IfcConnectionPointEccentricity shall be used.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcConnectionPointGeometry : IfcConnectionGeometry
    {
        private IfcPointOrVertexPoint _pointOnRelatingElement;
        private IfcPointOrVertexPoint _pointOnRelatedElement;


        /// <summary>
        ///   Point at which the connected object is aligned at the relating element, given in the LCS of the relating element.
        /// </summary>
         [IfcAttribute(1, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcPointOrVertexPoint PointOnRelatingElement
        {
            get { return _pointOnRelatingElement; }
            set
            {
                this.SetModelValue(this, ref _pointOnRelatingElement, value, v => PointOnRelatingElement = v,
                                           "PointOnRelatingElement");
            }
        }

        /// <summary>
        ///   Optional. Point at which connected objects are aligned at the related element, given in the LCS of the related element. If the information is omitted, then the origin of the related element is used.
        /// </summary>
         [IfcAttribute(2, IfcAttributeState.Mandatory), IndexedProperty]
         public IfcPointOrVertexPoint PointOnRelatedElement
        {
            get { return _pointOnRelatedElement; }
            set
            {
                this.SetModelValue(this, ref _pointOnRelatedElement, value, v => PointOnRelatedElement = v,
                                           "PointOnRelatedElement");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _pointOnRelatingElement = (IfcPointOrVertexPoint) value.EntityVal;
                    break;
                case 1:
                    _pointOnRelatedElement = (IfcPointOrVertexPoint) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}