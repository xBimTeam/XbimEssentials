using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.CostResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    ///  An IfcRelAssociatesAppliedValue is a subtype of IfcRelAssociates that enables the association of an instance of IfcAppliedValue with one or more instances of IfcObject.
    /// </summary>

    public class IfcRelAssociatesAppliedValue : IfcRelAssociates
    {
        private IfcAppliedValue _RelatingAppliedValue;

        /// <summary>
        /// Note that IfcRelAssociatesAppliedValue, when used for costing purposes, should be used only for relating specific cost values to objects and not for relating cost schedule items to cost schedule (for which purpose IfcRelSchedulesCostItems should be used).
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcAppliedValue RelatingAppliedValue
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RelatingAppliedValue;
            }
            set { this.SetModelValue(this, ref _RelatingAppliedValue, value, v => RelatingAppliedValue = v, "RelatingAppliedValue"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _RelatingAppliedValue = (IfcAppliedValue)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
