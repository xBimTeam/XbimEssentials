#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityVolume.cs
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
    ///   A physical quantity that defines a derived volume measure to provide an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A physical quantity that defines a derived volume measure to provide an element's physical property. It is normally derived from the physical properties of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: A thick brick wall may be measured according to its volume. The actual size of the volume depends on the method of measurement used.
    ///   HISTORY New entity in IFC Release 2.x. It replaces the calcXxx attributes used in previous IFC Releases.
    ///   EXPRESS specification:
    ///   Formal Propositions:
    ///   WR21   :   If a unit is given, the unit type shall be volume unit.  
    ///   WR22   :   A valid volume quantity shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityVolume : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcVolumeMeasure _volumeValue;

        #endregion

        /// <summary>
        ///   Volume measure value of this quantity.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcVolumeMeasure VolumeValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _volumeValue;
            }
            set { this.SetModelValue(this, ref _volumeValue, value, v => VolumeValue = v, "VolumeValue"); }
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
                    _volumeValue = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (Unit != null && Unit.UnitType != IfcUnitEnum.VOLUMEUNIT)
                baseErr += "WR21 QuantityVolume : If a unit is given, the unit type shall be volume unit.\n";
            if (_volumeValue < 0)
                baseErr += "WR22 QuantityVolume : A valid volume quantity shall be greater than or equal to zero.\n";
            return baseErr;
        }

        public override string ToString()
        {
            return VolumeValue.ToPart21;
        }
    }
}