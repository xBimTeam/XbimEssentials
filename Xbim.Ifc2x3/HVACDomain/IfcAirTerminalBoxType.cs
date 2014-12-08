#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAirTerminalBoxType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.SharedBldgServiceElements;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcAirTerminalBoxType : IfcFlowControllerType
    {
        #region Fields

        private IfcAirTerminalBoxTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(10, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcAirTerminalBoxTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _predefinedType =
                        (IfcAirTerminalBoxTypeEnum) Enum.Parse(typeof (IfcAirTerminalBoxTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}