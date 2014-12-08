#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEnvironmentalImpactValue.cs
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

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcEnvironmentalImpactValue is an amount or measure of an environmental impact or a value that affects an amount or measure of an environmental impact.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcEnvironmentalImpactValue is an amount or measure of an environmental impact or a value that affects an amount or measure of an environmental impact.
    ///   HISTORY: New entity in IFC 2x2
    ///   Use Definitions
    ///   The IfcEnvironmentalImpactValue contains the value of the environmental impact. For example this could represent the volume of carbon dioxide emission, amount of operational energy or mass of aluminium used in a product.
    ///   Each instance of IfcEnvironmentalImpactValue may also have an ImpactType. There are many possible types of environmental impact value that may be identified. To allow for any type of environmental impact value, the IfcLabel datatype is assigned. The following defines some impact types that might be applied: 
    ///   CO2 emission 
    ///   Embodied energy 
    ///   Mass of aluminium 
    ///   Operational energy 
    ///   Resource 
    ///   Water pollution 
    ///   Where a formal standard is not used, it is recommended that local agreements should be made to define allowable and understandable impact value types within a project or region.
    ///   Formal Propositions:
    ///   WR1   :   The attribute UserDefinedCategory must be asserted when the value of the IfcEnvironmentalImpactCategoryEnum is set to USERDEFINED.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcEnvironmentalImpactValue : IfcAppliedValue
    {
        #region Fields

        private IfcLabel _impactType;
        private IfcEnvironmentalImpactCategoryEnum _category;
        private IfcLabel _userDefinedCategory;

        #endregion

        /// <summary>
        ///   Specification of the environmental impact type to be referenced.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcLabel ImpactType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _impactType;
            }
            set { this.SetModelValue(this, ref _impactType, value, v => ImpactType = v, "ImpactType"); }
        }

        /// <summary>
        ///   The category into which the environmental impact value falls.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcEnvironmentalImpactCategoryEnum Category
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _category;
            }
            set { this.SetModelValue(this, ref _category, value, v => Category = v, "Category"); }
        }

        /// <summary>
        ///   Optional. A user defined value category into which the environmental impact value falls.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel UserDefinedCategory
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedCategory;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedCategory, value, v => UserDefinedCategory = v,
                                           "UserDefinedCategory");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _impactType = value.StringVal;
                    break;
                case 7:
                    _category =
                        (IfcEnvironmentalImpactCategoryEnum)
                        Enum.Parse(typeof (IfcEnvironmentalImpactCategoryEnum), value.EnumVal, true);
                    break;
                case 8:
                    _userDefinedCategory = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}