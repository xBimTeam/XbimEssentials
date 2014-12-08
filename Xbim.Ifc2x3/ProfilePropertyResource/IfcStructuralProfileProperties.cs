#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralProfileProperties.cs
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

namespace Xbim.Ifc2x3.ProfilePropertyResource
{
    public class IfcStructuralProfileProperties : IfcGeneralProfileProperties
    {
        #region Fields

        private IfcMomentOfInertiaMeasure? _torsionalConstantX;
        private IfcMomentOfInertiaMeasure? _momentOfInertiaYZ;
        private IfcMomentOfInertiaMeasure? _momentOfInertiaY;
        private IfcMomentOfInertiaMeasure? _momentOfInertiaZ;
        private IfcWarpingConstantMeasure? _warpingConstant;
        private IfcLengthMeasure? _shearCentreZ;
        private IfcLengthMeasure? _shearCentreY;
        private IfcAreaMeasure? _shearDeformationAreaZ;
        private IfcAreaMeasure? _shearDeformationAreaY;
        private IfcSectionModulusMeasure? _maximumSectionModulusY;
        private IfcSectionModulusMeasure? _minimumSectionModulusY;
        private IfcSectionModulusMeasure? _maximumSectionModulusZ;
        private IfcSectionModulusMeasure? _minimumSectionModulusZ;
        private IfcLengthMeasure? _centreOfGravityInX;
        private IfcLengthMeasure? _centreOfGravityInY;

        #endregion

        #region Properties

        [Ifc(8, IfcAttributeState.Optional)]
        public IfcMomentOfInertiaMeasure? TorsionalConstantX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._torsionalConstantX;
            }
            set
            {
                this.SetModelValue(this, ref this._torsionalConstantX, value, v => TorsionalConstantX = v,
                                           "TorsionalConstantX");
            }
        }

        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcMomentOfInertiaMeasure? MomentOfInertiaYZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._momentOfInertiaYZ;
            }
            set
            {
                this.SetModelValue(this, ref this._momentOfInertiaYZ, value, v => MomentOfInertiaYZ = v,
                                           "MomentOfInertiaYZ");
            }
        }

        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcMomentOfInertiaMeasure? MomentOfInertiaY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._momentOfInertiaY;
            }
            set
            {
                this.SetModelValue(this, ref this._momentOfInertiaY, value, v => MomentOfInertiaY = v,
                                           "MomentOfInertiaY");
            }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcMomentOfInertiaMeasure? MomentOfInertiaZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._momentOfInertiaZ;
            }
            set
            {
                this.SetModelValue(this, ref this._momentOfInertiaZ, value, v => MomentOfInertiaZ = v,
                                           "MomentOfInertiaZ");
            }
        }

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcWarpingConstantMeasure? WarpingConstant
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._warpingConstant;
            }
            set
            {
                this.SetModelValue(this, ref this._warpingConstant, value, v => WarpingConstant = v,
                                           "WarpingConstant");
            }
        }

        [IfcAttribute(13, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ShearCentreZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._shearCentreZ;
            }
            set { this.SetModelValue(this, ref this._shearCentreZ, value, v => ShearCentreZ = v, "ShearCentreZ"); }
        }

        [IfcAttribute(14, IfcAttributeState.Optional)]
        public IfcLengthMeasure? ShearCentreY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._shearCentreY;
            }
            set { this.SetModelValue(this, ref this._shearCentreY, value, v => ShearCentreY = v, "ShearCentreY"); }
        }

        [IfcAttribute(15, IfcAttributeState.Optional)]
        public IfcAreaMeasure? ShearDeformationAreaZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._shearDeformationAreaZ;
            }
            set
            {
                this.SetModelValue(this, ref this._shearDeformationAreaZ, value, v => ShearDeformationAreaZ = v,
                                           "ShearDeformationAreaZ");
            }
        }

        [IfcAttribute(16, IfcAttributeState.Optional)]
        public IfcAreaMeasure? ShearDeformationAreaY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._shearDeformationAreaY;
            }
            set
            {
                this.SetModelValue(this, ref this._shearDeformationAreaY, value, v => ShearDeformationAreaY = v,
                                           "ShearDeformationAreaY");
            }
        }

        [IfcAttribute(17, IfcAttributeState.Optional)]
        public IfcSectionModulusMeasure? MaximumSectionModulusY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._maximumSectionModulusY;
            }
            set
            {
                this.SetModelValue(this, ref this._maximumSectionModulusY, value,
                                           v => MaximumSectionModulusY = v, "MaximumSectionModulusY");
            }
        }

        [IfcAttribute(18, IfcAttributeState.Optional)]
        public IfcSectionModulusMeasure? MinimumSectionModulusY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._minimumSectionModulusY;
            }
            set
            {
                this.SetModelValue(this, ref this._minimumSectionModulusY, value,
                                           v => MinimumSectionModulusY = v, "MinimumSectionModulusY");
            }
        }

        [IfcAttribute(19, IfcAttributeState.Optional)]
        public IfcSectionModulusMeasure? MaximumSectionModulusZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._maximumSectionModulusZ;
            }
            set
            {
                this.SetModelValue(this, ref this._maximumSectionModulusZ, value,
                                           v => MaximumSectionModulusZ = v, "MaximumSectionModulusZ");
            }
        }

        [IfcAttribute(20, IfcAttributeState.Optional)]
        public IfcSectionModulusMeasure? MinimumSectionModulusZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._minimumSectionModulusZ;
            }
            set
            {
                this.SetModelValue(this, ref this._minimumSectionModulusZ, value,
                                           v => MinimumSectionModulusZ = v, "MinimumSectionModulusZ");
            }
        }

        [IfcAttribute(21, IfcAttributeState.Optional)]
        public IfcLengthMeasure? CentreOfGravityInX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._centreOfGravityInX;
            }
            set
            {
                this.SetModelValue(this, ref this._centreOfGravityInX, value, v => CentreOfGravityInX = v,
                                           "CentreOfGravityInX");
            }
        }

        [IfcAttribute(22, IfcAttributeState.Optional)]
        public IfcLengthMeasure? CentreOfGravityInY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this._centreOfGravityInY;
            }
            set
            {
                this.SetModelValue(this, ref this._centreOfGravityInY, value, v => CentreOfGravityInY = v,
                                           "CentreOfGravityInY");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _torsionalConstantX = value.RealVal;
                    break;
                case 8:
                    _momentOfInertiaYZ = value.RealVal;
                    break;
                case 9:
                    _momentOfInertiaY = value.RealVal;
                    break;
                case 10:
                    _momentOfInertiaZ = value.RealVal;
                    break;
                case 11:
                    _warpingConstant = value.RealVal;
                    break;
                case 12:
                    _shearCentreZ = value.RealVal;
                    break;
                case 13:
                    _shearCentreY = value.RealVal;
                    break;
                case 14:
                    _shearDeformationAreaZ = value.RealVal;
                    break;
                case 15:
                    _shearDeformationAreaY = value.RealVal;
                    break;
                case 16:
                    _maximumSectionModulusY = value.RealVal;
                    break;
                case 17:
                    _minimumSectionModulusY = value.RealVal;
                    break;
                case 18:
                    _maximumSectionModulusZ = value.RealVal;
                    break;
                case 19:
                    _minimumSectionModulusZ = value.RealVal;
                    break;
                case 20:
                    _centreOfGravityInX = value.RealVal;
                    break;
                case 21:
                    _centreOfGravityInY = value.RealVal;
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}