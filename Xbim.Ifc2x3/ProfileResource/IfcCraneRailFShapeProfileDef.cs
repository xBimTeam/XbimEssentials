#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCraneRailFShapeProfileDef.cs
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
    public class IfcCraneRailFShapeProfileDef : IfcParameterizedProfileDef
    {
        #region Fields

        private IfcPositiveLengthMeasure _overallHeight;
        private IfcPositiveLengthMeasure _headWidth;
        private IfcPositiveLengthMeasure? _radius;

        private IfcPositiveLengthMeasure _headDepth2;
        private IfcPositiveLengthMeasure _headDepth3;
        private IfcPositiveLengthMeasure _webThickness;

        private IfcPositiveLengthMeasure _baseDepth1;
        private IfcPositiveLengthMeasure _baseDepth2;

        private IfcPositiveLengthMeasure? _centreOfGravityInY;

        #endregion

        #region Properties

        /// <summary>
        ///   Total extent of the height, defined parallel to the y axis of the position coordinate system. See illustration above (= h1).
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure OverallHeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _overallHeight;
            }
            set { this.SetModelValue(this, ref _overallHeight, value, v => OverallHeight = v, "OverallHeight"); }
        }

        /// <summary>
        ///   Total extent of the width of the head, defined parallel to the x axis of the position coordinate system. See illustration above (= b1).
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure HeadWidth
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _headWidth;
            }
            set { this.SetModelValue(this, ref _headWidth, value, v => HeadWidth = v, "HeadWidth"); }
        }

        /// <summary>
        ///   Edge radius according the above illustration (= r1).
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Radius
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _radius;
            }
            set { this.SetModelValue(this, ref _radius, value, v => Radius = v, "Radius"); }
        }

        /// <summary>
        ///   Head depth of the A shape crane rail, see illustration above (= h2).
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure HeadDepth2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _headDepth2;
            }
            set { this.SetModelValue(this, ref _headDepth2, value, v => HeadDepth2 = v, "HeadDepth2"); }
        }

        /// <summary>
        ///   Head depth of the A shape crane rail, see illustration above (= h3).
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure HeadDepth3
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _headDepth3;
            }
            set { this.SetModelValue(this, ref _headDepth3, value, v => HeadDepth3 = v, "HeadDepth3"); }
        }

        /// <summary>
        ///   Thickness of the web of the A shape crane rail. See illustration above (= b3).
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure WebThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _webThickness;
            }
            set { this.SetModelValue(this, ref _webThickness, value, v => WebThickness = v, "WebThickness"); }
        }


        /// <summary>
        ///   Base depth of the A shape crane rail, see illustration above (= s1).
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure BaseDepth1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _baseDepth1;
            }
            set { this.SetModelValue(this, ref _baseDepth1, value, v => BaseDepth1 = v, "BaseDepth1"); }
        }

        /// <summary>
        ///   Base depth of the A shape crane rail, see illustration above (= s2).
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure BaseDepth2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _baseDepth2;
            }
            set { this.SetModelValue(this, ref _baseDepth2, value, v => BaseDepth2 = v, "BaseDepth2"); }
        }


        /// <summary>
        ///   Location of centre of gravity along the y axis measured from the center of the bounding box.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? CentreOfGravityInY
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _centreOfGravityInY;
            }
            set
            {
                this.SetModelValue(this, ref _centreOfGravityInY, value, v => CentreOfGravityInY = v,
                                           "CentreOfGravityInY");
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
                    _overallHeight = value.RealVal;
                    break;
                case 4:
                    _headWidth = value.RealVal;
                    break;
                case 5:
                    _radius = value.RealVal;
                    break;
                case 6:
                    _headDepth2 = value.RealVal;
                    break;
                case 7:
                    _headDepth3 = value.RealVal;
                    break;
                case 8:
                    _webThickness = value.RealVal;
                    break;
                case 9:
                    _baseDepth1 = value.RealVal;
                    break;
                case 10:
                    _baseDepth2 = value.RealVal;
                    break;
                case 11:
                    _centreOfGravityInY = value.RealVal;
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