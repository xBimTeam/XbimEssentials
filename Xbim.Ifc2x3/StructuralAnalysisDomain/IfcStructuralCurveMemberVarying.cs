#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralCurveMemberVarying.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   Instances of the entity IfcStructuralCurveMemberVarying shall be used to describe linear structural elements with varying profile properties.
    ///   The varying profile properties are assigned through the IfcRelAssociatesProfileProperties with an additional link to the IfcShapeAspect, 
    ///   which relates the profile properties to the different vertices of the structural curve member.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralCurveMemberVarying : IfcStructuralCurveMember
    {
    }
}