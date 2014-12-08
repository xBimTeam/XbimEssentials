#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadSingleForce.cs
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
    public class IfcStructuralLoadSingleForce : IfcStructuralLoadStatic
    {
        #region Fields

        private IfcForceMeasure? _forceX;
        private IfcForceMeasure? _forceY;
        private IfcForceMeasure? _forceZ;
        private IfcTorqueMeasure? _momentX;
        private IfcTorqueMeasure? _momentY;
        private IfcTorqueMeasure? _momentZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Force value in x-direction.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcForceMeasure? ForceX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _forceX;
            }
            set { this.SetModelValue(this, ref _forceX, value, v => ForceX = v, "ForceX"); }
        }

        /// <summary>
        ///   Force value in y-direction.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcForceMeasure? ForceY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _forceY;
            }
            set { this.SetModelValue(this, ref _forceY, value, v => ForceY = v, "ForceY"); }
        }

        /// <summary>
        ///   Force value in z-direction.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcForceMeasure? ForceZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _forceZ;
            }
            set { this.SetModelValue(this, ref _forceZ, value, v => ForceZ = v, "ForceZ"); }
        }

        /// <summary>
        ///   Moment about the x-axis.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcTorqueMeasure? MomentX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _momentX;
            }
            set { this.SetModelValue(this, ref _momentX, value, v => MomentX = v, "MomentX"); }
        }

        /// <summary>
        ///   Moment about the y-axis.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcTorqueMeasure? MomentY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _momentY;
            }
            set { this.SetModelValue(this, ref _momentY, value, v => MomentY = v, "MomentY"); }
        }

        /// <summary>
        ///   Moment about the z-axis.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcTorqueMeasure? MomentZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _momentZ;
            }
            set { this.SetModelValue(this, ref _momentZ, value, v => MomentZ = v, "MomentZ"); }
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
                    _forceX = value.RealVal;
                    break;
                case 2:
                    _forceY = value.RealVal;
                    break;
                case 3:
                    _forceZ = value.RealVal;
                    break;
                case 4:
                    _momentX = value.RealVal;
                    break;
                case 5:
                    _momentY = value.RealVal;
                    break;
                case 6:
                    _momentZ = value.RealVal;
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