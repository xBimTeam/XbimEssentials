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
    /// An IfcFurnitureStandard is a standard for furniture allocation that can be assigned to persons within an organization.
    /// </summary>
    /// <remarks>
    /// A furniture standard is assigned a set of classification notations (through the IfcRelAssociatesClassification class within the IfcKernel schema) 
    /// that determine the types of furniture that fulfil the requirements of the standard. In order to use the IfcFurnitureStandard class, a classification 
    /// of furniture items must have been established. This does not mean that each individual furniture item needs to have a classification notation although 
    /// this is considered to be desirable. 
    /// A furniture standard is assigned to one or several persons or organisations (like a work group or department) through the IfcRelAssignsToControl 
    /// relationship via the Controls inverse attribute.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcFurnitureStandard : IfcControl
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
