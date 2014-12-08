#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadLinearForce.cs
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
    public class IfcStructuralLoadLinearForce : IfcStructuralLoadStatic
    {
        #region Fields

        private IfcLinearForceMeasure? _linearForceX;
        private IfcLinearForceMeasure? _linearForceY;
        private IfcLinearForceMeasure? _linearForceZ;
        private IfcLinearMomentMeasure? _linearMomentX;
        private IfcLinearMomentMeasure? _linearMomentY;
        private IfcLinearMomentMeasure? _linearMomentZ;

        #endregion

        #region Properties

        /// <summary>
        ///   LinearForce value in x-direction.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLinearForceMeasure? LinearForceX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearForceX;
            }
            set { this.SetModelValue(this, ref _linearForceX, value, v => LinearForceX = v, "LinearForceX"); }
        }

        /// <summary>
        ///   LinearForce value in y-direction.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLinearForceMeasure? LinearForceY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearForceY;
            }
            set { this.SetModelValue(this, ref _linearForceY, value, v => LinearForceY = v, "LinearForceY"); }
        }

        /// <summary>
        ///   LinearForce value in z-direction.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcLinearForceMeasure? LinearForceZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearForceZ;
            }
            set { this.SetModelValue(this, ref _linearForceZ, value, v => LinearForceZ = v, "LinearForceZ"); }
        }

        /// <summary>
        ///   LinearMoment about the x-axis.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcLinearMomentMeasure? LinearMomentX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearMomentX;
            }
            set { this.SetModelValue(this, ref _linearMomentX, value, v => LinearMomentX = v, "LinearMomentX"); }
        }

        /// <summary>
        ///   LinearMoment about the y-axis.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcLinearMomentMeasure? LinearMomentY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearMomentY;
            }
            set { this.SetModelValue(this, ref _linearMomentY, value, v => LinearMomentY = v, "LinearMomentY"); }
        }

        /// <summary>
        ///   LinearMoment about the z-axis.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcLinearMomentMeasure? LinearMomentZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _linearMomentZ;
            }
            set { this.SetModelValue(this, ref _linearMomentZ, value, v => LinearMomentZ = v, "LinearMomentZ"); }
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
                    _linearForceX = value.RealVal;
                    break;
                case 2:
                    _linearForceY = value.RealVal;
                    break;
                case 3:
                    _linearForceZ = value.RealVal;
                    break;
                case 4:
                    _linearMomentX = value.RealVal;
                    break;
                case 5:
                    _linearMomentY = value.RealVal;
                    break;
                case 6:
                    _linearMomentZ = value.RealVal;
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