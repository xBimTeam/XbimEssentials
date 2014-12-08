#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricRepresentationItem.cs
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
    ///   geometric representation item is a representation item that has the additional meaning of having geometric position or orientation or both.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-43:1992: An geometric representation item is a representation item that has the additional meaning of having geometric position or orientation or both. This meaning is present by virtue of: 
    ///   being a Cartesian point or a direction 
    ///   referencing directly a Cartesian point or direction 
    ///   referencing indirectly a Cartesian point or direction 
    ///   An indirect reference to a Cartesian point or direction means that a given geometric item references the Cartesian point or direction through one or more intervening geometry or topology items. 
    ///   EXAMPLE: Consider a circle. It gains its geometric position and orientation by virtue of a reference to axis2_placement (IfcAxis2Placement) that is turn references a cartesian_point (IfcCartesianPoint) and several directions (IfcDirection). 
    ///   EXAMPLE: Consider a manifold brep. A manifold_solid_brep (IfcManifoldSolidBrep) is a geometric_representation_item (IfcGeometricRepresentationItem) that through several layers of topological_representation_item's (IfcTopologicalRepresentationItem) references poly loops (IfcPolyLoop). Through additional intervening entities poly loops reference cartesian_point's (IfcCartesianPoint). 
    ///   Definition from IAI: The derivation of the dimensionality of the IfcGeometricRepresentationItem is different to STEP, there is a specific derived attribute at each class that defines the dimensionality, whereas STEP does it for the representation_context and requires all geometric_representation_item's to have the same dimensionality therein. 
    ///   The definition of swept area solids as goemteric representation items is different to STEP, it is based on a set of predefined profiles (or cross sections), i.e. a set of parameterized geometric primitives widely supported in the industry. Those profiles are used to create volumes through extrusion, revolution and cross section based sweep operations. This method was called attribute driven geometric representation and it was formerly known as implicit geometry in IFC. 
    ///   NOTE: Corresponding STEP entity: geometric_representation_item. Please refer to ISO/IS 10303-42:1994, p. 22 for the final definition of the formal standard. The following changes have been made: It does not inherit from ISO/IS 10303-43:1994 entity representation_item. The derived attribute Dim is demoted to the appropriate subtypes. The WR1 has not been incorporated. Not all subtypes that are in ISO/IS 10303-42:1994 have been added to the current IFC Release . 
    ///   HISTORY: New entity in IFC Release 1.5
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcGeometricRepresentationItem : IfcRepresentationItem
    {
    }
}