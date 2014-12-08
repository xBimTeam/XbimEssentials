#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyleRefraction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceStyleRefraction : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                             IfcSurfaceStyleElementSelect, INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcSurfaceStyleRefraction root = (IfcSurfaceStyleRefraction)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcSurfaceStyleRefraction left, IfcSurfaceStyleRefraction right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcSurfaceStyleRefraction left, IfcSurfaceStyleRefraction right)
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

        private IfcReal? _refractionIndex;
        private IfcReal? _dispersionFactor;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The index of refraction for all wave lengths of light. 
        ///   The refraction index is the ratio between the speed of light in a vacuum and the speed of light in the medium. 
        ///   E.g. glass has a refraction index of 1.5, whereas water has an index of 1.33
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcReal? RefractionIndex
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _refractionIndex;
            }
            set
            {
                this.SetModelValue(this, ref _refractionIndex, value, v => RefractionIndex = v,
                                           "RefractionIndex");
            }
        }

        /// <summary>
        ///   The Abbe constant given as a fixed ratio between the refractive indices of the material at different wavelengths. 
        ///   A low Abbe number means a high dispersive power. 
        ///   In general this translates to a greater angular spread of the emergent spectrum.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcReal? DispersionFactor
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _dispersionFactor;
            }
            set
            {
                this.SetModelValue(this, ref _dispersionFactor, value, v => DispersionFactor = v,
                                           "DispersionFactor");
            }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _refractionIndex = value.RealVal;
                    break;
                case 1:
                    _dispersionFactor = value.RealVal;
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