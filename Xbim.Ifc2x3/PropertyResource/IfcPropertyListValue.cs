#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyListValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   An IfcPropertyListValue defines a property that has several (numeric or descriptive) values assigned, these values are given by an ordered list.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcPropertyListValue defines a property that has several (numeric or descriptive) values assigned, these values are given by an ordered list. 
    ///   An IfcPropertyListValue is a list of values. The order in which values appear is significant. Each value in the list is unique i.e. no duplicate values are allowed. All list members should be of the same type.
    ///   The unit applicable to all values is handled by the Unit attribute:
    ///   If the Unit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the Unit attribute is given, then the unit assigned by the Unit attribute overrides the globally assigned unit. 
    ///   Example of a property with list value is:
    ///   Name ListValues Type 
    ///   (through IfcValue) Unit 
    ///  
    ///   ApplicableSizes 1200 IfcPositiveLengthMeasure - 
    ///   - 1600 IfcPositiveLengthMeasure - 
    ///   - 2400 IfcPositiveLengthMeasure - 
    ///   HISTORY: New Entity in Release IFC 2x Edition 2.
    ///   Formal Propositions:
    ///   WR31   :   All values within the list of values shall be of the same measure type.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertyListValue : IfcSimpleProperty
    {
        public IfcPropertyListValue()
        {
            _listValues = new XbimList<IfcValue>(this);
        }

        #region Fields

        private XbimList<IfcValue> _listValues;
        private IfcUnit _unit;

        #endregion

        #region Constructors

        public IfcPropertyListValue(IfcIdentifier name)
            : base(name)
        {
            _listValues = new XbimList<IfcValue>(this);
        }

        #endregion

        /// <summary>
        ///   List of values.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcValue> ListValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _listValues;
            }
            set { this.SetModelValue(this, ref _listValues, value, v => ListValues = v, "ListValues"); }
        }

        /// <summary>
        ///   Optional.   Unit for the list values, if not given, the default value for the measure type (given by the TYPE of nominal value) is used as defined by the global unit assignment at IfcProject.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcUnit Unit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _unit;
            }
            set { this.SetModelValue(this, ref _unit, value, v => Unit = v, "Unit"); }
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
                    _listValues.Add((IfcValue) value.EntityVal);
                    break;
                case 3:
                    _unit = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            bool first = true;
            Type type = null;
            foreach (IfcValue val in _listValues)
            {
                if (first)
                {
                    type = val.GetType();
                    first = false;
                }
                else if (type != val.GetType())
                    return
                        "WR31 PropertyListValue : All values within the list of values shall be of the same measure type.\n";
            }
            return "";
        }
    }
}