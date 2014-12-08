#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCostValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   An IfcCostValue is an amount of money or a value that affects an amount of money.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI:An IfcCostValue is an amount of money or a value that affects an amount of money.
    ///   HISTORY: New Entity in IFC Release 1.0 
    ///   Use Definitions
    ///   Each instance of IfcCostValue may also have a CostType. There are many possible types of cost value that may be identified. Whilst there is a broad understanding of the meaning of names that may be assigned to different types of costs, there is no general standard for naming cost types nor are there any broadly defined classifications. To allow for any type of cost value, the IfcLabel datatype is assigned. The following defines some cost types that might be applied: 
    ///   Annual rate of return Lease Replacement 
    ///   Bonus List price Sale 
    ///   Bulk purchase rebate Maintenance Small quantity surcharge 
    ///   Contract Material Spares 
    ///   Consultancy Overhead Storage 
    ///   Delivery Postage and packing Sub-Contract 
    ///   Estimated cost Profit Trade discount 
    ///   Hire Purchase Transportation 
    ///   Installation Rental Waste allowance 
    ///   Interest rate Repair Whole life 
    ///   Labor   
    ///   In the absence of any well-defined standard, it is recommended that local agreements should be made to define allowable and understandable cost value types within a project or region
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcCostValue : IfcAppliedValue, IfcMetricValueSelect
    {
        #region Fields

        private IfcLabel _costType;
        private IfcText _condition;

        #endregion

        /// <summary>
        ///   Specification of the type of cost type used.
        /// </summary>
        /// <remarks>
        ///   NOTE: There are many possible types of cost value that may be identified. Whilst there is a broad understanding of the meaning of names that may be assigned to different types of costs, there is no general standard for naming cost types nor are there any broadly defined classifications. To allow for any type of cost value, the IfcLabel datatype is assigned.
        ///   In the absence of any well defined standard, it is recommended that local agreements should be made to define allowable and understandable cost value types within a project or region.
        /// </remarks>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcLabel CostType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _costType;
            }
            set { this.SetModelValue(this, ref _costType, value, v => CostType = v, "CostType"); }
        }

        /// <summary>
        ///   Optional. The condition under which a cost value applies.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcText Condition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _condition;
            }
            set { this.SetModelValue(this, ref _condition, value, v => Condition = v, "Condition"); }
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
                    _costType = value.StringVal;
                    break;
                case 7:
                    _condition = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}