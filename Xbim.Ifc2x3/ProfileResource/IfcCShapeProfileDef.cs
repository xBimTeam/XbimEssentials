#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCShapeProfileDef.cs
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
    public class IfcCShapeProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _depth;
        private IfcPositiveLengthMeasure _width;
        private IfcPositiveLengthMeasure _wallThickness;
        private IfcPositiveLengthMeasure _girth;
        private IfcPositiveLengthMeasure? _internalFilletRadius;
        private IfcPositiveLengthMeasure? _centreOfGravityInX;

        #endregion

        #region Properties

        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Depth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _depth;
            }
            set { this.SetModelValue(this, ref _depth, value, v => Depth = v, "Depth"); }
        }

        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Width
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _width;
            }
            set { this.SetModelValue(this, ref _width, value, v => Width = v, "Width"); }
        }

        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure WallThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _wallThickness;
            }
            set { this.SetModelValue(this, ref _wallThickness, value, v => WallThickness = v, "WallThickness"); }
        }

        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Girth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _girth;
            }
            set { this.SetModelValue(this, ref _girth, value, v => Girth = v, "Girth"); }
        }

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? InternalFilletRadius
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _internalFilletRadius;
            }
            set
            {
                this.SetModelValue(this, ref _internalFilletRadius, value, v => InternalFilletRadius = v,
                                           "InternalFilletRadius");
            }
        }

        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? CentreOfGravityInX
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _centreOfGravityInX;
            }
            set
            {
                this.SetModelValue(this, ref _centreOfGravityInX, value, v => CentreOfGravityInX = v,
                                           "CentreOfGravityInX");
            }
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
                    _depth = value.RealVal;
                    break;
                case 4:
                    _width = value.RealVal;
                    break;
                case 5:
                    _wallThickness = value.RealVal;
                    break;
                case 6:
                    _girth = value.RealVal;
                    break;
                case 7:
                    _internalFilletRadius = value.RealVal;
                    break;
                case 8:
                    _centreOfGravityInX = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            string err = "";
            if (_girth >= _depth/2)
                err += "WR1 CShapeProfileDef : The girth shall be smaller than half of the depth.\n";
            if (_internalFilletRadius.HasValue &&
                (_internalFilletRadius.Value >= _depth/2 || _internalFilletRadius.Value >= _width/2))
                err +=
                    "WR2 CShapeProfileDef : If the value for InternalFilletRadius is given, it shall be smaller than half of the Depth and half of the Width. \n";
            if (_wallThickness >= _width/2 || _wallThickness >= _depth/2)
                err +=
                    "WR3 CShapeProfileDef : The WallThickness shall be smaller than half of the Width and half of the Depth.\n";
            return err;
        }
    }
}