#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTendon.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralElementsDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcTendon : IfcReinforcingElement
    {
        #region Fields

        private IfcTendonTypeEnum _predefinedType;
        private IfcPositiveLengthMeasure _nominalDiameter;
        private IfcAreaMeasure _crossSectionArea;
        private IfcForceMeasure? _tensionForce;
        private IfcPressureMeasure? _preStress;
        private IfcNormalisedRatioMeasure? _frictionCoefficient;
        private IfcPositiveLengthMeasure? _anchorageSlip;
        private IfcPositiveLengthMeasure? _minCurvatureRadius;

        #endregion

        #region Properties

        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcTendonTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        [IfcAttribute(11, IfcAttributeState.Mandatory)]
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

        [IfcAttribute(12, IfcAttributeState.Mandatory)]
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

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcForceMeasure? TensionForce
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _tensionForce;
            }
            set { this.SetModelValue(this, ref _tensionForce, value, v => TensionForce = v, "TensionForce"); }
        }

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcPressureMeasure? PreStress
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _preStress;
            }
            set { this.SetModelValue(this, ref _preStress, value, v => PreStress = v, "PreStress"); }
        }

        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? FrictionCoefficient
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _frictionCoefficient;
            }
            set
            {
                this.SetModelValue(this, ref _frictionCoefficient, value, v => FrictionCoefficient = v,
                                           "FrictionCoefficient");
            }
        }

        [IfcAttribute(16, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? AnchorageSlip
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _anchorageSlip;
            }
            set { this.SetModelValue(this, ref _anchorageSlip, value, v => AnchorageSlip = v, "AnchorageSlip"); }
        }

        [IfcAttribute(17, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MinCurvatureRadius
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _minCurvatureRadius;
            }
            set
            {
                this.SetModelValue(this, ref _minCurvatureRadius, value, v => MinCurvatureRadius = v,
                                           "MinCurvatureRadius");
            }
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
                    _predefinedType = (IfcTendonTypeEnum) Enum.Parse(typeof (IfcTendonTypeEnum), value.EnumVal, true);
                    break;
                case 10:
                    _nominalDiameter = value.RealVal;
                    break;
                case 11:
                    _crossSectionArea = value.RealVal;
                    break;
                case 12:
                    _tensionForce = value.RealVal;
                    break;
                case 13:
                    _preStress = value.RealVal;
                    break;
                case 14:
                    _frictionCoefficient = value.RealVal;
                    break;
                case 15:
                    _anchorageSlip = value.RealVal;
                    break;
                case 16:
                    _minCurvatureRadius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_predefinedType == IfcTendonTypeEnum.USERDEFINED && !ObjectType.HasValue)
                baseErr +=
                    "WR1 Tendon : The attribute ObjectType shall be given, if the bar predefined type is set to USERDEFINED.\n";
            return baseErr;
        }
    }
}