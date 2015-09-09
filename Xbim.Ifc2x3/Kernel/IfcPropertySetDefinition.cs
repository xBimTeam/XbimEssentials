#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertySetDefinition.cs
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
    public class PropertySetDefinitionSet : XbimSet<IfcPropertySetDefinition>
    {
        internal PropertySetDefinitionSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }
    }


    /// <summary>
    ///   An IfcPropertySetDefinition is a generalization of property sets, that are either: 
    ///   Dynamically extendable properties, they define properties for which the IFC model only provides a kind of "meta model", to be further declared by agreement. 
    ///   This means no entity definition of the properties exists within the IFC model. The declaration is done by assigning a significant string value to the 
    ///   Name attribute of the entity as defined in the entity IfcPropertySet and at each subtype of IfcProperty, referenced by the property set.
    ///   Statically defined properties, they define properties for which an entity definition exists within the IFC model. 
    ///   The semantic meaning of each statically defined property is declared by its entity type and the meaning of the properties is defined by the name of the explicit attribute.
    ///   The subtypes of the IfcPropertySetDefinition are either the dynamically extendable IfcPropertySet, or all other statically defined subtypes.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPropertySetDefinition : IfcPropertyDefinition
    {
        #region Inverse Relationships

        /// <summary>
        ///   Inverse: The property style to which the property set might belong.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelDefinesByProperties> PropertyDefinitionOf
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelDefinesByProperties>(
                        rel => rel.RelatingPropertyDefinition == this);
            }
        }

        /// <summary>
        ///   Inverse: Reference to the relation to one or many objects that are characterized by the property definition. The reference may be omited, if the property definition is used to define a style library and no instances, to which the particular style of property set is associated, exist yet.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcTypeObject> DefinesType
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcTypeObject>(
                        t => t.HasPropertySets.Contains(this));
            }
        }

        #endregion
    }
}