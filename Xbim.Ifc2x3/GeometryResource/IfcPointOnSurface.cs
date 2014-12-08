#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPointOnSurface.cs
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
    public class IfcPointOnSurface : IfcPoint
    {
        #region Fields

        private IfcSurface _basisSurface;
        private IfcParameterValue _pointParameterU;
        private IfcParameterValue _pointParameterV;

        #endregion

        #region Part 21 Step file representation

        /// <summary>
        ///   The surface to which the parameter values relate.
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
        ///   The first parameter value of the point location.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcParameterValue PointParameterU
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pointParameterU;
            }
            set
            {
                this.SetModelValue(this, ref _pointParameterU, value, v => PointParameterU = v,
                                           "PointParameterU");
            }
        }

        /// <summary>
        ///   The second parameter value of the point location.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcParameterValue PointParameterV
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pointParameterV;
            }
            set
            {
                this.SetModelValue(this, ref _pointParameterV, value, v => PointParameterV = v,
                                           "PointParameterV");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _basisSurface = (IfcSurface) value.EntityVal;
                    break;
                case 1:
                    _pointParameterU = value.RealVal;
                    break;
                case 2:
                    _pointParameterV = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override IfcDimensionCount Dim
        {
            get { return _basisSurface == null ? new IfcDimensionCount(0) : _basisSurface.Dim; }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}