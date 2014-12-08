#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignsToControl.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship (IfcRelAssignsToControl) handles the assignment of a control (subtype of IfcControl) to other objects (subtypes of IfcObject, with the exception of controls). 
    ///   For example: The assignment of a cost (as subtype of IfcControl) to a building element (as subtype of IfcObject) is an application of this generic relationship.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssignsToControl : IfcRelAssigns
    {
        #region Fields

        private IfcControl _relatingControl;

        #endregion

        /// <summary>
        ///   Reference to group that finally contains all assigned group members.
        /// </summary>
        /// <remarks>
        ///   WR1   :   The instance to with the relation points shall not be contained in the List of RelatedObjects.
        /// </remarks>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcControl RelatingControl
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingControl;
            }
            set
            {
                this.SetModelValue(this, ref _relatingControl, value, v => RelatingControl = v,
                                           "RelatingControl");
            }
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
                    _relatingControl = (IfcControl) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}