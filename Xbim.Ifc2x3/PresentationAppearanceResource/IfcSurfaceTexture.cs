#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceTexture.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    ///   Definition from IAI: An IfcSurfaceTexture provides a 2-dimensional image-based texture map. 
    ///   It can either be given by referencing an external image file through an URL reference (IfcImageTexture), or by explicitly including an array of pixels (IfcPixelTexture).
    ///   The following additional definitions from ISO/IEC FCD 19775:200x, the Extensible 3D (X3D) specification, apply:
    ///   Texture: An image used in a texture map to create visual appearance effects when applied to geometry nodes. 
    ///   Texture map: A texture plus the general parameters necessary for mapping the texture to geometry. 
    ///   Texture maps are defined by 2D images that contain an array of colour values describing the texture. 
    ///   The texture map values are interpreted differently depending on the number of components in the texture map and the specifics of the image format.
    ///   In general, texture maps may be described using one of the following forms: 
    /// 
    ///   Intensity textures (one-component) 
    ///   Intensity plus alpha opacity textures (two-component) 
    ///   Full RGB textures (three-component) 
    ///   Full RGB plus alpha opacity textures (four-component) 
    /// 
    ///   NOTE: Most image formats specify an alpha opacity, not transparency (where alpha = 1 - transparency).
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcSurfaceTexture : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                              INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcSurfaceTexture root = (IfcSurfaceTexture)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcSurfaceTexture left, IfcSurfaceTexture right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcSurfaceTexture left, IfcSurfaceTexture right)
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

        private IfcBoolean _repeatS;
        private IfcBoolean _repeatT;
        private IfcSurfaceTextureEnum _textureType;
        private IfcCartesianTransformationOperator2D _textureTransform;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The RepeatS field specifies how the texture wraps in the S direction. 
        ///   If RepeatS is TRUE (the default), the texture map is repeated outside the [0.0, 1.0] texture coordinate range in the S direction so that it fills the shape. 
        ///   If repeatS is FALSE, the texture coordinates are clamped in the S direction to lie within the [0.0, 1.0] range.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcBoolean RepeatS
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _repeatS;
            }
            set { this.SetModelValue(this, ref _repeatS, value, v => RepeatS = v, "RepeatS"); }
        }

        /// <summary>
        ///   The RepeatT field specifies how the texture wraps in the T direction. 
        ///   If RepeatT is TRUE (the default), the texture map is repeated outside the [0.0, 1.0] texture coordinate range in the T direction so that it fills the shape. 
        ///   If repeatT is FALSE, the texture coordinates are clamped in the T direction to lie within the [0.0, 1.0] range.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBoolean RepeatT
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _repeatT;
            }
            set { this.SetModelValue(this, ref _repeatT, value, v => RepeatT = v, "RepeatT"); }
        }

        /// <summary>
        ///   Identifies the predefined types of image map from which the type required may be set.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcSurfaceTextureEnum TextureType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _textureType;
            }
            set { this.SetModelValue(this, ref _textureType, value, v => TextureType = v, "TextureType"); }
        }

        /// <summary>
        ///   These parameters support changes to the size, orientation, and position of textures on shapes. 
        ///   Note that these operations appear reversed when viewed on the surface of geometry. 
        ///   For example, a scale value of (2 2) will scale the texture coordinates and have the net effect of 
        ///   shrinking the texture size by a factor of 2 (texture coordinates are twice as large and thus cause the texture to repeat). 
        ///   A translation of (0.5 0.0) translates the texture coordinates +.5 units along the S-axis and has the net effect of translating the texture -0.5 along the S-axis 
        ///   on the geometry's surface. A rotation of PI/2 of the texture coordinates results in a -PI/2 rotation of the texture on the geometry.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcCartesianTransformationOperator2D TextureTransform
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _textureTransform;
            }
            set
            {
                this.SetModelValue(this, ref _textureTransform, value, v => TextureTransform = v,
                                           "TextureTransform");
            }
        }


        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _repeatS = value.BooleanVal;
                    break;
                case 1:
                    _repeatT = value.BooleanVal;
                    break;
                case 2:
                    _textureType =
                        (IfcSurfaceTextureEnum) Enum.Parse(typeof (IfcSurfaceTextureEnum), value.EnumVal, true);
                    break;
                case 3:
                    _textureTransform = (IfcCartesianTransformationOperator2D) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public abstract string WhereRule();

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
    }
}