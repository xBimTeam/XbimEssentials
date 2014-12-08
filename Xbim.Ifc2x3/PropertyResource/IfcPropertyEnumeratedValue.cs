#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyEnumeratedValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   A property with an enumerated value (IfcPropertyEnumeratedValue) defines a property object which has a value assigned which is chosen from an enumeration.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A property with an enumerated value (IfcPropertyEnumeratedValue) defines a property object which has a value assigned which is chosen from an enumeration. It defines a property - value combination for which the property name, the value with measure type (and optional the unit) are given.
    ///   NOTE: Multiple choices from the property enumeration are supported.
    ///   The unit is handled by the Unit attribute of the IfcPropertyEnumeration:
    ///   If the Unit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the Unit attribute is given, thrn the unit assigned by the unit attribute overrides the globally assigned unit. 
    ///   More precisely: The IfcPropertyEnumeratedValue defines a property, which value is selected from a defined list of enumerators. The enumerators are stored in a dynamic enumeration of values including the type information from IfcValue (see IfcPropertyEnumeration). This enables applications to use an enumeration value as a property within a property set (IfcPropertySet) including the allowed list of values. Examples of a property with enumerated value with are:
    ///   Name  Value
    ///   (EnumerationValue) Type 
    ///   (through IfcValue) ref. IfcPropertyEnumeration
    ///   (Name)  
    ///   BladeAction Opposed IfcString DamperBladeActionEnum 
    ///   BladeAction Parallel IfcString DamperBladeActionEnum 
    ///   The IfcPropertyEnumeratedValue refers to an IfcPropertyEnumeration, e.g. for the above:
    ///   Name EnumerationValues Type 
    ///   (through IfcValue) Unit 
    ///   DamperBladeActionEnum (Parallel, Opposed, Other, Unset) IfcString - 
    ///   HISTORY: New Entity in IFC Release 2.0, capabilities enhanced in IFC R2x. The entity has been renamed from IfcEnumeratedProperty in IFC Release 2x.
    ///   Formal Propositions:
    ///   WR1   :   Each enumeration value shall be a member of the list of EnumerationValues at the referenced IfcPropertyEnumeration (if given).
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertyEnumeratedValue : IfcSimpleProperty
    {
        public IfcPropertyEnumeratedValue()
        {
            _enumerationValues = new XbimList<IfcValue>(this);
        }

        #region Fields

        private XbimList<IfcValue> _enumerationValues;
        private IfcPropertyEnumeration _enumerationReference;

        #endregion

        /// <summary>
        ///   Optional. Enumeration values, which shall be listed in the referenced IfcPropertyEnumeration, if such a reference is provided.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcValue> EnumerationValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _enumerationValues;
            }
            set
            {
                this.SetModelValue(this, ref _enumerationValues, value, v => EnumerationValues = v,
                                           "EnumerationValues");
            }
        }

        /// <summary>
        ///   Enumeration from which a enumeration value has been selected. The referenced enumeration also establishes the unit of the enumeration value.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPropertyEnumeration EnumerationReference
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _enumerationReference;
            }
            set
            {
                this.SetModelValue(this, ref _enumerationReference, value, v => EnumerationReference = v,
                                           "EnumerationReference");
            }
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
                    _enumerationValues.Add((IfcValue) value.EntityVal);
                    break;
                case 3:
                    _enumerationReference = (IfcPropertyEnumeration) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_enumerationReference != null)
            {
                foreach (IfcValue ev in _enumerationValues)
                {
                    if (!_enumerationReference.EnumerationValues.Contains(ev))
                        return
                            "WR1 PropertyEnumeratedValue : Each enumeration value shall be a member of the list of EnumerationValues at the referenced IfcPropertyEnumeration (if given).\n";
                }
            }
            return "";
        }

        public override string ToString()
        {
            string vStr = "";
            foreach (IfcValue item in _enumerationValues)
            {
                if (vStr != "")
                    vStr += ", ";
                vStr += item.ToString();
            }
            return vStr;
        }
    }
}