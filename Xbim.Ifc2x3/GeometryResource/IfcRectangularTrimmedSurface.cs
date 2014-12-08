#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRectangularTrimmedSurface.cs
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

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcRectangularTrimmedSurface : IfcBoundedSurface, IPlacement3D
    {
        #region Fields

        private IfcSurface _basisSurface;
        private IfcParameterValue _u1;
        private IfcParameterValue _v1;
        private IfcParameterValue _u2;
        private IfcParameterValue _v2;
        private IfcBoolean _uSense;
        private IfcBoolean _vSense;

        #endregion

        #region Part 21 Step file representation

        /// <summary>
        ///   Surface being trimmed.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcSurface BasisSurface
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _basisSurface;
            }
            set { this.SetModelValue(this, ref _basisSurface, value, v => BasisSurface = v, "BasisSurface"); }
        }


        /// <summary>
        ///   First u parametric value.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcParameterValue U1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _u1;
            }
            set { this.SetModelValue(this, ref _u1, value, v => U1 = v, "U1"); }
        }

        /// <summary>
        ///   First v parametric value.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcParameterValue V1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _v1;
            }
            set { this.SetModelValue(this, ref _v1, value, v => V1 = v, "V1"); }
        }

        /// <summary>
        ///   Second u parametric value.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcParameterValue U2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _u2;
            }
            set { this.SetModelValue(this, ref _u2, value, v => U2 = v, "U2"); }
        }

        /// <summary>
        ///   Second v parametric value.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcParameterValue V2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _v2;
            }
            set { this.SetModelValue(this, ref _v2, value, v => V2 = v, "V2"); }
        }


        /// <summary>
        ///   Flag to indicate whether the direction of the first parameter of the trimmed surface agrees with or opposes the sense of u in the basis surface.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcBoolean USense
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _uSense;
            }
            set { this.SetModelValue(this, ref _uSense, value, v => USense = v, "USense"); }
        }

        /// <summary>
        ///   Flag to indicate whether the direction of the second parameter of the trimmed surface agrees with or opposes the sense of v in the basis surface.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcBoolean VSense
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _vSense;
            }
            set { this.SetModelValue(this, ref _vSense, value, v => VSense = v, "VSense"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _basisSurface = (IfcSurface) value.EntityVal;
                    break;
                case 1:
                    _u1 = value.RealVal;
                    break;
                case 2:
                    _v1 = value.RealVal;
                    break;
                case 3:
                    _u2 = value.RealVal;
                    break;
                case 4:
                    _v2 = value.RealVal;
                    break;
                case 5:
                    _uSense = value.BooleanVal;
                    break;
                case 6:
                    _vSense = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        
        public override IfcDimensionCount Dim
        {
            get { return BasisSurface == null ? (IfcDimensionCount) 0 : BasisSurface.Dim; }
        }

        #region IPlacement3D Members

        IfcAxis2Placement3D IPlacement3D.Position
        {
            get { return ((IPlacement3D) BasisSurface).Position; }
        }

        #endregion

        public override string WhereRule()
        {
            string err = "";
            if (_u1 == _u2)
                err += "WR1 RectangularTrimmedSurface : U1 and U2 shall have different values.\n";
            if (_v1 == _v2)
                err += "WR2 RectangularTrimmedSurface : V1 and V2 shall have different values.\n";
            if (!((_basisSurface is IfcElementarySurface && !(_basisSurface is IfcPlane))
                  || (_basisSurface is IfcSurfaceOfRevolution || (USense == (_u2 > _u1)))))
                err +=
                    "WR3 RectangularTrimmedSurface : With exception of those surfaces closed in the U parameter, direction Usense shall be compatible with the ordered parameter values for U.\n";
            if (!(VSense == (_v2 > _v1)))
                err +=
                    "WR4 RectangularTrimmedSurface : Vsense shall be compatible with the ordered parameter values for V.\n";
            return err;
        }
    }
}