#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianTransformationOperator3DnonUniform.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcCartesianTransformationOperator3DnonUniform : IfcCartesianTransformationOperator3D
    {
        #region Fields

        private double? _scale2;
        private double? _scale3;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The scaling value specified for the transformation along the axis 2. This is normally the y scale factor.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public double? Scale2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _scale2;
            }
            set { this.SetModelValue(this, ref _scale2, value, v => Scale2 = v, "Scale2"); }
        }

        /// <summary>
        ///   The scaling value specified for the transformation along the axis 3. This is normally the z scale factor.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public double? Scale3
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _scale3;
            }
            set { this.SetModelValue(this, ref _scale3, value, v => Scale3 = v, "Scale3"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _scale2 = value.RealVal;
                    break;
                case 6:
                    _scale3 = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public double Scl2
        {
            get { return Scale2.HasValue ? Scale2.Value : Scl; }
        }

        public double Scl3
        {
            get { return Scale3.HasValue ? Scale3.Value : Scl; }
        }

        #endregion
    }
}