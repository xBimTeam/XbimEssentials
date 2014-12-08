#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertySet.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public class PropertySetCollection : XbimSet<IfcPropertySet>
    {
        internal PropertySetCollection(IPersistIfcEntity owner, ICollection<IfcPropertySet> list)
            : base(owner, list)

        {
        }

        internal PropertySetCollection(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        public IfcPropertySet this[string pSetName]
        {
            get { return this.First(p => p.Name == pSetName); }
        }
    }

    /// <summary>
    ///   The IfcPropertySet defines all dynamically extensible properties.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcPropertySet defines all dynamically extensible properties. The property set is a container class that holds properties within a property tree. These properties are interpreted according to their name attribute. 
    ///   Property sets, defining a particular type of object, can be assigned an object type (IfcTypeObject). Property sets are assigned to objects (IfcObject) through an objectified relationship (IfcRelDefinedByProperties). If the same set of properties applies to more than one object, it should be assigned by a single instance of IfcRelDefinedByProperties to a set of related objects. Those property sets are referred to as shared property sets.
    ///   HISTORY  New Entity in IFC Release 1.0 
    ///   Use Definition
    ///   Instances of IfcPropertySet are used to assign named sets of individual properties (complex or single properties). Each individual property has a significant name string. Some property sets have predefined instructions on assigning those significant name, these are listed under "property sets" main menu item within this specification. The naming convention "Pset_Xxx" applies to those property sets and shall be used as the value to the Name attribute.
    ///   In addition any user defined property set can be captured, those property sets shall have a Name value not including the "Pset_" prefix.
    ///   Formal Propositions:
    ///   WR31   :   The Name attribute has to be provided. The attribute is used to specify the type of the property set definition. The property set structure for particular property sets may be given within the property set definition part of the IFC specification.
    ///  
    ///   WR32   :   Every property within the property set shall have a unique name attribute value.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertySet : IfcPropertySetDefinition
    {
        public IfcPropertySet()
        {
            _hasProperties = new SetOfProperty(this);
        }

        #region Fields

        private SetOfProperty _hasProperties;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Contained set of properties. For property sets defined as part of the IFC Object model, the property objects within a property set are defined as part of the standard.
        /// </summary>
        /// <remarks>
        ///   If a property is not contained within the set of predefined properties, its value has not been set at this time.
        /// </remarks>

        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public SetOfProperty HasProperties
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hasProperties;
            }
            private set { this.SetModelValue(this, ref _hasProperties, value, v => HasProperties = v, "HasProperties"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _hasProperties.Add(((IfcProperty) (value.EntityVal)));
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
            if (!Name.HasValue)
                err += "WR31 PropertySet : The Name attribute has to be provided.\n";
            if (_hasProperties.Distinct(new UniquePropertyNameComparer()).Count() != _hasProperties.Count)
                err +=
                    "WR32 PropertySet : Every property within the property set shall have a unique name attribute value.\n";
            return err;
        }

        #endregion
    }
}