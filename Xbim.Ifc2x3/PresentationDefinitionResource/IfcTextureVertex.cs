using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// An IfcTextureVertex is a list of 2 (S, T) texture coordinates.
    /// 
    /// The following additional definitions from ISO/IEC FCD 19775:200x, the Extensible 3D (X3D) specification, apply:
    /// 
    ///     Each vertex-based geometry node uses a set of 2D texture coordinates that map textures to vertices. Texture map values ( ImageTexture, PixelTexture) range from [0.0, 1.0] along the S-axis and T-axis. However, texture coordinate values may be in the range (-?,?). Texture coordinates identify a location (and thus a colour value) in the texture map. The horizontal coordinate S is specified first, followed by the vertical coordinate T. If the texture map is repeated in a given direction (S-axis or T-axis), a texture coordinate C (s or t) is mapped into a texture map that has N pixels in the given direction as follows:
    /// 
    ///         Texture map location = (C - floor(C)) × N
    ///         			 
    /// 
    ///     If the texture map is not repeated, the texture coordinates are clamped to the 0.0 to 1.0 range as follows:
    /// 
    ///         Texture map location = N,     if C &gt; 1.0,
    ///                              = 0.0,   if C &lt; 0.0,
    ///                              = C × N, if 0.0 greater or equal to C greater or equal to 1.0.
    /// 
    ///             
    /// 
    ///     Texture coordinates may be transformed (scaled, rotated, translated) by supplying a TextureTransform as a component of the texture's definition.
    /// 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcTextureVertex : IPersistIfcEntity, ISupportChangeNotification
    {
        public IfcTextureVertex()
        {
            _Coordinates = new XbimSet<IfcParameterValue>(this);
        }

        private XbimSet<IfcParameterValue> _Coordinates;

        /// <summary>
        /// The first coordinate[1] is the S, the second coordinate[2] is the T parameter value. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.None, 2, 2)]
        public XbimSet<IfcParameterValue> Coordinates
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Coordinates;
            }
            set { this.SetModelValue(this, ref _Coordinates, value, v => Coordinates = v, "Coordinates"); }
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
                    _Coordinates.Add(value.RealVal);
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
