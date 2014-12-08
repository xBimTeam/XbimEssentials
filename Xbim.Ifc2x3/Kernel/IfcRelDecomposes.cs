#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelDecomposes.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using System.Runtime.Serialization;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The decomposition relationship, IfcRelDecomposes, defines the general concept of elements being composed or decomposed.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The abstract generalization of all objectified relationships in IFC. Objectified relationships are the preferred way to handle relationships among objects. This allows to keep relationship specific properties directly at the relationship and opens the possibility to later handle relationship specific behavior. 
    ///   There are two different types of relationships, 1-to-1 relationships and 1-to-many relationship. used within the subtypes of IfcRelationship. The following convention applies to all subtypes:
    ///   The two sides of the objectified relationship are named 
    ///   - Relating+name of relating object and 
    ///   - Related+name of related object 
    ///   In case of the 1-to-many relationship, the related side of the relationship shall be an aggregate SET 1:N 
    ///   HISTORY: New entitiy in IFC Release 1.0.
    ///   Formal Propositions:
    ///   WR31   :   The instance to which the relation RelatingObject points shall not be contained in the List of RelatedObjects.  
    ///   WR32   :   Only object occurrences shall be valid instances for an object decomposition. 
    ///   NOTE  This restriction might be lifted in future releases of the IFC object model.  
    ///   WR33   :   Only object occurrences shall be valid instances for an object decomposition. 
    ///   NOTE  This restriction might be lifted in future releases of the IFC object model.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelDecomposes : IfcRelationship
    {
        public IfcRelDecomposes()
        {
            _relatedObjects = new ObjectDefinitionSet(this);
        }

        #region Fields and Events

        private IfcObjectDefinition _relatingObject;
        private ObjectDefinitionSet _relatedObjects;

        #endregion

        #region Constructors

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The object that represents the nest or aggregation.
        /// </summary>
        [IndexedProperty]
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcObjectDefinition RelatingObject
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingObject;
            }
            set { this.SetModelValue(this, ref _relatingObject, value, v => RelatingObject = v, "RelatingObject"); }
        }

        /// <summary>
        ///   The objects being nested or aggregated.
        /// </summary>

        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        [IndexedProperty]
        public ObjectDefinitionSet RelatedObjects
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedObjects;
            }
            set { this.SetModelValue(this, ref _relatedObjects, value, v => RelatedObjects = v, "RelatedObjects"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatingObject = (IfcObjectDefinition) value.EntityVal;
                    break;
                case 5:
                    _relatedObjects.Add((IfcObjectDefinition) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string err = "";
            if (RelatedObjects.Contains(RelatingObject))
                err +=
                    "WR31 RelDecomposes: The instance to which the relation RelatingObject points shall not be contained in the List of RelatedObjects.";
            if (RelatedObjects.OfType<IfcTypeObject>().Count() != 0)
                err +=
                    "WR32 RelDecomposes: Only IfcObject occurrences shall be valid instances for an object decomposition. This contains a TypeObject";
            if (RelatingObject is IfcTypeObject)
                err +=
                    "WR33 RelDecomposes: Only object occurrences shall be valid instances for an object decomposition.. This is a TypeObject";
            return err;
        }

        #endregion
    }
}