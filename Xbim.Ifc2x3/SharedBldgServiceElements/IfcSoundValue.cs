#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSoundValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.TimeSeriesResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcSoundValue : IfcPropertySetDefinition
    {
        #region Fields

        private IfcTimeSeries _soundLevelTimeSeries;
        private IfcFrequencyMeasure _frequency;
        private IfcDerivedMeasureValue _soundLevelSingleValue;

        #endregion

        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcTimeSeries SoundLevelTimeSeries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _soundLevelTimeSeries;
            }
            set
            {
                this.SetModelValue(this, ref _soundLevelTimeSeries, value, v => SoundLevelTimeSeries = v,
                                           "SoundLevelTimeSeries");
            }
        }

        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcFrequencyMeasure Frequency
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _frequency;
            }
            set { this.SetModelValue(this, ref _frequency, value, v => Frequency = v, "Frequency"); }
        }

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcDerivedMeasureValue SoundLevelSingleValue
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _soundLevelSingleValue;
            }
            set
            {
                this.SetModelValue(this, ref _soundLevelSingleValue, value, v => SoundLevelSingleValue = v,
                                           "SoundLevelSingleValue");
            }
        }

        #region ISupportIfcParser Members

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _soundLevelTimeSeries = (IfcTimeSeries) value.EntityVal;
                    break;
                case 1:
                    _frequency = value.RealVal;
                    break;
                case 2:
                    _soundLevelSingleValue = (IfcDerivedMeasureValue) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (!(_soundLevelSingleValue is IfcSoundPowerMeasure || _soundLevelSingleValue is IfcSoundPressureMeasure))
                return
                    "WR1 SoundValue : SoundLevelSingleValue should be of type SoundPowerMeasure or SoundPressureMeasure";
            else
                return "";
        }
    }
}