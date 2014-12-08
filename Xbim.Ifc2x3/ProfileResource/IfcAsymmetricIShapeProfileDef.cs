#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAsymmetricIShapeProfileDef.cs
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

namespace Xbim.Ifc2x3.ProfileResource
{
    [IfcPersistedEntityAttribute]
    public class IfcAsymmetricIShapeProfileDef : IfcIShapeProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _topFlangeWidth;
        private IfcPositiveLengthMeasure? _topFlangeThickness;
        private IfcPositiveLengthMeasure? _topFlangeFilletRadius;
        private IfcPositiveLengthMeasure? _centreOfGravityInY;

        #endregion

        #region Properties

        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure TopFlangeWidth
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _topFlangeWidth;
            }
            set { this.SetModelValue(this, ref _topFlangeWidth, value, v => TopFlangeWidth = v, "TopFlangeWidth"); }
        }
        
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? TopFlangeThickness
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _topFlangeThickness;
            }
            set { this.SetModelValue(this, ref _topFlangeThickness, value, v => TopFlangeThickness = v, "TopFlangeThickness"); }
        }

        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? TopFlangeFilletRadius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _topFlangeFilletRadius;
            }
            set { this.SetModelValue(this, ref _topFlangeFilletRadius, value, v => TopFlangeFilletRadius = v, "TopFlangeFilletRadius"); }
        }

        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? CentreOfGravityInY
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _centreOfGravityInY;
            }
            set { this.SetModelValue(this, ref _centreOfGravityInY, value, v => CentreOfGravityInY = v, "CentreOfGravityInY"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _topFlangeWidth = value.RealVal;
                    break;
                case 9:
                    _topFlangeThickness = value.RealVal;
                    break;
                case 10:
                    _topFlangeFilletRadius = value.RealVal;
                    break;
                case 11:
                    _centreOfGravityInY = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}