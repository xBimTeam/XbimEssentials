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
    /// An IfcPermit is a document that allows permission to carry out actions in places and on 
    /// artefacts where security or other access restrictions apply.
    /// </summary>
    /// <remarks>
    /// The permit will identify the restrictions that apply and when access may be gained to carry out the actions.
    /// IfcRelAssignsToControl is used to identify related spaces, assets etc. upon which actions are permitted to take place.
    /// </remarks>
    [IfcPersistedEntity]
    public class IfcPermit : IfcControl
    {

        #region Fields

        private IfcIdentifier _permitID;

        #endregion

        #region Properties

        /// <summary>
        /// A unique identifier assigned to a permit.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier PermitID
        {
            get
            {
#if SupportActivation
                ((IPersistIfcEntity)this).Activate(false);
#endif
                return _permitID;
            }
            set { this.SetModelValue(this, ref _permitID, value, v => PermitID = v, "PermitID"); }
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
                    _permitID = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        //TODO: UNIQUE  UR2	 : 	PermitID; see http://www.buildingsmart-tech.org/ifc/IFC2x4/alpha/html/ifcfacilitiesmgmtdomain/lexical/ifcpermit.htm

        public override string WhereRule()
        {
            return base.WhereRule();
        }

        #endregion
      
    }
}
