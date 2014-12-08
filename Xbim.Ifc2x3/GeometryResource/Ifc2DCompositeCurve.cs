#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Ifc2DCompositeCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   An Ifc2DCompositeCurve is an IfcCompositeCurve that is defined within the coordinate space of an IfcPlane.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An Ifc2DCompositeCurve is an IfcCompositeCurve that is defined within the coordinate space of an IfcPlane. Therefore the dimensionality of the Ifc2DCompositeCurve has to be 2. 
    ///   NOTE OF DEPRECATION: After the enhancement of the profile definition capacities in IFC Release 2.x this entity is not needed anymore for the definition of boundaries on a plane. Therefore this entity is marked as deprecated - that means it will not be supported in future versions of IFC.
    ///   NOTE: This entity has been introduced to get a more straight forward definition of surface boundaries than by its counterpart in STEP: composite_curve_on_surface and boundary_curve. Since the only basis elementary surface in IFC is the plane surface, a two dimensional composite curve provides enough capability to define the boundary. 
    ///   HISTORY: New class in IFC Release 1.5 
    ///   Formal Propositions:
    ///   WR1   :   The composite curve shall be closed.
    ///  
    ///   WR2   :   The dimensionality of the composite curve shall be 2
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class Ifc2DCompositeCurve : IfcCompositeCurve
    {
    }
}