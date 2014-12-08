#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTopologyRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcTopologyRepresentation represents the concept of a particular topological representation of a product or a product component within a representation context.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcTopologyRepresentation represents the concept of a particular topological representation of a product or a product component within a representation context. This representation context does not need to be (but may be) a geometric representation context. Several representation types for shape representation are included as predefined types: 
    ///   Vertex topological vertex representation (with or without assigned geometry) 
    ///   Edge topological edge representation (with or without assigned geometry) 
    ///   Path topological path representation (with or without assigned geometry) 
    ///   Face topological face representation (with or without assigned geometry) 
    ///   Shell topological shell representation (with or without assigned geometry) 
    ///   Undefined no constraints imposed 
    ///   The representation type is given as a string value at the inherited attribute 'RepresentationType'.
    ///   HISTORY: New entity in Release IFC 2x Edition 2. 
    ///   Formal Propositions:
    ///   WR21   :   Only topological representation items should be used.  
    ///   WR22   :   A representation type should be given to the topology representation.  
    ///   WR23   :   Checks the proper use of Items according to the RepresentationType.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcTopologyRepresentation : IfcShapeModel
    {
        #region Constructors

        #endregion
    }
}