#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeneralProfileProperties.cs
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

namespace Xbim.Ifc2x3.ProfilePropertyResource
{
    public class IfcGeneralProfileProperties : IfcProfileProperties
    {
        #region Fields

        private IfcMassPerLengthMeasure? _physicalWeight;
        private IfcPositiveLengthMeasure? _perimeter;
        private IfcPositiveLengthMeasure? _minimumPlateThickness;
        private IfcPositiveLengthMeasure? _maximumPlateThickness;
        private IfcAreaMeasure? _crossSectionArea;

        #endregion

        #region Properties

        /// <summary>
        ///   Weight of an imaginary steel beam per length, as for example given by the national standards for this profile. Usually measured in [kg/m].
        /// </summary>
        [Ifc(3, IfcAttributeState.Optional)]
        public IfcMassPerLengthMeasure? PhysicalWeight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _physicalWeight;
            }
            set { this.SetModelValue(this, ref _physicalWeight, value, v => PhysicalWeight = v, "PhysicalWeight"); }
        }

        /// <summary>
        ///   Perimeter of the profile for calculating the surface area. Usually measured in [mm].
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? Perimeter
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _perimeter;
            }
            set { this.SetModelValue(this, ref _perimeter, value, v => Perimeter = v, "Perimeter"); }
        }

        /// <summary>
        ///   This value is needed for stress analysis and to handle buckling problems. 
        ///   It can also be derived from the given profile geometry and therefore it is only an optional feature allowing for an explicit description. 
        ///   Usually measured in [mm].
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MinimumPlateThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _minimumPlateThickness;
            }
            set
            {
                this.SetModelValue(this, ref _minimumPlateThickness, value, v => MinimumPlateThickness = v,
                                           "MinimumPlateThickness");
            }
        }

        /// <summary>
        ///   RThis value is needed for stress analysis and to handle buckling problems. 
        ///   It can also be derived from the given profile geometry and therefore it is only an optional feature allowing for an explicit description. 
        ///   Usually measured in [mm].
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcPositiveLengthMeasure? MaximumPlateThickness
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _maximumPlateThickness;
            }
            set
            {
                this.SetModelValue(this, ref _maximumPlateThickness, value, v => MaximumPlateThickness = v,
                                           "MaximumPlateThickness");
            }
        }

        /// <summary>
        ///   Cross sectional area of profile. Usually measured in [mm2].
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcAreaMeasure? CrossSectionArea
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _crossSectionArea;
            }
            set
            {
                this.SetModelValue(this, ref _crossSectionArea, value, v => CrossSectionArea = v,
                                           "CrossSectionArea");
            }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _physicalWeight = value.RealVal;
                    break;
                case 3:
                    _perimeter = value.RealVal;
                    break;
                case 4:
                    _minimumPlateThickness = value.RealVal;
                    break;
                case 5:
                    _maximumPlateThickness = value.RealVal;
                    break;
                case 6:
                    _crossSectionArea = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            if (_crossSectionArea.HasValue && _crossSectionArea <= 0)
                return
                    "WR1 GeneralProfileProperties : The value of the cross section area shall (if given) be greater than zero. ";
            else
                return "";
        }
    }
}