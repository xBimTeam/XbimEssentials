#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesApproval.cs
// Published:   05, 2012
// Last Edited: 15:00 PM on 23 05 2012
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ApprovalResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ControlExtension
{
    /// <summary>
    /// The entity IfcRelAssociatesApproval is used to apply approval information defined by IfcApproval, 
    /// in IfcApprovalResource schema, to all subtypes of IfcRoot.
    /// </summary>
    /// <remarks>
    /// Definition from IAI: The entity IfcRelAssociatesApproval is used to apply approval information defined by IfcApproval, 
    /// in IfcApprovalResource schema, to all subtypes of IfcRoot.
    /// NOTE: This entity replaces the IfcApprovalUsage in IFC2x
    /// HISTORY: New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesApproval : IfcRelAssociates
    {
        #region Fields
        IfcApproval _relatingApproval;
        #endregion

        #region Ifc Properties
        /// <summary>
        /// Reference to approval that is being applied using this relationship.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcApproval RelatingApproval
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _relatingApproval;
            }
            set { this.SetModelValue(this, ref _relatingApproval, value, v => RelatingApproval = v, "RelatingApproval"); }
        }

        #endregion

        #region IfcParse

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
                    _relatingApproval = (IfcApproval)value.EntityVal; 
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
        #endregion

    }
}
