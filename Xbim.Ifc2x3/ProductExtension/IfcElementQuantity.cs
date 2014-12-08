#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementQuantity.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An element quantity (IfcElementQuantity) defines a set of derived measures of an element's physical property.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An element quantity (IfcElementQuantity) defines a set of derived measures of an element's physical property. Elements could be spatial structure elements (like buildings, storeys, or spaces) or elements (like building elements or opening elements). It gets assigned to the element by using the IfcRelDefinesByProperties relationship.
    ///   The MethodOfMeasurement attribute defines the building code, which had been used to calculate the element quantity. The interpretation of the attribute value has to be agreed upon.
    ///   NOTE The recognizable values for the name and the method of measurement attributes have to be agreed upon in further agreement documents, such as implementers agreements. Some of these agreements might be limited to a certain region, to which the method of measurement applies.
    ///   The name attribute, given at the individual Quantities provides a recognizable semantic meaning of the element quantity. Both information is needed to establish a precise meaning for the measure value. An optional description may be assigned to each of the Quantities. All quantities assigned by asingle instance of IfcElementQuantity are deemed to have be generated according to the same method of measurement. However several instances of IfcElementQuantity are assignable to an element, thus allowing for an element having quatities generated according to several methods of measurements.
    ///   EXAMPLE 1: To exchange the net floor area of spaces in the German region (as IfcSpace), the name might be 'HNF1' (area of main function type 1), and the method of measurement might be accordingly 'DIN277' (German industry norm no. 277)
    ///   EXAMPLE 2: The same instance of IfcSpace may have a different area measure assigned in the German region according to a housing regulation, the name would be 'Wohnfläche' and the method of measurement would be '2.BV'. It would be attached to the IfcSpace by a separate IfcRelDefinesByProperties relationship.
    ///   The IfcElementQuantity can have the following subtypes of IfcPhysicalQuantity within its SET Quantities, which count for the basis measure types used:
    ///   count measure 
    ///   weight measure 
    ///   length measure 
    ///   area measure 
    ///   volume measure 
    ///   time measure 
    ///   HISTORY New entity in IFC Release 2.x. NOTE: It replaces the calcXxx attributes used in previous IFC Releases.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcElementQuantity : IfcPropertySetDefinition
    {
        public IfcElementQuantity()
        {
            _quantities = new PhysicalQuantitySet(this);
        }

        #region Fields

        private IfcLabel? _methodOfMeasurement;

        private PhysicalQuantitySet _quantities;

        #endregion

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel? MethodOfMeasurement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _methodOfMeasurement;
            }
            set
            {
                this.SetModelValue(this, ref _methodOfMeasurement, value, v => MethodOfMeasurement = v,
                                           "MethodOfMeasurement");
            }
        }

        [IfcAttribute(6, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public PhysicalQuantitySet Quantities
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _quantities;
            }
            set { this.SetModelValue(this, ref _quantities, value, v => Quantities = v, "Quantities"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _methodOfMeasurement = value.StringVal;
                    break;
                case 5:
                    _quantities.Add((IfcPhysicalQuantity) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            return "";
        }
    }
}