#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConversionBasedUnit.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    /// <summary>
    ///   A conversion based unit is a unit that is defined based on a measure with unit. 
    ///   NOTE Corresponding STEP name: conversion_based_unit, please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard. 
    ///   HISTORY New entity in IFC Release 1.5.1. 
    ///   An inch is a converted unit. It is from the Imperial system, its name is "inch" and it can be related to the si unit, millimetre, through a measure with unit whose value is 25.4 millimetre. A foot is also a converted unit. It is from the Imperial system, its name is "foot" and it can be related to an si unit, millimetre, either directly or through the unit called "inch".
    ///   To identify some commonly used conversion based units the standard designations (case insensitive) for the Name attribute include the following: Name Description 
    ///   'inch' Length measure equal to 25.4 mm 
    ///   'foot' Length measure equal to 30.48 mm 
    ///   'yard' Length measure equal to 914 mm 
    ///   'mile' Length measure equal to 1609 m 
    ///   'acre' Area measure equal to 4046,86 square meters 
    ///   'litre' Volume measure equal to 0.001 cubic meters 
    ///   'pint UK' Volume measure equal to 0.000568 cubic meters 
    ///   'pint US' Volume measure equal to 0.000473 cubic meters 
    ///   'gallon UK' Volume measure equal to 0.004546 cubic meters 
    ///   'gallon US' Volume measure equal to 0.003785 cubic meters 
    ///   'ounce' Weight measure equal to 28.35 g 
    ///   'pound' Weight measure equal to 0.454 kg
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcConversionBasedUnit : IfcNamedUnit
    {
        #region Fields

        private IfcLabel _name;
        private IfcMeasureWithUnit _conversionFactor;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The word, or group of words, by which the conversion based unit is referred to.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcLabel Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }


        /// <summary>
        ///   The physical quantity from which the converted unit is derived.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcMeasureWithUnit ConversionFactor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _conversionFactor;
            }
            set
            {
                this.SetModelValue(this, ref _conversionFactor, value, v => ConversionFactor = v,
                                           "ConversionFactor");
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
                    _name = value.StringVal;
                    break;
                case 3:
                    _conversionFactor = (IfcMeasureWithUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}