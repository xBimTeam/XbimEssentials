#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyBoundedValue.cs
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
    ///   A property with a bounded value (IfcPropertyBoundedValue) defines a property object which has a maximum of two (numeric or descriptive) values assigned, the first value specifying the upper bound and the second value specifying the lower bound.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A property with a bounded value (IfcPropertyBoundedValue) defines a property object which has a maximum of two (numeric or descriptive) values assigned, the first value specifying the upper bound and the second value specifying the lower bound. It defines a property - value bound (min-max) combination for which the property name, the upper bound value with measure type, the lower bound value with measure type (and optional the unit) is given.
    ///   The unit is handled by the Unit attribute:
    ///   If the Unit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the Unit attribute is given, then the unit assigned by the Unit attribute overrides the globally assigned unit. 
    ///   The IfcPropertyBoundedValue allows for the specification of an interval for the value component of the property description. If either the LowerBoundValue or the UpperBoundValue is not given, then it indicates an open bound (either a minimum value or a maximum value). The interval is by definition inclusive, i.e. the value given for the LowerBoundValue or the UpperBoundValue is included in the interval.
    ///   Examples of a property with bounded value are:
    ///   Name UpperBoundValue LowerBoundValue Type 
    ///   (through IfcValue, WR1 ensures same type for both values) Unit 
    ///  
    ///   OverallHeight 1930 2300 IfcPositiveLengthMeasure -  
    ///   OverallWidth 0.9 1.25 IfcPositiveLengthMeasure m 
    ///   MaxHeight 20.0 nil IfcPositiveLengthMeasure - 
    ///   MinWeight nil 20 IfcMassMeasure kg 
    ///   HISTORY: New entity in IFC Release 2x. 
    ///   IFC2x PLATFORM CHANGE: The attribute type of the attribute UpperBoundValue and LowerBoundValue has been changed from mandatory to optional with upward compatibility for file based exchange.
    ///   Informal proposition:
    ///   If the measure type for the upper and lover bound value is a numeric measure, then the following shall be true: UpperBoundValue &gt; LowerBoundValue. 
    ///   Definition from IAI: A property with a bounded value (IfcPropertyBoundedValue) defines a property object which has a maximum of two (numeric or descriptive) values assigned, the first value specifying the upper bound and the second value specifying the lower bound. It defines a property - value bound (min-max) combination for which the property name, the upper bound value with measure type, the lower bound value with measure type (and optional the unit) is given.
    ///   The unit is handled by the Unit attribute:
    ///   If the Unit attribute is not given, then the unit is already implied by the type of IfcMeasureValue or IfcDerivedMeasureValue. The associated unit can be found at the IfcUnitAssignment globally defined at the project level (IfcProject.UnitsInContext). 
    ///   If the Unit attribute is given, then the unit assigned by the Unit attribute overrides the globally assigned unit. 
    ///   The IfcPropertyBoundedValue allows for the specification of an interval for the value component of the property description. If either the LowerBoundValue or the UpperBoundValue is not given, then it indicates an open bound (either a minimum value or a maximum value). The interval is by definition inclusive, i.e. the value given for the LowerBoundValue or the UpperBoundValue is included in the interval.
    ///   Examples of a property with bounded value are:
    ///   Name UpperBoundValue LowerBoundValue Type 
    ///   (through IfcValue, WR1 ensures same type for both values) Unit 
    ///  
    ///   OverallHeight 1930 2300 IfcPositiveLengthMeasure -  
    ///   OverallWidth 0.9 1.25 IfcPositiveLengthMeasure m 
    ///   MaxHeight 20.0 nil IfcPositiveLengthMeasure - 
    ///   MinWeight nil 20 IfcMassMeasure kg 
    ///   HISTORY: New entity in IFC Release 2x. 
    ///   IFC2x PLATFORM CHANGE: The attribute type of the attribute UpperBoundValue and LowerBoundValue has been changed from mandatory to optional with upward compatibility for file based exchange.
    ///   Informal proposition:
    ///   If the measure type for the upper and lover bound value is a numeric measure, then the following shall be true: UpperBoundValue &gt; LowerBoundValue.
    ///   Formal Propositions:
    ///   WR21   :   The measure type of the upper bound value shall be the same as the measure type of the lower bound value, if both (upper and lower bound) are given.  
    ///   WR22   :   Either the upper bound, or the lower bound, or both bounds shall be given.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertyBoundedValue : IfcSimpleProperty
    {
        #region Fields

        private IfcValue _upperBoundValue;
        private IfcValue _lowerBoundValue;
        private IfcUnit _unit;

        #endregion

        /// <summary>
        ///   Optional.   Upper bound value for the interval defining the property value. If the value is not given, it indicates an open bound (all values to be greater than or equal to LowerBoundValue).
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcValue UpperBoundValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _upperBoundValue;
            }
            set
            {
                this.SetModelValue(this, ref _upperBoundValue, value, v => UpperBoundValue = v,
                                           "UpperBoundValue");
            }
        }

        /// <summary>
        ///   Optional.   Lower bound value for the interval defining the property value. If the value is not given, it indicates an open bound (all values to be lower than or equal to UpperBoundValue).
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcValue LowerBoundValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _lowerBoundValue;
            }
            set
            {
                this.SetModelValue(this, ref _lowerBoundValue, value, v => LowerBoundValue = v,
                                           "LowerBoundValue");
            }
        }

        /// <summary>
        ///   Optional.  Unit for the upper and lower bound values, if not given, the default value for the measure type (given by the TYPE of the upper and lower bound values) is used as defined by the global unit assignment at IfcProject.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
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
                    _upperBoundValue = (IfcValue) value.EntityVal;
                    break;
                case 3:
                    _lowerBoundValue = (IfcValue) value.EntityVal;
                    break;
                case 4:
                    _unit = (IfcUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string err = "";
            if (_upperBoundValue != null && _lowerBoundValue != null &&
                _upperBoundValue.GetType() != _lowerBoundValue.GetType())
                err +=
                    "WR21 PropertyBoundedValue : The measure type of the upper bound value shall be the same as the measure type of the lower bound value, if both (upper and lower bound) are given.\n";
            if (_upperBoundValue == null && _lowerBoundValue == null)
                err +=
                    "WR22 PropertyBoundedValue : Either the upper bound, or the lower bound, or both bounds shall be given.\n";
            return err;
        }
    }
}