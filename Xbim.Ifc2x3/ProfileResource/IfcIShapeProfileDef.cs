#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcIShapeProfileDef.cs
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
    public class IfcIShapeProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _overallWidth;
        private IfcPositiveLengthMeasure _overallDepth;
        private IfcPositiveLengthMeasure _webThickness;
        private IfcPositiveLengthMeasure _flangeThickness;
        private IfcPositiveLengthMeasure? _filletRadius;

        #endregion

        #region Properties

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure OverallWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallWidth;
            }
            set { this.SetModelValue(this, ref _overallWidth, value, v => OverallWidth = v, "OverallWidth"); }
        }

        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure OverallDepth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallDepth;
            }
            set { this.SetModelValue(this, ref _overallDepth, value, v => OverallDepth = v, "OverallDepth"); }
        }

        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure WebThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _webThickness;
            }
            set { this.SetModelValue(this, ref _webThickness, value, v => WebThickness = v, "WebThickness"); }
        }

        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure FlangeThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _flangeThickness;
            }
            set
            {
                this.SetModelValue(this, ref _flangeThickness, value, v => FlangeThickness = v,
                                           "FlangeThickness");
            }
        }

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? FilletRadius
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _filletRadius;
            }
            set { this.SetModelValue(this, ref _filletRadius, value, v => FilletRadius = v, "FilletRadius"); }
        }

        #endregion

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
                    _overallWidth = value.RealVal;
                    break;
                case 4:
                    _overallDepth = value.RealVal;
                    break;
                case 5:
                    _webThickness = value.RealVal;
                    break;
                case 6:
                    _flangeThickness = value.RealVal;
                    break;
                case 7:
                    _filletRadius = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string err = "";
            if (_flangeThickness >= _overallDepth/2)
                err +=
                    "WR1 IShapeProfileDef : The thickness of the flange shall be less than half of the overall depth.\n";
            if (_webThickness >= _overallWidth)
                err += "WR2 IShapeProfileDef :The web thickness shall be less then the overall width.\n";
            if (_filletRadius.HasValue &&
                (_filletRadius.Value > (_overallWidth - _webThickness)/2 ||
                 _filletRadius.Value > (_overallDepth - (2*_flangeThickness))/2))
                err +=
                    "WR3 IShapeProfileDef : The FilletRadius, if given, should be within the range of allowed values.\n";
            return err;
        }
    }
}