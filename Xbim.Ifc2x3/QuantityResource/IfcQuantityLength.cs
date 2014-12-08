#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityLength.cs
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
    ///   A physical quantity, IfcQuantityLength, that defines a derived length measure to provide an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A physical quantity, IfcQuantityLength, that defines a derived length measure to provide an element's physical property. It is normally derived from the physical properties of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: A rafter within a roof construction may be measured according to its length (taking a common cross section into account). The actual size of the length depends on the method of measurement used.
    ///   HISTORY New entity in IFC Release 2.x . It replaces the calcXxx attributes used in previous IFC Releases.
    ///   Formal Propositions:
    ///   WR21   :   If a unit is given, the unit type shall be a lenght unit.  
    ///   WR22   :   A valid length quantity shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityLength : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcLengthMeasure _lengthValue;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcLengthMeasure LengthValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _lengthValue;
            }
            set { this.SetModelValue(this, ref _lengthValue, value, v => LengthValue = v, "LengthValue"); }
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
                    _lengthValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Unit != null && Unit.UnitType != IfcUnitEnum.LENGTHUNIT)
                baseErr += "WR21 QuantityLength : If a unit is given, the unit type shall be length unit.\n";
            if (LengthValue < 0)
                baseErr += "WR22 QuantityLength : A valid length quantity shall be greater than or equal to zero.\n";
            return baseErr;
        }

        public override string ToString()
        {
            return LengthValue.ToPart21;
        }
    }
}