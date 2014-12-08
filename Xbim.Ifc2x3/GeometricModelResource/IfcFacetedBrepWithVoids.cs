#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFacetedBrepWithVoids.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   The IfcFacetedBrepWithVoids is a specialization of a faceted B-rep which contains one or more voids in its interior.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcFacetedBrepWithVoids is a specialization of a faceted B-rep which contains one or more voids in its interior. The voids are represented as closed shells which are defined so that the shell normal point into the void. 
    ///   NOTE: Corresponding STEP entity: brep_with_voids (see note above). Please refer to ISO/IS 10303-42:1994, p. 173 for the final definition of the formal standard. In IFC faceted B-rep with voids is represented by this subtype IfcFacetedBrepWithVoids and not defined via an implicit ANDOR supertype constraint as in ISO/IS 10303-42:1994 between an instance of faceted_brep AND brep_with_voids. This change has been made due to the fact, that only ONEOF supertype constraint is allowed within the IFC object model. 
    ///   HISTORY: New entity in IFC Release 1.0 
    ///   Informal propositions:
    ///   Each void shell shall be disjoint from the outer shell and from every other void shell 
    ///   Each void shell shall be enclosed within the outer shell but not within any other void shell. In particular the outer shell is not in the set of void shells 
    ///   Each shell in the IfcManifoldSolidBrep shall be referenced only once. 
    ///   All the bounding loops of all the faces of all the shells in the IfcFacetedBrep shall be of type IfcPolyLoop.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcFacetedBrepWithVoids : IfcManifoldSolidBrep
    {
        public IfcFacetedBrepWithVoids()
        {
            _voids = new XbimSet<IfcClosedShell>(this);
        }

        private XbimSet<IfcClosedShell> _voids;

        /// <summary>
        ///   Set of closed shells defining voids within the solid.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcClosedShell> Voids
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _voids;
            }
            set { this.SetModelValue(this, ref _voids, value, v => Voids = v, "Voids"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _voids.Add((IfcClosedShell) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}