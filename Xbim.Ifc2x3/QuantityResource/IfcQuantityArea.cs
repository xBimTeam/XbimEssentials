#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityArea.cs
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
    ///   physical quantity, IfcQuantityArea, that defines a derived area measure to provide an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A physical quantity, IfcQuantityArea, that defines a derived area measure to provide an element's physical property. It is normally derived from the physical properties of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: An opening may have an opening area used to deduct it from the wall surface area. The actual size of the area depends on the method of measurement used.
    ///   HISTORY New entity in IFC Release 2.x. It replaces the calcXxx attributes used in previous IFC Releases.
    ///   Formal Propositions:
    ///   WR21   :   If a unit is given, the unit type shall be area unit.  
    ///   WR22   :   A valid area quantity shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityArea : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcAreaMeasure _areaValue;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure AreaValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _areaValue;
            }
            set { this.SetModelValue(this, ref _areaValue, value, v => AreaValue = v, "AreaValue"); }
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
                    _areaValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Unit != null && Unit.UnitType != IfcUnitEnum.AREAUNIT)
                baseErr += "WR21 QuantityArea : If a unit is given, the unit type shall be area unit.\n";
            if (AreaValue < 0)
                baseErr += "WR22 QuantityArea : A valid area quantity shall be greater than or equal to zero.\n";
            return baseErr;
        }
        
        public override string ToString()
        {
            return AreaValue.ToPart21;
        }
    }
}