#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionControlElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcDistributionControlElement : IfcDistributionElement
    {
        #region fields

        private IfcIdentifier? _controlElementId;

        #endregion

        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcIdentifier? ControlElementId
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _controlElementId;
            }
            set
            {
                this.SetModelValue(this, ref _controlElementId, value, v => ControlElementId = v,
                                           "ControlElementId");
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
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _controlElementId = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 0, 1)]
        public IEnumerable<IfcRelFlowControlElements> AssignedToFlowElement
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcRelFlowControlElements>(
                        r => (r.RelatedControlElements.Contains(this)));
            }
        }
    }
}