#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralItem.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   The abstract entity IfcStructuralItem covers structural members and structural connections. It defines the relation needed to associate structural actions to structural members and connections.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The abstract entity IfcStructuralItem covers structural members and structural connections. It defines the relation needed to associate structural actions to structural members and connections.
    ///   Topology Use Definition
    ///   Instances of IfcStructuralCurveMember shall have a topology representation. It includes a placement and a product representation. The IfcProductRepresentation shall be given by an item of Representations being of type "IfcTopologyRepresentation".
    ///   Local Placement
    ///   The preferred placement for topological representations is the placement within the world coordinate system of the project. This is provided by leaving the ObjectPlacement attribute with a NIL value. If the ObjectPlacement however is provided, all geometric entities underneath the topological representation (such as IfcVertexPoint, IfcEdgeCurve, or IfcFaceSurface) are founded within the object coordinate system established by ObjectPlacement.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcStructuralItem : IfcProduct, IfcStructuralActivityAssignmentSelect
    {
        /// <summary>
        ///   Inverse. Inverse relationship to all structural activities (i.e. to actions or reactions) which are assigned to this structural member.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelConnectsStructuralActivity> AssignedStructuralActivity
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelConnectsStructuralActivity>(
                        r => (r.RelatingElement as IfcStructuralItem) == this);
            }
        }
    }
}