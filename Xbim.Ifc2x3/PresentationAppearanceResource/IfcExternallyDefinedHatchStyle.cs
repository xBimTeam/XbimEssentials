using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    /// The externally defined hatch style is an entity which makes an external reference to a hatching style.
    /// 
    /// NOTE: The allowable values for the name source and item reference, by which the externally defined hatch style is 
    /// identified, need to be determined by implementer agreements.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcExternallyDefinedHatchStyle : IPersistIfcEntity, ISupportChangeNotification
    {

        private IfcLabel? _Location;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? Location
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Location;
            }
            set { this.SetModelValue(this, ref _Location, value, v => Location = v, "Location"); }
        }

        private IfcIdentifier? _ItemReference;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcIdentifier? ItemReference
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ItemReference;
            }
            set { this.SetModelValue(this, ref _ItemReference, value, v => ItemReference = v, "ItemReference"); }
        }

        private IfcLabel? _Name;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? Name
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Name;
            }
            set { this.SetModelValue(this, ref _Name, value, v => Name = v, "Name"); }
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
                    _Location = value.StringVal;
                    break;
                case 1:
                    _ItemReference = value.StringVal;
                    break;
                case 2:
                    _Name = value.StringVal;
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
