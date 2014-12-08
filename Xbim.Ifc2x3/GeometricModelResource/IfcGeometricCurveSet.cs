#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricCurveSet.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A geometric curve set is a collection of two or three dimensional points and curves.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A geometric curve set is a collection of two or three dimensional points and curves.
    ///   Definition from IAI: The IfcGeometricCurveSet is used for the exchange of shape representations consisting of (2D or 3D) points and curves only.
    ///   NOTE: Corresponding STEP entity: geometric_set. Please refer to ISO/IS 10303-42:1994, p. 190 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release2x Edition 2.
    ///   No surface shall be included in this geometric set.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcGeometricCurveSet : IfcGeometricSet
    {
    }
}