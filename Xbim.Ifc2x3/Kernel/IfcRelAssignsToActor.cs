#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignsToActor.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ActorResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship (IfcRelAssignsToActor) handles the assignment of objects (subtypes of IfcObject) to an actor (subtypes of IfcActor).
    ///   The IfcRelAssignsToActor objectified relationship defines a relationship between an IfcActor and one or many objects. 
    ///   A particular role of the actor played in that relationship can be associated. If specified, it takes priority over the role that may be directly assigned to the person or organization.
    ///   Example: An occupant (as an actor) may rent (as a special association type) a flat (as a collection of spaces or a zone). This would be an application of this generic relationship.
    ///   Reference to the objects (or single object) on which the actor acts upon in a certain role (if given) is specified in the inherited RelatedObjects attribute.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssignsToActor : IfcRelAssigns
    {
        #region Fields

        private IfcActor _relatingActor;
        private IfcActorRole _actingRole;

        #endregion

        #region Constructors & Initialisers

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Reference to the information about the actor. It comprises the information about the person or organization and its addresses.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcActor RelatingActor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingActor;
            }
            set { this.SetModelValue(this, ref _relatingActor, value, v => RelatingActor = v, "RelatingActor"); }
        }

        /// <summary>
        ///   Role of the actor played within the context of the assignment to the object(s).
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcActorRole ActingRole
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _actingRole;
            }
            set { this.SetModelValue(this, ref _actingRole, value, v => ActingRole = v, "ActingRole"); }
        }

        #endregion

        #region Part 21 Step file Parse routines

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
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _relatingActor = value.EntityVal as IfcActor;
                    break;
                case 7:
                    _actingRole = value.EntityVal as IfcActorRole;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (RelatedObjects.Contains(RelatingActor))
                baseErr +=
                    "WR1 RelAssignsToActor: The RelatingActor shall not be contained in the List of RelatedObjects.";
            return baseErr;
        }

        #endregion
    }
}