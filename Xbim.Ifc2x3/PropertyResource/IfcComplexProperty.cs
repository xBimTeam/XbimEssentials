#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcComplexProperty.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   This IfcComplexProperty is used to define complex properties to be handled completely within a property set.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This IfcComplexProperty is used to define complex properties to be handled completely within a property set. The included list may be a mixed or consistent collection of IfcProperty subtypes. This enables the definition of a list of properties to be included as a single 'property' entry in a property set. The definition of such a list can be reused in many different property sets, but the instantiation of such a complex property shall only be used within a single property set.
    ///   NOTE: Since an IfcComplexProperty may contain other complex properties, list of properties can be nested.
    ///   HISTORY: New Entity in IFC Release 2.0, capabilities enhanced in IFC Release 2x.
    ///   Formal Propositions:
    ///   WR21   :   The IfcComplexProperty should not reference itself within the list of HasProperties.  
    ///   WR22   :   Each property within the complex property shall have a unique name attribute.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcComplexProperty : IfcProperty
    {
        public IfcComplexProperty()
        {
            _hasProperties = new SetOfProperty(this);
        }

        public IfcComplexProperty(IfcIdentifier name)
            : base(name)
        {
            _hasProperties = new SetOfProperty(this);
        }

        #region Fields

        private IfcIdentifier _usageName;
        private SetOfProperty _hasProperties;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Usage description of the IfcComplexProperty within the property set which references the IfcComplexProperty.
        /// </summary>
        /// <value>NOTE: Consider a complex property for glazing properties. The Name attribute of the IfcComplexProperty could be Pset_GlazingProperties, and the UsageName attribute could be OuterGlazingPane.</value>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcIdentifier UsageName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _usageName;
            }
            set { this.SetModelValue(this, ref _usageName, value, v => UsageName = v, "UsageName"); }
        }

        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public SetOfProperty HasProperties
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hasProperties;
            }
            set { this.SetModelValue(this, ref _hasProperties, value, v => HasProperties = v, "HasProperties"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _usageName = value.StringVal;
                    break;
                case 3:
                    _hasProperties.Add((IfcProperty) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string err = "";
            if (_hasProperties.Contains(this))
                err +=
                    "WR21 ComplexProperty : The IfcComplexProperty should not reference itself within the list of HasProperties.\n";
            if (_hasProperties.Distinct(new UniquePropertyNameComparer()).Count() != _hasProperties.Count)
                err +=
                    "WR22 ComplexProperty : Each property within the complex property shall have a unique name attribute.\n";
            return err;
        }
    }
}