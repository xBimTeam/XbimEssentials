#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcReinforcingBar.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcReinforcingBar : IfcReinforcingElement
    {
        #region Fields

        private IfcPositiveLengthMeasure _nominalDiameter;
        private IfcAreaMeasure _crossSectionArea;
        private IfcPositiveLengthMeasure? _barLength;
        private IfcReinforcingBarRoleEnum _barRole;
        private IfcReinforcingBarSurfaceEnum? _barSurface;

        #endregion

        #region Properties

        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure NominalDiameter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _nominalDiameter;
            }
            set
            {
                this.SetModelValue(this, ref _nominalDiameter, value, v => NominalDiameter = v,
                                           "NominalDiameter");
            }
        }

        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcAreaMeasure CrossSectionArea
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _crossSectionArea;
            }
            set
            {
                this.SetModelValue(this, ref _crossSectionArea, value, v => CrossSectionArea = v,
                                           "CrossSectionArea");
            }
        }

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? BarLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _barLength;
            }
            set { this.SetModelValue(this, ref _barLength, value, v => BarLength = v, "BarLength"); }
        }

        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcReinforcingBarRoleEnum BarRole
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _barRole;
            }
            set { this.SetModelValue(this, ref _barRole, value, v => BarRole = v, "BarRole"); }
        }

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcReinforcingBarSurfaceEnum? BarSurface
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _barSurface;
            }
            set { this.SetModelValue(this, ref _barSurface, value, v => BarSurface = v, "BarSurface"); }
        }

        #endregion

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
                    _nominalDiameter = value.RealVal;
                    break;
                case 10:
                    _crossSectionArea = value.RealVal;
                    break;
                case 11:
                    _barLength = value.RealVal;
                    break;
                case 12:
                    _barRole =
                        (IfcReinforcingBarRoleEnum)
                        Enum.Parse(typeof (IfcReinforcingBarRoleEnum), value.EnumVal, true);
                    break;
                case 13:
                    _barSurface =
                        (IfcReinforcingBarSurfaceEnum)
                        Enum.Parse(typeof (IfcReinforcingBarSurfaceEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_barRole == IfcReinforcingBarRoleEnum.USERDEFINED && !ObjectType.HasValue)
                baseErr +=
                    "WR1 ReinforcingBar : The attribute ObjectType shall be given, if the bar role type is set to USERDEFINED.\n";
            return baseErr;
        }
    }
}