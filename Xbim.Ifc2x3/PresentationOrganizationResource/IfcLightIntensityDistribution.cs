using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions;
using System.ComponentModel;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// The IfcLightIntensityDistribution defines the the luminous intensity
    /// of a light source that changes according to the direction of the ray. 
    /// It is based on some standardized light distribution curves, which are 
    /// defined by the LightDistributionCurve attribute.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightIntensityDistribution : IfcLightDistributionDataSourceSelect
    {

        public IfcLightIntensityDistribution()
        {
            _distributionData = new XbimSet<IfcLightDistributionData>(this);
        }

        #region fields
         IfcLightDistributionCurveEnum _lightDistributionCurve;
         XbimSet<IfcLightDistributionData> _distributionData;

         /// <summary>
         /// Standardized light distribution curve used to define the luminous intensity of the light in all directions. 
         /// </summary>
         [IfcAttribute(1, IfcAttributeState.Mandatory)]
         public IfcLightDistributionCurveEnum LightDistributionCurve
         {
             get
             {
                 ((IPersistIfcEntity)this).Activate(false);
                 return _lightDistributionCurve;
             }
             set { this.SetModelValue(this, ref _lightDistributionCurve, value, v => LightDistributionCurve = v, "LightDistributionCurve"); }
         }
         
         /// <summary>
         /// Light distribution data applied to the light source. It is defined by a list of 
         /// main plane angles (B or C according to the light distribution curve chosen) that
         /// includes (for each B or C angle) a second list of secondary plane angles (the β or γ angles)
         /// and the according luminous intensity distribution measures. 
         /// </summary>
         [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
         public XbimSet<IfcLightDistributionData> DistributionData
         {
             get
             {
                 ((IPersistIfcEntity)this).Activate(false);
                 return _distributionData;
             }
             set { this.SetModelValue(this, ref _distributionData, value, v => DistributionData = v, "DistributionData"); }
         }
        #endregion

         
        void IPersistIfc.IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _lightDistributionCurve = (IfcLightDistributionCurveEnum)Enum.Parse(typeof(IfcLightDistributionCurveEnum), value.EnumVal, true);
                    break;
                case 1:
                    _distributionData.Add((IfcLightDistributionData)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        string IPersistIfc.WhereRule()
        {
            return "";
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
