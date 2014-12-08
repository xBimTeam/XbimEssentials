using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcEquipmentStandard is a standard for equipment allocation that can be assigned to persons within an organization.
    /// </summary>
    /// <remarks>
    /// An IfcEquipmentStandard is assigned a set of classification notations (through the IfcRelAssociatesClassification class within the IfcKernel schema) 
    /// that determine the types of equipment that fulfill the requirements of the standard. In order to use the equipment standard class, 
    /// a classification of equipment items must have been established. This does not mean that each individual equipment item needs to have a classification 
    /// notation although this is considered to be desirable. Examples of equipment items that might fall within an equipment standard include number and type 
    /// PC's and connections, task lighting, pictures etc.
    /// An equipment standard is assigned to one or several persons or organizations (like a work group or department) through the IfcRelAssignsToControl 
    /// relationship via the Controls inverse attribute.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcEquipmentStandard : IfcControl
    {
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
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            return base.WhereRule();
        }
    }
}
