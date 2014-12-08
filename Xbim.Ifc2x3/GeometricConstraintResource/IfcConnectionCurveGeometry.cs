#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionCurveGeometry.cs
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
    ///   The IfcConnectionCurveGeometry is used to describe the geometric constraints that facilitate the physical connection of two objects at a curve or at an edge with curve geometry associated.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcConnectionCurveGeometry is used to describe the geometric constraints that facilitate the physical connection of two objects at a curve or at an edge with curve geometry associated. It is envisioned as a control that applies to the element connection relationships. 
    ///   EXAMPLE  The connection relationship between two walls has a geometric constraint which describes the end caps (or cut-off of the wall ends) by a CurveOnRelatingElement for the first wall and a CurveOnRelatedElement for the second wall. The exact usage of the IfcConnectionCurveGeometry is further defined in the geometry use sections of the elements that use it. 
    ///   The available geometry for the connection constraint may be further restricted to only allow straight segments by applying IfcPolyline only. Such an usage constraint is provided at the object definition of the IfcElement subtype, utilizing the element connection by refering to the subtype of IfcRelConnects with the associated IfcConnectionCurveGeometry.
    ///   HISTORY  New entity in IFC Release 1.5, has been renamed from IfcLineConnectionGeometry in IFC Release 2x.
    ///   IFC2x Edition 3 CHANGE  The provision of topology with associated geometry, IfcEdgeCurve, is enabled by using the IfcCurveOrEdgeCurve.
    ///   Geometry Use Definitions:
    ///   The IfcCurve (or the IfcEdgeCurve with an associated IfcCurve) at the CurveOnRelatingElement attribute defines the curve where the basic geometry items of the connected elements connects. The curve geometry and coordinates are provided within the local coordinate system of the RelatingElement, as specified at the IfcRelConnects Subtype that utilizes the IfcConnectionCurveGeometry. Optionally, the same curve geometry and coordinates can also be provided within the local coordinate system of the RelatedElement by using the CurveOnRelatedElement attribute.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcConnectionCurveGeometry : IfcConnectionGeometry
    {
        private IfcCurveOrEdgeCurve _curveOnRelatingElement;
        private IfcCurveOrEdgeCurve _curveOnRelatedElement;

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The bounded curve at which the connected objects are aligned at the relating element, given in the LCS of the relating element.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcCurveOrEdgeCurve CurveOnRelatingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curveOnRelatingElement;
            }
            set
            {
                this.SetModelValue(this, ref _curveOnRelatingElement, value, v => CurveOnRelatingElement = v,
                                           "CurveOnRelatingElement");
            }
        }

        /// <summary>
        ///   Optional. The bounded curve at which the connected objects are aligned at the related element, given in the LCS of the related element. If the information is omitted, then the origin of the related element is used.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional), IndexedProperty]
        public IfcCurveOrEdgeCurve CurveOnRelatedElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _curveOnRelatedElement;
            }
            set
            {
                this.SetModelValue(this, ref _curveOnRelatedElement, value, v => CurveOnRelatedElement = v,
                                           "CurveOnRelatedElement");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _curveOnRelatingElement = (IfcCurveOrEdgeCurve) value.EntityVal;
                    break;
                case 1:
                    _curveOnRelatedElement = (IfcCurveOrEdgeCurve) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}