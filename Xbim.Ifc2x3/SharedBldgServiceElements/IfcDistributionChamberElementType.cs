#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDistributionChamberElementType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcDistributionChamberElementType : IfcDistributionFlowElementType
    {
        #region Fields

        private IfcDistributionChamberElementTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(10, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcDistributionChamberElementTypeEnum PredefinedType
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
                        (IfcDistributionChamberElementTypeEnum)
                        Enum.Parse(typeof (IfcDistributionChamberElementTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}