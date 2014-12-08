#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRectangleProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcRectangleProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _XDim;
        private IfcPositiveLengthMeasure _YDim;

        #endregion

        #region Part 21 Step file Parse routines


        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure XDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _XDim;
            }
            set { this.SetModelValue(this, ref _XDim, value, v => XDim = v, "XDim"); }
        }



        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure YDim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _YDim;
            }
            set { this.SetModelValue(this, ref _YDim, value, v => YDim = v, "YDim"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _XDim = value.RealVal;
                    break;
                case 4:
                    _YDim = value.RealVal;
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