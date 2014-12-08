#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPointOnCurve.cs
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
    public class IfcPointOnCurve : IfcPoint
    {
        #region Fields

        private IfcCurve _basisCurve;
        private IfcParameterValue _pointParameter;

        #endregion

        #region Part 21 Step file representation

        /// <summary>
        ///   The curve to which point parameter relates.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCurve BasisCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _basisCurve;
            }
            set { this.SetModelValue(this, ref _basisCurve, value, v => BasisCurve = v, "BasisCurve"); }
        }


        /// <summary>
        ///   The parameter value of the point location.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcParameterValue PointParameter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _pointParameter;
            }
            set { this.SetModelValue(this, ref _pointParameter, value, v => PointParameter = v, "PointParameter"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _basisCurve = (IfcCurve) value.EntityVal;
                    break;
                case 1:
                    _pointParameter = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        
        public override IfcDimensionCount Dim
        {
            get { return _basisCurve == null ? (IfcDimensionCount) 0 : _basisCurve.Dim; }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}