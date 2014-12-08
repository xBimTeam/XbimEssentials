#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCircle.cs
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
    public class IfcCircle : IfcConic
    {
        #region Fields

        private IfcPositiveLengthMeasure _radius;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The radius of the circle, which shall be greater than zero.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Radius
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _radius;
            }
            set { this.SetModelValue(this, ref _radius, value, v => Radius = v, "Radius"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _radius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}