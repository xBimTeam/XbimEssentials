#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityWeight.cs
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
    ///   A physical element quantity that defines a derived weight measure to provide an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A physical element quantity that defines a derived weight measure to provide an element's physical property. It is normally derived from the physical properties of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: The amount of reinforcement used within a building element may be measured according to its weight. The actual size of the weight depends on the method of measurement used.
    ///   HISTORY New entity in IFC Release 2.x. It replaces the calcXxx attributes used in previous IFC Releases.
    ///   Formal Propositions:
    ///   WR21   :   If a unit is given, the unit type shall be mass unit. NOTE: There is no distinction between the concept of "Mass" and "Weight" in the current IFC Release.  
    ///   WR22   :   A valid weight quantity shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityWeight : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcMassMeasure _weightValue;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcMassMeasure WeightValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _weightValue;
            }
            set { this.SetModelValue(this, ref _weightValue, value, v => WeightValue = v, "WeightValue"); }
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
                    _weightValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Unit != null && Unit.UnitType != IfcUnitEnum.MASSUNIT)
                baseErr += "WR21 QuantityWeight : If a unit is given, the unit type shall be mass unit.\n";
            if (WeightValue < 0)
                baseErr += "WR22 QuantityWeight : A valid mass quantity shall be greater than or equal to zero.\n";
            return baseErr;
        }

        public override string ToString()
        {
            return WeightValue.ToPart21;
        }
    }
}