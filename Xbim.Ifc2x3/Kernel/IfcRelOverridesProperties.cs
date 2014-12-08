#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelOverridesProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The objectified relationship (IfcRelOverridesProperties) defines the relationships between objects and a standard property set.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The objectified relationship (IfcRelOverridesProperties) defines the relationships between objects and a standard property set. It also defines a set of properties, which values override the standard values given within the standard property set.
    ///   The inherited attributes should be interpreted as follows:
    ///   SELF\IfcRelDefinedByProperties.RelatingPropertyDefinition: Property set, which defines the standard set of properties assigned to all objects, that have the same set of properties, 
    ///   SELF\IfcRelDefines.RelatedObjects: An object occurrence, to which the same set of properties is applied. The object is characterized that certain property values, given by the standard set of properties, have a different value than those defined for all objects of the same style, 
    ///   OverridingProperties: A set of properties, that have a different value for the subset of objects. The set of the individual overriding properties have to correspond with a standard property set and its containing properties, as given by the RelatingPropertyDefinition attribute. The correspondence is established by the Name attribute. 
    ///   It is provided as specialization of IfcRelDefinedByProperties relationship. 
    ///   NOTE that there must be a correspondence between the names of the properties in the set of overriding properties and the names of the properties whose values are to be changed in the base property set. In addition the inherited attribute RelatingPropertyDefinition points to the property set which values are overridden.
    ///   HISTORY: New Entity in IFC Release 2x.
    ///   Formal Propositions:
    ///   WR1   :   The overriding is only applicable as an occurrence property set - i.e. it can only be assigned to a single occurrence of IfcObject.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelOverridesProperties : IfcRelDefinesByProperties
    {
        public IfcRelOverridesProperties()
        {
            _overridingProperties = new XbimSet<IfcProperty>(this);
        }

        #region Fields

        private XbimSet<IfcProperty> _overridingProperties;

        #endregion

        /// <summary>
        ///   A property set, which contains those properties, that have a different value for the subset of objects.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcProperty> OverridingProperties
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overridingProperties;
            }
            set
            {
                this.SetModelValue(this, ref _overridingProperties, value, v => OverridingProperties = v,
                                           "OverridingProperties");
            }
        }

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
                    _overridingProperties.Add((IfcProperty) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
            base.IfcParse(propIndex, value);
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (RelatedObjects.Count != 1)
                baseErr +=
                    "WR1 RelOverridesProperties: The overriding is only applicable as an occurrence property set - i.e. it can only be assigned to a single occurrence of IfcObject.\n";
            return baseErr;
        }
    }
}