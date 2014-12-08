using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.Transactions.Extensions;

namespace Xbim.Ifc2x3.SharedMgmtElements
{
    /// <summary>
    /// An IfcProjectOrderRecord records information in sequence about the incidence of each order that is connected with one or a set of objects.
    /// </summary>
    public class IfcProjectOrderRecord : IfcControl
    {
        public IfcProjectOrderRecord()
        {
            _Records = new XbimSet<IfcRelAssignsToProjectOrder>(this);
        }

        #region Fields

        private XbimSet<IfcRelAssignsToProjectOrder> _Records;
        private IfcProjectOrderRecordTypeEnum _PredefinedType;
        

        #endregion

        /// <summary>
        /// Records in the sequence of occurrence the incident of a project order and the objects that are related to that project order. For instance, a maintenance incident will connect a work order with the objects (elements or assets) that are subject to the provisions of the work order
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public XbimSet<IfcRelAssignsToProjectOrder> Records
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                if (_Records == null)
                    _Records = new XbimSet<IfcRelAssignsToProjectOrder>(this);
                return _Records;
            }
            set { this.SetModelValue(this, ref _Records, value, v => Records = v, "Records"); }
        }

        /// <summary>
        /// Identifies the type of project incident.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcProjectOrderRecordTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _PredefinedType;
            }
            set { this.SetModelValue(this, ref _PredefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                    _Records = (XbimSet<IfcRelAssignsToProjectOrder>)value.EntityVal;
                    break;
                case 6:
                    _PredefinedType = (IfcProjectOrderRecordTypeEnum)Enum.Parse(typeof(IfcProjectOrderRecordTypeEnum), value.EnumVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public void SetRecords(IEnumerable<IfcRelAssignsToProjectOrder> records)
        {
            if (_Records == null)
                _Records = new XbimSet<IfcRelAssignsToProjectOrder>(this);
            else
                _Records.Clear_Reversible();
            foreach (IfcRelAssignsToProjectOrder item in records)
            {
                _Records.Add_Reversible(item);
            }
        }
    }
}
