using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// The IfcLightDistributionData defines the luminous intensity of a light source given at a 
    /// particular main plane angle. It is based on some standardized light distribution curves,
    /// the MainPlaneAngle is either the
    /// 
    ///     A angle; if the IfcLightDistributionCurveEnum is set to TYPE_A
    ///     B angle; if the IfcLightDistributionCurveEnum is set to TYPE_B
    ///     C angle; if the IfcLightDistributionCurveEnum is set to TYPE_C
    /// 
    /// For each MainPlaneAngle (considered as being the row of a table) a list of SecondaryPlaneAngle's 
    /// are given (considered to be the columns of a table). They are either the:
    /// 
    ///     α angle; if the IfcLightDistributionCurveEnum is set to TYPE_A
    ///     β angle; if the IfcLightDistributionCurveEnum is set to TYPE_B
    ///     γ angle; if the IfcLightDistributionCurveEnum is set to TYPE_C
    /// 
    /// For each pair of MainPlaneAngle and SecondaryPlaneAngle the LuminousIntensity is provides 
    /// (the unit is given by the IfcUnitAssignment referring to the LuminousIntensityDistributionUnit, normally cd/klm).
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightDistributionData : IPersistIfcEntity, ISupportChangeNotification
    {
        #region fields
        IfcPlaneAngleMeasure _mainPlaneAngle;
        XbimSet<IfcPlaneAngleMeasure> _secondaryPlaneAngle;
        XbimSet<IfcLuminousIntensityDistributionMeasure> _luminousIntensity;
        #endregion

        public IfcLightDistributionData()
        {
            _secondaryPlaneAngle = new XbimSet<IfcPlaneAngleMeasure>(this);
            _luminousIntensity = new XbimSet<IfcLuminousIntensityDistributionMeasure>(this);
        }

        /// <summary>
        /// The main plane angle (A, B or C angles, according to the light distribution curve chosen). 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcPlaneAngleMeasure MainPlaneAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _mainPlaneAngle;
            }
            set { this.SetModelValue(this, ref _mainPlaneAngle, value, v => MainPlaneAngle = v, "MainPlaneAngle"); }
        }

        /// <summary>
        /// The list of secondary plane angles (the α, β or γ angles) according 
        /// to the light distribution curve chosen. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcPlaneAngleMeasure> SecondaryPlaneAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _secondaryPlaneAngle;
            }
            set { this.SetModelValue(this, ref _secondaryPlaneAngle, value, v => SecondaryPlaneAngle = v, "SecondaryPlaneAngle"); }
        }

        /// <summary>
        /// The luminous intensity distribution measure for this pair of main and secondary 
        /// plane angles according to the light distribution curve chosen. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcLuminousIntensityDistributionMeasure> LuminousIntensity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _luminousIntensity;
            }
            set { this.SetModelValue(this, ref _luminousIntensity, value, v => LuminousIntensity = v, "LuminousIntensity"); }
        }


        private IModel _model;
        private int _entityLabel;
        private bool _activated;

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock (this) { if (_model != null && !_activated) _activated = _model.Activate(this, false) > 0; }
            if (write) _model.Activate(this, write);
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _model = model;
            _entityLabel = entityLabel;
            _activated = activated;
        }

        IModel IPersistIfcEntity.ModelOf
        {
            get { return _model; }
        }

        int IPersistIfcEntity.EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfc.IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _mainPlaneAngle = value.RealVal;
                    break;
                case 1:
                    _secondaryPlaneAngle.Add(value.RealVal);
                    break;
                case 2:
                    _luminousIntensity.Add(value.RealVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        string IPersistIfc.WhereRule()
        {
            return "";
        }

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized] //don't serialize event
        private event PropertyChangingEventHandler PropertyChanging;

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        event System.ComponentModel.PropertyChangingEventHandler System.ComponentModel.INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }
    }
}
