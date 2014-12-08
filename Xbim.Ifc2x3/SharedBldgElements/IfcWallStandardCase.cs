#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWallStandardCase.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   The standard wall (IfcWallStandardCase) defines a wall with certain constraints for the provision of parameter and with certain constraints for the geometric representation.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The standard wall (IfcWallStandardCase) defines a wall with certain constraints for the provision of parameter and with certain constraints for the geometric representation. The IfcWallStandardCase handles all cases of walls, that have a single thickness along the path, i.e.:
    ///   parallel sides for straight walls 
    ///   co-centric sides for curved walls. 
    ///   The following parameter shall be given:
    ///   Wall height, taken from the depth of extrusion, provided by the geometric representation. 
    ///   Wall thickness, taken from the material layer set, attached to the wall 
    ///   Wall offset from axis, taken from the material layer set, attached to the wall 
    ///   The material of the wall is defined by the IfcMaterialLayerSetUsage and attached by the IfcRelAssociatesMaterial. It is accessibly by the inverse HasAssociations relationship. The material layer set has to be given (enforced by where rule).
    ///   HISTORY New entity in IFC Release 2x.
    ///   Property Set Use Definition:
    ///   The property sets relating to the IfcWallStandardCase are defined at the supertype IfcWall.
    ///   Quantity Use Definition:
    ///   The quantities relating to the IfcWallStandardCase are defined at the supertype IfcWall.
    ///   Geometry Use Definitions:
    ///   The geometric representation of IfcWallStandardCase is given by the IfcProductDefinitionShape, allowing multiple geometric representation. Included are: 
    ///   Local Placement
    ///   The use of local placement is defined at the supertype IfcWall.
    ///   Geometric Representation
    ///   The standard geometric representation of IfcWallStandardCase is defined using the following multiple shape representations for its definition:
    ///   Axis: A two-dimensional open curve (IfcBoundedCurve) defining the axis for the standard wall. The material layer offset is measured from the wall axis. 
    ///   Body: A Swept Solid Representation or a CSG representation defining the 3D shape of the standard wall 
    ///   First representation: Curve2D representation of wall axis
    ///   The wall axis is represented by a two-dimensional open curve within a particular shape representation. The wall axis is used to apply the parameter to the wall geometry. The following attribute values shall be used
    ///   IfcShapeRepresentation shall have the following values:
    ///   RepresentationIdentifier : 'Axis' 
    ///   RepresentationType : 'Curve2D' 
    ///   In case of a straight wall, the set of items shall include a single geometric representation item of type IfcPolyline or IfcTrimmedCurve with the BasisCurve being an IfcLine.
    ///  
    ///   In case of a curved wall, the set of items shall include a single geometric representation item of type IfcTrimmedCurve. The curve shall have a BasisCurve of type IfcCircle.
    ///  
    ///   Second representation: SweptSolid or Clipping representation of wall body
    ///   The body of the IfcWallStandardCase is defined by using 'SweptSolid' representation for walls without clippings or 'Clipping' representation for walls with clippings (e.g. under sloped roof slabs).
    ///   IfcShapeRepresentation shall have the following values:
    ///   RepresentationIdentifier : 'Body' 
    ///   RepresentationType : 'SweptSolid' or 'Clipping' 
    ///   SweptSolid representation
    ///   The standard geometric representation (for body) of IfcWallStandardCase is defined using the 'SweptSolid' representation. The following additional constraints apply to the swept solid representation:
    ///   Solid: IfcExtrudedAreaSolid is required, 
    ///   Profile: IfcArbitraryClosedProfileDef and IfcRectangleProfileDef shall be supported. 
    ///   Extrusion: The profile shall be extruded vertically, i.e., in the direction of the z-axis of the co-ordinate system of the referred spatial structure element. It might be further constraint to be in the direction of the global z-axis in implementers agreements. The extrusion axis shall be perpendicular to the swept profile, i.e. pointing into the direction of the z-axis of the Position of the IfcExtrudedAreaSolid. 
    ///   The profile of a wall is described in the ground view and extruded vertically. The profile (also identical with the foot print of the wall) is defined by the IfcArbitraryClosedProfileDef (excluding its subtypes). The profile is given with all wall connections already resolved. 
    ///   In case of a straight wall the two sides of the profile shall be parallel to the wall axis, i.e. the wall has a single unchanged thickness. 
    ///  
    ///   In case of a curved wall the two sides of the profile shall be parallel (with defined offset) to the wall axis, i.e. the wall has a single unchanged thickness. 
    ///  
    ///   The advanced geometric representation (for body) of IfcWallStandardCase is defined using the 'Clipping' representation. The following additional constraints apply to the swept solid representation:
    ///   Solid: see standard geometric representation 
    ///   Profile: see standard geometric representation 
    ///   Extrusion: see standard geometric representation 
    ///   Boolean result: The IfcBooleanClippingResult shall be supported, allowing for Boolean differences between the swept solid (here IfcExtrudedAreaSolid) and one or several IfcHalfSpaceSolid (or subtypes). 
    ///   Example of clipping using an IfcPolygonalBoundedHalfSpace as SecondOperand in the IfcBooleanClippingResult.
    ///  
    ///   Example of clipping using an IfcHalfSpaceSolid as SecondOperand in the IfcBooleanClippingResult
    ///   Formal Propositions:
    ///   WR1   :   The IfcWallStandard relies on the provision of an IfcMaterialLayerSetUsage.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcWallStandardCase : IfcWall
    {
        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        #endregion
    }
}