using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.Interfaces;


namespace Xbim.Ifc2x3.FacilitiesMgmtDomain
{
    /// <summary>
    /// An IfcActionRequest is a request for an action to fulfil a need.
    /// Requests may take many forms depending on the need including fault reports for maintenance, 
    /// requests for small works, purchase requests (where these are to be made through a help desk or buying function) etc.
    /// </summary>
    /// <remarks>
    /// Requests may take many forms depending on the need including fault reports for maintenance, requests for small works, 
    /// purchase requests (where these are to be made through a help desk or buying function) etc.
    /// A request may call for several actions and an action may refer to several requests. IfcRelAssignsToControl is used to 
    /// relate one or more requests to an action.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcActionRequest : IfcControl
    {

        #region Fields

        private IfcIdentifier _requestId;

        #endregion

        #region Properties

        /// <summary>
        /// A unique identifier assigned to the request on receipt.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier RequestId
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _requestId;
            }
            set { this.SetModelValue(this, ref _requestId, value, v => RequestId = v, "RequestId"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _requestId = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }


        public override string WhereRule()
        {
            return base.WhereRule();
        }

        //TODO: UNIQUE - UR2 : RequestID see http://www.steptools.com/support/stdev_docs/express/ifc2x3/html/t_ifcac-05.html
        #endregion
      

    }
}
