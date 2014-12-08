#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcVertexLoop.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A vertex_loop is a loop of zero genus consisting of a single vertex.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A vertex_loop is a loop of zero genus consisting of a single vertex. A vertex can exist independently of a vertex loop. The topological data shall satisfy the following constraint:
    ///   Informal propositions:
    ///   A vertex loop has zero extent and dimensionality. 
    ///   The vertex loop has genus 0. 
    ///   NOTE: Corresponding STEP entity: vertex_loop. Please refer to ISO/IS 10303-42:1994, p. 121 for the final definition of the formal standard. 
    ///   HISTORY: New Entity in Release IFC 2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcVertexLoop : IfcLoop
    {
        private IfcVertex _loopVertex;

        /// <summary>
        ///   The vertex which defines the entire loop.
        /// </summary>
        public IfcVertex LoopVertex
        {
            get { return _loopVertex; }
            set { _loopVertex = value; }
        }
    }
}