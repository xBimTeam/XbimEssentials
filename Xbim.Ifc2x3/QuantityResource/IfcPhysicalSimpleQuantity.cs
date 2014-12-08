#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPhysicalSimpleQuantity.cs
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
    ///   The physical quantity, IfcPhysicalSimpleQuantity, is an entity that holds a single quantity measure value (as defined at the subtypes of IfcPhysicalSimpleQuantity) together with a semantic definition of the usage for the measure value.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The physical quantity, IfcPhysicalSimpleQuantity, is an entity that holds a single quantity measure value (as defined at the subtypes of IfcPhysicalSimpleQuantity) together with a semantic definition of the usage for the measure value.
    ///   EXAMPLE: An element, like a wall, may have several area measures, like footprint area, left wall face area, right wall face area. These areas would be given by three instances of the area quantity subtype, with different Name string values.
    ///   A section "Quantity Use Definition" at individual entities as subtypes of IfcBuildingElement gives guidance to the usage of the Name attribute to characterize the individual quantities. If the Unit attribute is given, the value attribute (introduced at the level of subtypes of IfcPhysicalSimpleQuantity) are given as quantities of this unit, otherwise the global unit definitions (given by IfcUnitAssignment) are used.
    ///   HISTORY New entity in IFC Release 2x2 Addendum 1. 
    ///   IFC2x2 ADDENDUM 1 CHANGE The abstract entity IfcPhysicalSimpleQuantity has been added. Upward compatibility for file based exchange is guaranteed.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPhysicalSimpleQuantity : IfcPhysicalQuantity
    {
        #region Fields

        private IfcNamedUnit _unit;

        #endregion

        /// <summary>
        ///   Optional assignment of a unit. If no unit is given, then the global unit assignment, as established at the IfcProject, applies to the quantity measures.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcNamedUnit Unit
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
                    _unit = (IfcNamedUnit) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}