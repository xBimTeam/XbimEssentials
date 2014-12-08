#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcQuantityCount.cs
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
    ///   An physical quantity, IfcQuantityCount, that defines a derived count measure to provide an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An physical quantity, IfcQuantityCount, that defines a derived count measure to provide an element's physical property. It is normally derived from the physical properties of the element under the specific measure rules given by a method of measurement.
    ///   EXAMPLE: An radiator may be measured according to its number of coils. The actual counting method depends on the method of measurement used.
    ///   HISTORY New entity in IFC Release 2.x . It replaces the calcXxx attributes used in previous IFC Releases
    ///   Formal Propositions:
    ///   WR21   :   The value of the count shall be greater than or equal to zero.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcQuantityCount : IfcPhysicalSimpleQuantity
    {
        #region Fields

        private IfcCountMeasure _countValue;

        #endregion

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcCountMeasure CountValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _countValue;
            }
            set { this.SetModelValue(this, ref _countValue, value, v => CountValue = v, "CountValue"); }
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
                    _countValue = value.NumberVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (CountValue < 0)
                baseErr += "WR21 QuantityCount : The value of the count shall be greater than or equal to zero.\n";
            return baseErr;
        }

        public override string ToString()
        {
            return CountValue.ToPart21;
        }
    }
}