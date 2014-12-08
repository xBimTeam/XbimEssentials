#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcObjectDefinition.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public class ObjectDefinitionSet : XbimSet<IfcObjectDefinition>
    {
        internal ObjectDefinitionSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }

    /// <summary>
    ///   An IfcObjectDefinition is the generalization of any semantically treated thing or process, either being a type or an occurrences.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcObjectDefinition is the generalization of any semantically treated thing or process, either being a type or an 
    ///   occurrences. Object defintions can be named, using the inherited Name attribute, which should be a user recognizable label for the object
    ///   occurrance. Further explanations to the object can be given using the inherited Description attribute. 
    ///   Objects are independent pieces of information that might contain or reference other pieces of information. There are three essential kinds of
    ///   relationships in which object definitons (by their instantiable subtypes) can be involved:
    ///   Assignment of other objects - an assignment relationship (IfcRelAssigns) that refers to other types of objects and creates a bi-directional
    ///   association. The semantic of the assignment is established at the level of the subtypes of the general IfcRelAssigns relationship. There is no
    ///   dependency implied a priori by the assignment. 
    ///   Association to external resources - an association relationship (IfcRelAssociates) that refers to external sources of information (most 
    ///   notably a classification or document) and creates a uni-directional association. There is no dependency implied by the association. 
    ///   Aggregation of other objects - an aggregation relationship (IfcRelDecomposes) that establishes a whole/part relation and creates a bi-
    ///   directional relation. There is an implied dependency established. 
    ///   HISTORY  New abstract entity in Release IFC2x Edition 3. 
    ///   IFC2x Edition 3 CHANGE  The abstract entity IfcObjectDefinition has been added. Upward compatibility for file based exchange is guaranteed.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcObjectDefinition : IfcRoot
    {
        #region Fields and Events

        #endregion

        #region Inverse Relationships

        /// <summary>
        ///   Inverse. Reference to the relationship objects, that assign (by an association relationship) other subtypes of IfcObject to this object instance. Examples are the association to products, processes, controls, resources or groups.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelAssigns> HasAssignments
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelAssigns>(r => r.RelatedObjects.Contains(this));
            }
        }

        /// <summary>
        ///   Reference to the decomposition relationship, that allows this object to be the composition of other objects. An object can be decomposed by several other objects.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelDecomposes> IsDecomposedBy
        {
            get { return ModelOf.Instances.Where<IfcRelDecomposes>(r => r.RelatingObject == this); }
        }

        /// <summary>
        ///   References to the decomposition relationship, that allows this object to be a part of the decomposition. An object can only be part of a single decomposition (to allow hierarchical strutures only).
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelDecomposes> Decomposes
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelDecomposes>(r => r.RelatedObjects.Contains(this));
            }
        }

        /// <summary>
        ///   Reference to the relationship objects, that associates external references or other resource definitions to the object.. Examples are the association to library, documentation or classification.
        /// </summary>
        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class)]
        public IEnumerable<IfcRelAssociates> HasAssociations
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelAssociates>(r => r.RelatedObjects.Contains(this));
            }
        }

        #endregion
    }
}