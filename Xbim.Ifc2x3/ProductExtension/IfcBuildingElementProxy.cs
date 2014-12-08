#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBuildingElementProxy.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The IfcBuildingElementProxy is a proxy definition that provides the same functionality, as an IfcBuildingElement, but without having a defined meaning of the special type of building element, it represents.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcBuildingElementProxy is a proxy definition that provides the same functionality, as an IfcBuildingElement, but without having a defined meaning of the special type of building element, it represents.
    ///   NOTE The building element proxy should be used to exchange special types of building elements, for which the current IFC Release does not yet provide a semantic definition.
    ///   HISTORY: New entity in IFC Release 2.x.
    ///   Geometry Use Definitions
    ///   The geometric representation of any IfcBuildingElementProxy is given by the IfcProductDefinitionShape and IfcLocalPlacement allowing multiple geometric representations. The representation types defined at the supertype IfcBuildingElement also apply.
    ///   Local Placement
    ///   The local placement for any IfcBuildingElementProxy is defined in its supertype IfcProduct. It is defined by the IfcLocalPlacement, which defines the local coordinate system that is referenced by all geometric representations. The local placement can be given relativly.
    ///   Geometric Representation
    ///   Currently, the use of 'GeometricCurveSet', 'SurfaceModel', 'SweptSolid', 'Brep' representations is supported. In addition the general representation types 'BoundingBox' and 'MappedRepresentation' are allowed.
    ///   GeometricSet Representation
    ///   Any building element proxy may be represented by a geometric set, given by a collection of 2D points, curves and surfaces. It is ensured by assigning the value 'GeometricSet' to the RepresentationType attribute of IfcShapeRepresentation The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'FootPrint'. 
    ///   SurfaceModel Representation
    ///   Any building element proxy may be represented by a surface model. The surface model representation allows for the representation of complex element shape without defining a volume. It is ensured by assigning the value 'SurfaceModel' to the RepresentationType attribute of IfcShapeRepresentation The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'. 
    ///   Swept Solid Representation
    ///   Any building element proxy may be represented by a swept area solid (either by extrusion or by revolution). The attribute RepresentationType of IfcShapeRepresentation should have the value 'SweptSolid' and the RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'. No further restrictions (e.g., for the profile or extrusion direction) are defined at this level.
    ///   Brep Representation
    ///   The general b-rep geometric representation of IfcBuildingElementProxy is defined using the Brep geometry. The Brep representation allows for the representation of complex element shape. It is ensured by assigning the value 'Brep' to the RepresentationType attribute of IfcShapeRepresentation The RepresentationIdentifier of IfcShapeRepresentation should then have the value 'Body'.
    ///   Formal Propositions:
    ///   WR1   :   A Name attribute should be asserted for a building element proxy.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBuildingElementProxy : IfcBuildingElement
    {
        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Indication, whether the proxy is intended to form an aggregation (COMPLEX), an integral element (ELEMENT), or a part in an aggregation (PARTIAL).
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcElementCompositionEnum? CompositionType { get; set; }


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
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    CompositionType =
                        (IfcElementCompositionEnum?) Enum.Parse(typeof (IfcElementCompositionEnum), value.EnumVal, true);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}