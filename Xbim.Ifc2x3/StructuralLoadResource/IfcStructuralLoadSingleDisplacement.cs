#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadSingleDisplacement.cs
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

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    [IfcPersistedEntityAttribute]
    public class IfcStructuralLoadSingleDisplacement : IfcStructuralLoadStatic
    {
        #region Fields

        private IfcLengthMeasure? _displacementX;
        private IfcLengthMeasure? _displacementY;
        private IfcLengthMeasure? _displacementZ;
        private IfcPlaneAngleMeasure? _rotationalDisplacementRX;
        private IfcPlaneAngleMeasure? _rotationalDisplacementRY;
        private IfcPlaneAngleMeasure? _rotationalDisplacementRZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Displacement value in x-direction.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLengthMeasure? DisplacementX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _displacementX;
            }
            set { this.SetModelValue(this, ref _displacementX, value, v => DisplacementX = v, "DisplacementX"); }
        }

        /// <summary>
        ///   Displacement value in y-direction.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLengthMeasure? DisplacementY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _displacementY;
            }
            set { this.SetModelValue(this, ref _displacementY, value, v => DisplacementY = v, "DisplacementY"); }
        }

        /// <summary>
        ///   Displacement value in z-direction.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLengthMeasure? DisplacementZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _displacementZ;
            }
            set { this.SetModelValue(this, ref _displacementZ, value, v => DisplacementZ = v, "DisplacementZ"); }
        }

        /// <summary>
        ///   RotationalDisplacementR about the x-axis.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPlaneAngleMeasure? RotationalDisplacementRX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalDisplacementRX;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalDisplacementRX, value, v => RotationalDisplacementRX = v,
                                           "RotationalDisplacementRX");
            }
        }

        /// <summary>
        ///   RotationalDisplacementR about the y-axis.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPlaneAngleMeasure? RotationalDisplacementRY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalDisplacementRY;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalDisplacementRY, value, v => RotationalDisplacementRY = v,
                                           "RotationalDisplacementRY");
            }
        }

        /// <summary>
        ///   RotationalDisplacementR about the z-axis.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcPlaneAngleMeasure? RotationalDisplacementRZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _rotationalDisplacementRZ;
            }
            set
            {
                this.SetModelValue(this, ref _rotationalDisplacementRZ, value, v => RotationalDisplacementRZ = v,
                                           "RotationalDisplacementRZ");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _displacementX = value.RealVal;
                    break;
                case 2:
                    _displacementY = value.RealVal;
                    break;
                case 3:
                    _displacementZ = value.RealVal;
                    break;
                case 4:
                    _rotationalDisplacementRX = value.RealVal;
                    break;
                case 5:
                    _rotationalDisplacementRY = value.RealVal;
                    break;
                case 6:
                    _rotationalDisplacementRZ = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}