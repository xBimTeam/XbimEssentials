using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// The IfcTextureCoordinate a an abstract supertype of the different kinds 
    /// to apply texture coordinates to geometries. For vertex based geometries
    /// an explicit assignment of 2D texture vertices to the 3D geometry vertices 
    /// is supported, in addition there can be a procedural description of texture
    /// coordinates. For parametrically described base geometry types a default 
    /// mapping procedure is given.
    /// </summary>
    [IfcPersistedEntity]
    public abstract class IfcTextureCoordinate : IPersistIfcEntity, ISupportChangeNotification
    {
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 1)]
        public virtual IEnumerable<IfcAnnotationSurface> AnnotatedSurface
        {
            get { return ModelOf.Instances.Where<IfcAnnotationSurface>(r => r.TextureCoordinates == this); }
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

        public IModel ModelOf
        {
            get { return _model; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        public abstract void IfcParse(int propIndex, IPropertyValue value);


        public abstract string WhereRule();
        

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
