#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadPlanarForce.cs
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
    public class IfcStructuralLoadPlanarForce : IfcStructuralLoadStatic
    {
        #region Fields

        private IfcPlanarForceMeasure? _planarForceX;
        private IfcPlanarForceMeasure? _planarForceY;
        private IfcPlanarForceMeasure? _planarForceZ;

        #endregion

        #region Properties

        /// <summary>
        ///   Force value in x-direction.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcPlanarForceMeasure? PlanarForceX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _planarForceX;
            }
            set { this.SetModelValue(this, ref _planarForceX, value, v => PlanarForceX = v, "PlanarForceX"); }
        }

        /// <summary>
        ///   Force value in y-direction.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcPlanarForceMeasure? PlanarForceY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _planarForceY;
            }
            set { this.SetModelValue(this, ref _planarForceY, value, v => PlanarForceY = v, "PlanarForceY"); }
        }

        /// <summary>
        ///   Force value in z-direction.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPlanarForceMeasure? PlanarForceZ
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _planarForceZ;
            }
            set { this.SetModelValue(this, ref _planarForceZ, value, v => PlanarForceZ = v, "PlanarForceZ"); }
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
                    _planarForceX = value.RealVal;
                    break;
                case 2:
                    _planarForceY = value.RealVal;
                    break;
                case 3:
                    _planarForceZ = value.RealVal;
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