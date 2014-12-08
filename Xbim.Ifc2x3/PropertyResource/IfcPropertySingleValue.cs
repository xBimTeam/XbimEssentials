#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertySingleValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   A property with a single value (IfcPropertySingleValue) defines a property object which has a single (numeric or descriptive) value assigned.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A property with a single value (IfcPropertySingleValue) defines a property object which has a single (numeric or descriptive) value assigned. It defines a property - single value combination for which the property name, the value with measure type (and optionally the unit) is given.
    ///   The unit is handled by the Unit attribute:
    ///   If the Unit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the Unit attribute is given, then the unit assigned by the Unit attribute overrides the globally assigned unit. 
    ///   Examples of a property with single value are:
    ///   Name NominalValue Type 
    ///   (through IfcValue) Unit 
    ///  
    ///   Description Manufacturer "A" door IfcLabel - 
    ///   PanelThickness 0.12 IfcPositiveLengthMeasure -  
    ///   ThermalTransmittance 2.6 IfcThermalTransmittanceMeasure W/(m2K) 
    ///   HISTORY: New entity in IFC Release 1.0. The entity has been renamed from IfcSimpleProperty in IFC Release 2x
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertySingleValue : IfcSimpleProperty
    {
        #region Fields

        private IfcValue _nominalValue;
        private IfcUnit _unit;

        #endregion

        #region Constructors

        public IfcPropertySingleValue()
        {
        }

        public IfcPropertySingleValue(IfcIdentifier name)
            : base(name)
        {
        }

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional. Value and measure type of this property.
        /// </summary>
        /// <value>
        ///   NOTE  By virtue of the defined data type, that is selected from the SELECT IfcValue, the appropriate unit can be found within the IfcUnitAssignment, 
        ///   defined for the project if no value for the unit attribute is given.
        ///   IFC2x Edition 3 CHANGE  The attribute has been made optional with upward compatibility for file based exchange.
        /// </value>

        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcValue NominalValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _nominalValue;
            }
            set { this.SetModelValue(this, ref _nominalValue, value, v => NominalValue = v, "NominalValue"); }
        }

        /// <summary>
        ///   Optional. Unit for the nominal value, if not given, the default value for the measure type (given by the TYPE of nominal value) is used as defined by the global unit assignment at IfcProject.
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
                    _nominalValue = (IfcValue) value.EntityVal;
                    break;
                case 3:
                    _unit = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", _nominalValue == null ? "" : _nominalValue.ToString(),
                                 _unit == null ? "" : _unit.ToString());
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}