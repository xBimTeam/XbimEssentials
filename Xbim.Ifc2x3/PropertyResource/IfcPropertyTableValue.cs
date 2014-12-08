#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyTableValue.cs
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
    ///   A property with a range value (IfcPropertyTableValue) defines a property object which has two lists of (numeric or descriptive) values assigned, the values specifying a table with two columns.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A property with a range value (IfcPropertyTableValue) defines a property object which has two lists of (numeric or descriptive) values assigned, the values specifying a table with two columns. The defining values provide the first column and establish the scope for the defined values (the second column). Interpolations are out of scope of the IfcPropertyTableValue. An optional Expression attribute may give the equation used for deriving the range value, which is for information purposes only.
    ///   The IfcPropertyTableValue defines a defining/defined property value combination for which the property name, the table with defining and defined values with measure type (and optional the units for defining and defined values)are given.
    ///   The units are handled by the DefiningUnit and DefinedUnit attributes:
    ///   If the DefiningUnit or DefinedUnit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the DefiningUnit or DefinedUnit attribute is given, then the unit assigned by the unit attribute overrides the globally assigned unit. 
    ///   The IfcPropertyTableValue allows for the specification of a table of defining/defined value pairs of the property description.
    ///   Examples of a property with range value are:
    ///   Name DefiningValues DefiningValue Type 
    ///   (through IfcValue) DefinedValues DefinedValue Type 
    ///   (through IfcValue) DefingUnit  DefinedUnit 
    ///   SoundTransmissionLoss 100 IfcFrequencyMeasure 20 IfcNumericMeasure -  dB 
    ///   200	 IfcFrequencyMeasure 	42 	IfcNumericMeasure     
    ///   400	 IfcFrequencyMeasure 	46 	IfcNumericMeasure     
    ///   800	 IfcFrequencyMeasure 	56 	IfcNumericMeasure     
    ///   1600	 IfcFrequencyMeasure 	60 	IfcNumericMeasure     
    ///   3200 	IfcFrequencyMeasure 	65 	IfcNumericMeasure     
    ///   HISTORY: New entity in IFC Release 2x. 
    ///   Informal propositions:
    ///   The list of DerivedValues and the list of DefiningValues are corresponding lists. 
    ///   Formal Propositions:
    ///   WR1   :   The number of members in the list of defining values shall be the same as the number of members in the list of defined values.  
    ///   WR2   :   All values within the list of defining values shall have the same measure type.  
    ///   WR3   :   All values within the list of defined values shall have the same measure type.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertyTableValue : IfcSimpleProperty
    {
        public IfcPropertyTableValue()
        {
            _definingValues = new XbimList<IfcValue>(this);
            _definedValues = new XbimList<IfcValue>(this);
        }

        #region Fields

        private XbimList<IfcValue> _definingValues;
        private XbimList<IfcValue> _definedValues;
        private IfcText? _expression;
        private IfcUnit _definingUnit;
        private IfcUnit _definedUnit;

        #endregion

        /// <summary>
        ///   List of defining values, which determine the defined values.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcValue> DefiningValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _definingValues;
            }
            set { this.SetModelValue(this, ref _definingValues, value, v => DefiningValues = v, "DefiningValues"); }
        }

        /// <summary>
        ///   Defined values which are applicable for the scope as defined by the defining values.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1)]
        public XbimList<IfcValue> DefinedValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _definedValues;
            }
            set { this.SetModelValue(this, ref _definedValues, value, v => DefinedValues = v, "DefinedValues"); }
        }

        /// <summary>
        ///   Optional. Expression for the derivation of defined values from the defining values, the expression is given for information only, i.e. no automatic processing can be expected from the expression.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcText? Expression
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _expression;
            }
            set { this.SetModelValue(this, ref _expression, value, v => Expression = v, "Expression"); }
        }

        /// <summary>
        ///   Optional. Unit for the defining values, if not given, the default value for the measure type (given by the TYPE of the defining values) is used as defined by the global unit assignment at IfcProject.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcUnit DefiningUnit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _definingUnit;
            }
            set { this.SetModelValue(this, ref _definingUnit, value, v => DefiningUnit = v, "DefiningUnit"); }
        }

        /// <summary>
        ///   Optional. Unit for the defined values, if not given, the default value for the measure type (given by the TYPE of the defined values) is used as defined by the global unit assignment at IfcProject.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcUnit DefinedUnit
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _definedUnit;
            }
            set { this.SetModelValue(this, ref _definedUnit, value, v => DefinedUnit = v, "DefinedUnit"); }
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
                    _definingValues.Add((IfcValue) value.EntityVal);
                    break;
                case 3:
                    _definedValues.Add((IfcValue)value.EntityVal);
                    break;
                case 4:
                    _expression = value.StringVal;
                    break;
                case 5:
                    _definingUnit = (IfcUnit) value.EntityVal;
                    break;
                case 6:
                    _definedUnit = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string err = "";
            if (_definedValues.Count != _definedValues.Count)
                err +=
                    "WR1 PropertyTableValue : The number of members in the list of defining values shall be the same as the number of members in the list of defined values.\n";
            return err;
        }
    }
}