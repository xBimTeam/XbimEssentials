#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPhysicalComplexQuantity.cs
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
    ///   The complex physical quantity, IfcPhysicalComplexQuantity, is an entity that holds a set of single quantity measure value (as defined at the subtypes of IfcPhysicalSimpleQuantity), that all apply to a given component or aspect of the element.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The complex physical quantity, IfcPhysicalComplexQuantity, is an entity that holds a set of single quantity measure value (as defined at the subtypes of IfcPhysicalSimpleQuantity), that all apply to a given component or aspect of the element.
    ///   EXAMPLE: A layered element, like a wall, may have several material layers, each having individual quantities, like footprint area, side area and volume. An instance of IfcPhysicalComplexQuantity would group these individual quantities (given by a subtype of IfcPhysicalSimpleQuantity) and name them according to the material layer name by using the Name attribute. The Discrimination attribute would then be 'layer'.
    ///   A section "Quantity Use Definition" at individual entities as subtypes of IfcBuildingElement gives guidance to the usage of the Name and Discrimination attribute to characterize the complex quantities.
    ///   HISTORY New entity in IFC Release 2x2 Addendum 1. 
    ///   IFC2x2 ADDENDUM 1 CHANGE The entity IfcPhysicalComplexQuantity has been added. Upward compatibility for file based exchange is guaranteed.
    ///   Formal Propositions:
    ///   WR21   :   The IfcPhysicalComplexQuantity should not reference itself within the list of HasQuantities.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPhysicalComplexQuantity : IfcPhysicalQuantity
    {
        public IfcPhysicalComplexQuantity()
        {
            _hasQuantities = new XbimSet<IfcPhysicalQuantity>(this);
        }

        #region Fields

        private XbimSet<IfcPhysicalQuantity> _hasQuantities;
        private IfcLabel? _discrimination;
        private IfcLabel? _quality;
        private IfcLabel? _usage;

        #endregion

        /// <summary>
        ///   Set of physical quantities that are grouped by this complex physical quantity according to a given discrimination.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcPhysicalQuantity> HasQuantities
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _hasQuantities;
            }
            set { this.SetModelValue(this, ref _hasQuantities, value, v => HasQuantities = v, "HasQuantities"); }
        }

        /// <summary>
        ///   Identification of the dicrimination by which this physical complex property is distinguished. Examples of discriminations are 'layer', 'steel bar diameter', etc.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLabel? Discrimination
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _discrimination;
            }
            set { this.SetModelValue(this, ref _discrimination, value, v => Discrimination = v, "Discrimination"); }
        }

        /// <summary>
        ///   Optional. Additional indication of a quality of the quantities that are grouped under this physical complex quantity.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLabel? Quality
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _quality;
            }
            set { this.SetModelValue(this, ref _quality, value, v => Quality = v, "Quality"); }
        }

        /// <summary>
        ///   Optional. Additional indication of a usage type of the quantities that are grouped under this physical complex quantity.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLabel? Usage
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _usage;
            }
            set { this.SetModelValue(this, ref _usage, value, v => Usage = v, "Usage"); }
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
                    _hasQuantities.Add((IfcPhysicalQuantity) value.EntityVal);
                    break;
                case 3:
                    _discrimination = value.StringVal;
                    break;
                case 4:
                    _quality = value.StringVal;
                    break;
                case 5:
                    _usage = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (HasQuantities.Contains(this))
                baseErr +=
                    "WR21 PhysicalComplexQuantity : The IfcPhysicalComplexQuantity should not reference itself within the list of HasQuantities.\n";
            return baseErr;
        }
    }
}