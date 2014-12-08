using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProcessExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcOrderAction is the point at which requests for work are received and processed within an organization.
    /// </summary>
    /// <remarks>
    /// The IfcOrderAction represents tasks that might be carried out by a Helpdesk acting the role of interface for 
    /// the organization between the facility user and the functional requirement of fulfilling their needs. The actual 
    /// task represented by the IfcOrderAction class is turning a request into an order and initiating the action that 
    /// will enable the order to be completed.
    /// IfcRelAssignsToControl is used to relate one or more instances of IfcOrderAction to an IfcProjectOrder or one 
    /// of its subtypes including maintenance work order.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcOrderAction : IfcTask
    {

        #region Fields

        private IfcIdentifier _actionID;

        #endregion

        #region Properties

        /// <summary>
        /// A unique identifier assigned to an action on issue.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcIdentifier ActionID
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _actionID;
            }
            set { this.SetModelValue(this, ref _actionID, value, v => ActionID = v, "ActionID"); }
        }

        #endregion

        #region Methods
        
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
                case 6:
                case 7:
                case 8:
                case 9:
                    base.IfcParse(propIndex, value);
                    break;
                case 10:
                    _actionID = value.StringVal;
                    break;  
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        //TODO: UR2 : ActionID - see http://www.buildingsmart-tech.org/ifc/ifc2x3/tc1/html/ifcfacilitiesmgmtdomain/lexical/ifcorderaction.htm

        public override string WhereRule()
        {
            return base.WhereRule();
        }

        #endregion
      
    }
}
