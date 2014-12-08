#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionPort.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcDistributionPort : IfcPort
    {
        #region fields

        private IfcFlowDirectionEnum _flowDirection;

        #endregion

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcFlowDirectionEnum FlowDirection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flowDirection;
            }
            set { this.SetModelValue(this, ref _flowDirection, value, v => FlowDirection = v, "FlowDirection"); }
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
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _flowDirection =
                        (IfcFlowDirectionEnum) Enum.Parse(typeof (IfcFlowDirectionEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}