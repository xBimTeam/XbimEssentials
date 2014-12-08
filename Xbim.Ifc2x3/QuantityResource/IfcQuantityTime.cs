#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityTime.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.QuantityResource
{
    /// <summary>
    ///   An element quantity that defines a time measure to provide an property of time related to an element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An element quantity that defines a time measure to provide an property of time related to an element. It is normally given by the recipe information of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: The amount of time needed to pour concrete for a wall is given as a time quantity for the labour part of the recipe information.
    ///   HISTORY New entity in Release IFC 2x Edition 2
    ///   Formal Propositions:
    ///   WR21   :   If a unit is given, the unit type shall be time unit.  
    ///   WR22   :   A valid weight quantity shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityTime : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcTimeMeasure _timeValue;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcTimeMeasure TimeValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _timeValue;
            }
            set { this.SetModelValue(this, ref _timeValue, value, v => TimeValue = v, "TimeValue"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _timeValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Unit != null && Unit.UnitType != IfcUnitEnum.TIMEUNIT)
                baseErr += "WR21 QuantityTime : If a unit is given, the unit type shall be time unit.\n";
            if (TimeValue < 0)
                baseErr += "WR22 QuantityTime : A valid time quantity shall be greater than or equal to zero.\n";
            return baseErr;
        }

        public override string ToString()
        {
            return TimeValue.ToPart21;
        }
    }
}