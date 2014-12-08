#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyleLighting.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceStyleLighting : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                           IfcSurfaceStyleElementSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcSurfaceStyleLighting root = (IfcSurfaceStyleLighting)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcSurfaceStyleLighting left, IfcSurfaceStyleLighting right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcSurfaceStyleLighting left, IfcSurfaceStyleLighting right)
        {
            return !(left == right);
        }
        #region IPersistIfcEntity Members

        private int _entityLabel;
		bool _activated;

        private IModel _model;

        public IModel ModelOf
        {
            get { return _model; }
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _activated=activated;
			_model = model;
            _entityLabel = entityLabel;
        }

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock(this) { if (_model != null && !_activated) _activated = _model.Activate(this, false)>0;  }
            if (write) _model.Activate(this, write);
        }

        #endregion

        #region Fields

        private IfcColourRgb _diffuseTransmissionColour;
        private IfcColourRgb _diffuseReflectionColour;
        private IfcColourRgb _transmissionColour;
        private IfcColourRgb _reflectanceColour;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The degree of diffusion of the transmitted light. In the case of completely transparent materials there is no diffusion. 
        ///   The greater the diffusing power, the smaller the direct component of the transmitted light, up to the point where only diffuse light is produced.
        ///   A value of 1 means totally diffuse for that colour part of the light.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcColourRgb DiffuseTransmissionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _diffuseTransmissionColour;
            }
            set
            {
                this.SetModelValue(this, ref _diffuseTransmissionColour, value,
                                           v => DiffuseTransmissionColour = v, "DiffuseTransmissionColour");
            }
        }

        /// <summary>
        ///   The degree of diffusion of the reflected light. In the case of specular surfaces there is no diffusion. 
        ///   The greater the diffusing power of the reflecting surface, the smaller the specular component of the reflected light,
        ///   up to the point where only diffuse light is produced. 
        ///   A value of 1 means totally diffuse for that colour part of the light.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcColourRgb DiffuseReflectionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _diffuseReflectionColour;
            }
            set
            {
                this.SetModelValue(this, ref _diffuseReflectionColour, value, v => DiffuseReflectionColour = v,
                                           "DiffuseReflectionColour");
            }
        }

        /// <summary>
        ///   Describes how the light falling on a body is totally or partially transmitted.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcColourRgb TransmissionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transmissionColour;
            }
            set
            {
                this.SetModelValue(this, ref _transmissionColour, value, v => TransmissionColour = v,
                                           "TransmissionColour");
            }
        }

        /// <summary>
        ///   A coefficient that determines the extent that the light falling onto a surface is fully or partially reflected.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcColourRgb ReflectanceColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _reflectanceColour;
            }
            set
            {
                this.SetModelValue(this, ref _reflectanceColour, value, v => ReflectanceColour = v,
                                           "ReflectanceColour");
            }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _diffuseTransmissionColour = (IfcColourRgb) value.EntityVal;
                    break;
                case 1:
                    _diffuseReflectionColour = (IfcColourRgb) value.EntityVal;
                    break;
                case 2:
                    _transmissionColour = (IfcColourRgb) value.EntityVal;
                    break;
                case 3:
                    _reflectanceColour = (IfcColourRgb) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized] //don't serialize events
            private event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }

        #endregion

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion
    }
}