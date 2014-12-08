using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcProjectOrder sets common properties for project orders issued in a construction or facilities management project.
    /// </summary>
    public class IfcProjectOrder : IfcControl
    {
        #region Fields

        private IfcIdentifier _ID;
        private IfcProjectOrderTypeEnum _PredefinedType;
        private IfcLabel _Status; //optional

        #endregion

        /// <summary>
        /// A unique identification assigned to a project order that enables its differentiation from other project orders.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcIdentifier ID
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ID;
            }
            set { this.SetModelValue(this, ref _ID, value, v => ID = v, "ID"); }
        }

        /// <summary>
        /// The type of project order.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcProjectOrderTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PredefinedType;
            }
            set { this.SetModelValue(this, ref _PredefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        /// <summary>
        /// The current status of a project order.Examples of status values that might be used for a project order status include: - PLANNED - REQUESTED - APPROVED - ISSUED - STARTED - DELAYED - DONE         
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcLabel Status
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Status;
            }
            set { this.SetModelValue(this, ref _Status, value, v => Status = v, "Status"); }
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
                    _ID = (IfcIdentifier)value.EntityVal;
                    break;
                case 6:
                    _PredefinedType = (IfcProjectOrderTypeEnum)Enum.Parse(typeof(IfcProjectOrderTypeEnum), value.EnumVal);
                    break;
                case 7:
                    _Status = (IfcLabel)value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        //TODO: UNIQUE
    }
}
